using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using SnapshotMaker.BL.Interfaces;

namespace SnapshotMaker.BL.Models
{
    public class HorizontalFrameClassifier : IFrameClassifier
    {
        private int _frameCount;
        public bool IsSuitableFrame(Mat frame)
        {
            var mainRect = new Rectangle(580, 660, 50, 500);
            var blackPoints = new List<Point>();
            var imgGray = frame.ToImage<Gray, byte>().ThresholdBinary(new Gray(38), new Gray(255))
                .Dilate(3).Erode(1);
            imgGray.ROI = mainRect;
            for (int i = 0; i < mainRect.Height; i++)
            {
                if (Math.Abs(imgGray[i, 0].Intensity) < 1)
                {
                    blackPoints.Add(new Point(mainRect.X, mainRect.Y + i));
                }
            }
            if (blackPoints.Count > 30)
            {
                var lengthDiffTop = blackPoints[0].Y - mainRect.Y;
                var lengthDiffBottom = mainRect.Bottom - blackPoints[^1].Y;
                if (lengthDiffTop <= 45 && lengthDiffBottom > 400)
                {
                    //DebugMethod(frame, blackPoints, mainRect);
                    //imgGrayDebug.Save($"D:\\temp\\temp\\{_frameCount}gray.jpg");
                    imgGray.Dispose();
                    return true;
                }
            }
            imgGray.Dispose();
            _frameCount++;
            return false;
        }

        private void DebugMethod(Mat frame, List<Point> blackPoints, Rectangle mainRect)
        {
            if (blackPoints.Count == 0) return;
            var rect = new Rectangle(blackPoints[0], new Size(50, blackPoints.Count));
            var temp = frame.ToImage<Bgr, byte>();
            temp.Draw(rect, new Bgr(0, 0, 255), 2);
            temp.Draw(mainRect, new Bgr(255, 0, 0), 2);
            temp.Draw(new LineSegment2D(rect.Location, mainRect.Location), new Bgr(0, 255, 0), 2);
            temp.Draw(new LineSegment2D(new Point(rect.X, rect.Bottom), new Point(mainRect.X, mainRect.Bottom)),
                new Bgr(0, 255, 0), 2);
            temp.Save($"D:\\temp\\{_frameCount}.jpg");
        }
    }
}