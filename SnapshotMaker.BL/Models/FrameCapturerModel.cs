using System;
using System.Collections.Concurrent;
using Emgu.CV;
using SnapshotMaker.BL.Interfaces;

namespace SnapshotMaker.BL.Models
{
    public abstract class FrameCapturerModel
    {
        public void StartCaptureAsync()
        {
            OpenVideoSource();
            CaptureFrames();
        }

        public abstract event Action<ConcurrentQueue<Mat>, string> OnFrameAdded;

        protected abstract void OpenVideoSource();
        protected abstract void CaptureFrames();

    }
}
