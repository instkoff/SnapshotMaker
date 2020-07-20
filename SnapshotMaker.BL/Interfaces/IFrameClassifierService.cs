using System;
using System.Collections.Concurrent;
using Emgu.CV;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.BL.Interfaces
{
    public interface IFrameClassifierService
    {
        void ClassifyFrame(Mat frame, long framePosition);
        event Action<ConcurrentQueue<FrameInfo>> OnFrameAdded;
    }
}
