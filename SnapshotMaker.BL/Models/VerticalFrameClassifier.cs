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
        public bool IsSuitableFrame(Mat frame)
        {
            var mainRect = new Rectangle(1150, 1233, 560, 50);
            var blackPoints = new List<Point>();
            var imgGray = frame.ToImage<Gray, byte>().ThresholdBinary(new Gray(30), new Gray(255))
                .Dilate(2).Erode(1);
            //imgGray.Save($"D:\\temp\\{_nextFrameIndex}gray.jpg");
            imgGray.ROI = mainRect;
            for (int i = 0; i < mainRect.Width; i++)
            {
                if (Math.Abs(imgGray[0, i].Intensity) < 255)
                {
                    blackPoints.Add(new Point(mainRect.X + i, mainRect.Y));
                }
            }

            if (blackPoints.Count > 40)
            {
                var lengthDiffTop = blackPoints[0].X - mainRect.X;
                var lengthDiffBottom = mainRect.Right - blackPoints[^1].X;
                if (lengthDiffTop <= 40 && lengthDiffBottom > 450)
                {
                    //var rect = new Rectangle(blackPoints[0], new Size(blackPoints.Count, 50));
                    //var temp = frame.ToImage<Bgr, byte>();
                    //temp.Draw(rect, new Bgr(0, 0, 255), 2);
                    //temp.Draw(mainRect, new Bgr(255, 0, 0), 2);
                    //temp.Draw(new LineSegment2D(rect.Location, mainRect.Location), new Bgr(0, 255, 0), 2);
                    //temp.Draw(new LineSegment2D(new Point(rect.Right, rect.Y), new Point(mainRect.Right, mainRect.Y)), new Bgr(0, 255, 0), 2);
                    //temp.Save($"D:\\temp\\{_nextFrameIndex}.jpg");
                    imgGray.Dispose();
                    return true;
                }
            }
            imgGray.Dispose();
            return false;
        }
    }
}