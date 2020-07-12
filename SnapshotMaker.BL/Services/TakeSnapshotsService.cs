using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.BL.Services
{
    public class TakeSnapshotsService : ITakeSnapshotService
    {
        private readonly FrameCapturerModel _frameCapturer;

        public TakeSnapshotsService(FrameCapturerModel frameCapturer)
        {
            _frameCapturer = frameCapturer;
        }

        public void StartTakeSnapshots()
        {
            _frameCapturer.StartCaptureAsync();
        }
    }
}
