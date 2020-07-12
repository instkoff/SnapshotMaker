using System;
using System.Collections.Concurrent;
using Emgu.CV;

namespace SnapshotMaker.BL.Models
{
    public class FrameCapturerFromFile : FrameCapturerModel
    {
        public override event Action<ConcurrentQueue<Mat>, string> OnFrameAdded;
        protected override void OpenVideoSource()
        {
            throw new NotImplementedException();
        }

        protected override void CaptureFrames()
        {
            throw new NotImplementedException();
        }
    }
}
