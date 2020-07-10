using System;
using System.Collections.Concurrent;
using Emgu.CV;

namespace SnapshotMaker.BL.Interfaces
{
    public interface IMakeSnapshotService
    {
        void StartCaptureAsync();
        event Action<ConcurrentQueue<Mat>, string> onFrameAdded;
    }
}