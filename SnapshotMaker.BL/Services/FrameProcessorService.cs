using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using SnapshotMaker.BL.Interfaces;

namespace SnapshotMaker.BL.Services
{
    public class FrameProcessorService : IFrameProcessorService
    {
        private readonly IMakeSnapshotService _makeSnapshotService;
        private readonly ILogger<FrameProcessorService> _logger;

        public FrameProcessorService(IMakeSnapshotService makeSnapshotService, ILogger<FrameProcessorService> logger)
        {
            _makeSnapshotService = makeSnapshotService;
            _logger = logger;
        }

        public void StartProcessing()
        {
            _makeSnapshotService.onFrameAdded += (frameQueue, outputFolder) =>
            {
                Task.Run(() => ProcessFrame(frameQueue, outputFolder));
            };
        }

        private void ProcessFrame(ConcurrentQueue<Mat> frameQueue, string outputFolder)
        {
            while (frameQueue.TryDequeue(out var frame))
            {
                var cvImage = frame.ToImage<Bgr, byte>();
                var jpegImageBytes = cvImage.ToJpegData(100);
                var snapshotName = DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";
                var path = Path.Combine(outputFolder, snapshotName);
                using var stream = File.OpenWrite(path);
                stream.Write(jpegImageBytes, 0, jpegImageBytes.Length);
                _logger.LogInformation($"{snapshotName} saved in {path}");
                frame.Dispose();
            }
        }
    }
}
