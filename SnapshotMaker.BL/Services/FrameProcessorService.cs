using System;
using System.Collections.Concurrent;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;

namespace SnapshotMaker.BL.Services
{
    public class FrameProcessorService : IFrameProcessorService
    {
        public bool ProcessFrame(ConcurrentQueue<Mat> frameQueue, string outputFolder)
        {
            var result = frameQueue.TryDequeue(out var frame);
            if (!result)
                return false;
            var cvImage = frame.ToImage<Bgr, byte>();
            var jpegImageBytes = cvImage.ToJpegData(100);
            var snapshotName = DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";
            var path = Path.Combine(outputFolder, snapshotName);
            using var stream = File.OpenWrite(path);
            stream.Write(jpegImageBytes, 0, jpegImageBytes.Length);
            return true;
        }
    }
}
