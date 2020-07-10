using System.Collections.Concurrent;
using Emgu.CV;

namespace SnapshotMaker.BL.Services
{
    public interface IFrameProcessorService
    {
        void StartProcessing();
    }
}