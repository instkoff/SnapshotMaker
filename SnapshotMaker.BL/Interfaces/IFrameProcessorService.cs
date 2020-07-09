using System.Collections.Concurrent;
using Emgu.CV;

namespace SnapshotMaker.BL.Services
{
    public interface IFrameProcessorService
    {
        bool ProcessFrame(ConcurrentQueue<Mat> frameQueue, string outputFolder);
    }
}