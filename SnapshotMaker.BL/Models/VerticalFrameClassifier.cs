using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using SnapshotMaker.BL.Interfaces;

namespace SnapshotMaker.BL.Models
{
    public class VerticalFrameClassifier : IFrameClassifier
    {
        private int _frameCount;
        public bool IsSuitableFrame(Mat frame)
        {
            var mainRect = new Rectangle(250, 1233, 500, 50);
            var blackPoints = new List<Point>();
            var imgGray = frame.ToImage<Gray, byte>().ThresholdBinary(new Gray(30), new Gray(255))
                .Dilate(2).Erode(1);
            imgGray.ROI = mainRect;
            for (int i = 0; i < mainRect.Width; i++)
            {
                if (Math.Abs(imgGray[0, i].Intensity) < 1)
                {
                    blackPoints.Add(new Point(mainRect.X + i, mainRect.Y));
                }
            }
            if (blackPoints.Count > 30)
            {
                var lengthDiffTop = mainRect.Right - blackPoints[^1].X;
                var lengthDiffBottom = blackPoints[0].X - mainRect.X;
                if (lengthDiffTop <= 40 && lengthDiffBottom > 400)
                {
                    //imgGrayDebug.Save($"D:\\temp\\temp\\{_frameCount}gray.jpg");
                    //DebugMethod(frame, blackPoints, mainRect);
                    imgGray.Dispose();
                    return true;
                }
            }

            _frameCount++;
            imgGray.Dispose();
            return false;
        }

        private void DebugMethod(Mat frame, List<Point> blackPoints, Rectangle mainRect)
        {
            if (blackPoints.Count == 0) return;
            var rect = new Rectangle(blackPoints[0], new Size(blackPoints.Count, 50));
            var temp = frame.ToImage<Bgr, byte>();
            temp.Draw(rect, new Bgr(0, 0, 255), 2);
            temp.Draw(mainRect, new Bgr(255, 0, 0), 2);
            temp.Draw(new LineSegment2D(rect.Location, mainRect.Location), new Bgr(0, 255, 0), 2);
            temp.Draw(new LineSegment2D(new Point(rect.Right, rect.Y), new Point(mainRect.Right, mainRect.Y)),
                new Bgr(0, 255, 0), 2);
            temp.Save($"D:\\temp\\{_frameCount}.jpg");
        }
    }
}