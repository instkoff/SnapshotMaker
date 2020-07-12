using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Extensions.Logging;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.BL.Services
{
    public class FrameProcessorService : IFrameProcessorService
    {
        private readonly FrameCapturerModel _frameCapturer;
        private readonly ILogger<FrameProcessorService> _logger;
        private List<RoiWrapper> _roiWrappers;
        private Queue<CvImageWrapper> _imageParts;

        public FrameProcessorService(FrameCapturerModel frameCapturer, ILogger<FrameProcessorService> logger)
        {
            _frameCapturer = frameCapturer;
            _logger = logger;
            _roiWrappers = new List<RoiWrapper>();
            _imageParts = new Queue<CvImageWrapper>();
        }

        public void StartProcessing()
        {
            var topBullion = new Rectangle(0, 85, 2591, 989);
            var middleBullion = new Rectangle(0, 425, 2587, 1121);
            var bottomBullion = new Rectangle(0,1037, 2591,931);
            _roiWrappers.Add(new RoiWrapper(topBullion, "Top"));
            _roiWrappers.Add(new RoiWrapper(middleBullion, "Middle"));
            _roiWrappers.Add(new RoiWrapper(bottomBullion, "Bottom"));
            _frameCapturer.OnFrameAdded += (frameQueue, outputFolder) =>
            {
                Task.Run(() => ProcessFrame(frameQueue, outputFolder));
            };
        }

        private void ProcessFrame(ConcurrentQueue<Mat> frameQueue, string outputFolder)
        {

            while (frameQueue.TryDequeue(out var frame))
            {
                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);
                var cvImage = frame.ToImage<Bgr, byte>();
                var snapshotName ="Original"+ "_" + DateTime.Now.ToString("T").Replace(":", "_") + ".jpg";
                var path = Path.Combine(outputFolder, snapshotName);
                cvImage.Save(path);
                CropImage(cvImage);
                while (_imageParts.TryDequeue(out var inputImage))
                {
                    snapshotName = inputImage.Location + "_" + DateTime.Now.ToString("T").Replace(":", "_") + ".jpg";
                    path = Path.Combine(outputFolder, snapshotName);
                    DetectBullion(inputImage.Image, path);
                }
                //cvImage.Save(path);
                frame.Dispose();
                cvImage.Dispose();
            }
        }

        private void CropImage(Image<Bgr, byte> inputImage)
        {
            foreach (var roiWrapper in _roiWrappers)
            {
                inputImage.ROI = roiWrapper.Roi;
                var bullionImage = new CvImageWrapper(inputImage.Copy(), roiWrapper.Location);
                _imageParts.Enqueue(bullionImage);
            }
        }

        private void DetectBullion(Image<Bgr, byte> imgInput, string filePath)
        {
            // Working Images
            Image<Gray, byte> imgEdges = new Image<Gray, byte>(imgInput.Size);
            Image<Gray, byte> imgDilatedEdges = new Image<Gray, byte>(imgInput.Size);

            // 1. Edge Detection
            CvInvoke.Canny(imgInput, imgEdges, 25, 80);

            // 2. Dilation
            CvInvoke.Dilate(
                imgEdges,
                imgDilatedEdges,
                CvInvoke.GetStructuringElement(
                    ElementShape.Rectangle,
                    new Size(3, 3),
                    new Point(-1, -1)),
                new Point(-1, -1),
                5,
                BorderType.Default,
                new MCvScalar(0));

            // 3. Contours Detection
            VectorOfVectorOfPoint inputContours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(
                imgDilatedEdges,
                inputContours,
                hierarchy,
                RetrType.External,
                ChainApproxMethod.ChainApproxSimple);
            VectorOfPoint primaryContour = (from contour in inputContours.ToList()
                                            orderby contour.GetArea() descending
                                            select contour).FirstOrDefault();
            // 4. Corner Point Extraction
            RotatedRect bounding = CvInvoke.MinAreaRect(primaryContour);
            PointF topLeft = (from point in bounding.GetVertices()
                              orderby Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2))
                              select point).FirstOrDefault();
            PointF topRight = (from point in bounding.GetVertices()
                               orderby Math.Sqrt(Math.Pow(imgInput.Width - point.X, 2) + Math.Pow(point.Y, 2))
                               select point).FirstOrDefault();
            PointF botLeft = (from point in bounding.GetVertices()
                              orderby Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(imgInput.Height - point.Y, 2))
                              select point).FirstOrDefault();
            PointF botRight = (from point in bounding.GetVertices()
                               orderby Math.Sqrt(Math.Pow(imgInput.Width - point.X, 2) + Math.Pow(imgInput.Height - point.Y, 2))
                               select point).FirstOrDefault();
            double boundingWidth = Math.Sqrt(Math.Pow(topRight.X - topLeft.X, 2) + Math.Pow(topRight.Y - topLeft.Y, 2));
            double boundingHeight = Math.Sqrt(Math.Pow(botLeft.X - topLeft.X, 2) + Math.Pow(botLeft.Y - topLeft.Y, 2));
            bool isLandscape = boundingWidth > boundingHeight;

            // 5. Define warp crieria as triangles              
            PointF[] srcTriangle = new PointF[3];
            PointF[] dstTriangle = new PointF[3];
            Rectangle ROI;
            if (isLandscape)
            {
                srcTriangle[0] = botLeft;
                srcTriangle[1] = topLeft;
                srcTriangle[2] = topRight;
                dstTriangle[0] = new PointF(0, (float)boundingHeight);
                dstTriangle[1] = new PointF(0, 0);
                dstTriangle[2] = new PointF((float)boundingWidth, 0);
                ROI = new Rectangle(0, 0, (int)boundingWidth, (int)boundingHeight);
            }
            else
            {
                srcTriangle[0] = topLeft;
                srcTriangle[1] = topRight;
                srcTriangle[2] = botRight;
                dstTriangle[0] = new PointF(0, (float)boundingWidth);
                dstTriangle[1] = new PointF(0, 0);
                dstTriangle[2] = new PointF((float)boundingHeight, 0);
                ROI = new Rectangle(0, 0, (int)boundingHeight, (int)boundingWidth);
            }
            Mat warpMat = new Mat(2, 3, DepthType.Cv32F, 1);
            warpMat = CvInvoke.GetAffineTransform(srcTriangle, dstTriangle);
            // 6. Apply the warp and crop
            CvInvoke.WarpAffine(imgInput, imgInput, warpMat, imgInput.Size);
            imgInput.ROI = ROI;
            imgInput.Save(filePath);
            imgInput.Dispose();
            _logger.LogInformation($"Snapshot saved in {filePath}");
            //imgOutput = imgInput.Copy(ROI);
            //return imgOutput;

        }
    }
}
