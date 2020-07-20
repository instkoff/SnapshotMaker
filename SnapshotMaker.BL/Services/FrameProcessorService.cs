using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.BL.Services
{
    public class FrameProcessorService : IFrameProcessorService
    {
        private readonly IFrameClassifierService _frameClassifierService;
        private readonly ILogger<FrameProcessorService> _logger;
        private readonly AppSettings _settings;
        private readonly Dictionary<char, Point> _partPoints;

        public FrameProcessorService(IFrameClassifierService frameClassifierService, ILogger<FrameProcessorService> logger, IOptions<AppSettings> settings)
        {
            _frameClassifierService = frameClassifierService;
            _logger = logger;
            _settings = settings.Value;
            _partPoints = new Dictionary<char, Point>();
        }

        public void StartProcessing()
        {
            if (_settings.IsVertical)
            {
                _partPoints['B'] = new Point(430, 940);
                _partPoints['M'] = new Point(920, 940);
                _partPoints['T'] = new Point(1460, 940);
            }
            else
            {
                _partPoints['T'] = new Point(1225, 325);
                _partPoints['M'] = new Point(1225, 921);
                _partPoints['B'] = new Point(1225, 1500);
            }

            _frameClassifierService.OnFrameAdded += (frameQueue) =>
            {
                Task.Run(() => ProcessFrame(frameQueue));
            };
        }

        private void ProcessFrame(ConcurrentQueue<FrameInfo> frameQueue)
        {
            var sourceFileName = Path.GetFileName(_settings.VideoSource);
            while (frameQueue.TryDequeue(out var frameInfo))
            {
                if (_settings.SaveOriginal)
                {
                    var cvImage = frameInfo.Frame.ToImage<Bgr, byte>();
                    var cvInfoOriginal = new CvImageInfo(cvImage, 'O', sourceFileName, frameInfo.FrameIndex);
                    var savedFileName = cvInfoOriginal.Save(_settings.OutputFolder);
                    _logger.LogInformation($"Original snapshot {savedFileName} saved in {_settings.OutputFolder}");
                }
                DetectIngot(frameInfo);
            }

        }

        private void DetectIngot(FrameInfo frameInfo)
        {
            var cvImage = frameInfo.Frame.ToImage<Bgr, byte>();
            var imgGray = cvImage.Convert<Gray, byte>().ThresholdBinary(new Gray(45), new Gray(255))
                .Dilate(3).Erode(1);
            imgGray.Save($"D:\\temp\\{frameInfo.FrameIndex}_temp_gray.jpg");
            var labels = new Mat();
            CvInvoke.ConnectedComponents(imgGray, labels);
            var cc = labels.ToImage<Gray, byte>();
            foreach (var partPoint in _partPoints)
            {
                var label = (int) cc[partPoint.Value].Intensity;
                if (label == 0) 
                    continue;
                var temp = cc.InRange(new Gray(label), new Gray(label));
                var contours = new VectorOfVectorOfPoint();
                var m = new Mat();
                CvInvoke.FindContours(temp, contours, m, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                if (contours.Size <= 0) 
                    continue;
                var roi = CvInvoke.BoundingRectangle(contours[0]);
                cvImage.ROI = roi;
                var imgOutput = cvImage.Copy();
                if (frameInfo.IsVertical)
                {
                    imgOutput = imgOutput.Rotate(90, new Bgr(0, 0, 0), false);
                }
                var outputImage = new CvImageInfo(imgOutput, partPoint.Key, frameInfo.SourceName, frameInfo.FrameIndex);
                var savedFileName = outputImage.Save(_settings.OutputFolder);
                _logger.LogInformation($"Snapshot {savedFileName} saved in {_settings.OutputFolder}");
                temp.Dispose();
            }
            cvImage.Dispose();
            imgGray.Dispose();
        }
    }
}
