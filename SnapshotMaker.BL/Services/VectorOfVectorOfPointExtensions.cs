using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace SnapshotMaker.BL.Services
{
    public static class VectorOfVectorOfPointExtensions
    {
        public static List<VectorOfPoint> ToList(this VectorOfVectorOfPoint vectorOfVectorOfPoint)
        {
            List<VectorOfPoint> result = new List<VectorOfPoint>();
            for (int contour = 0; contour < vectorOfVectorOfPoint.Size; contour++)
            {
                result.Add(vectorOfVectorOfPoint[contour]);
            }
            return result;
        }

        public static double GetArea(this VectorOfPoint contour)
        {
            RotatedRect bounding = CvInvoke.MinAreaRect(contour);
            return bounding.Size.Width * bounding.Size.Height;
        }
    }
}
