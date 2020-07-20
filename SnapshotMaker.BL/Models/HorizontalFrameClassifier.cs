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
        public bool IsSuitableFrame(Mat frame)
        {
            var mainRect = new Rectangle(560, 640, 50, 620);
            var blackPoints = new List<Point>();
            var imgGray = frame.ToImage<Gray, byte>().ThresholdBinary(new Gray(30), new Gray(255))
                .Dilate(3).Erode(1);
            //imgGray.Save($"D:\\temp\\{_nextFrameIndex}gray.jpg");
            imgGray.ROI = mainRect;
            for (int i = 0; i < mainRect.Height; i++)
            {
                if (Math.Abs(imgGray[i, 0].Intensity) < 1)
                {
                    blackPoints.Add(new Point(mainRect.X, mainRect.Y + i));
                }
            }

            if (blackPoints.Count > 45)
            {
                var lengthDiffTop = blackPoints[0].Y - mainRect.Y;
                var lengthDiffBottom = mainRect.Bottom - blackPoints[^1].Y;
                if (lengthDiffTop <= 50 && lengthDiffBottom > 500)
                {
                    //var rect = new Rectangle(blackPoints[0], new Size(50, blackPoints.Count));
                    //var temp = frame.ToImage<Bgr, byte>();
                    //temp.Draw(rect, new Bgr(0, 0, 255), 2);
                    //temp.Draw(mainRect, new Bgr(255, 0, 0), 2);
                    //temp.Draw(new LineSegment2D(rect.Location, mainRect.Location), new Bgr(0, 255, 0), 2);
                    //temp.Draw(new LineSegment2D(new Point(rect.X, rect.Bottom), new Point(mainRect.X, mainRect.Bottom)), new Bgr(0, 255, 0), 2);
                    //temp.Save($"D:\\temp\\{_nextFrameIndex}.jpg");
                    return true;
                }
            }
            imgGray.Dispose();
            return false;
        }
    }
}