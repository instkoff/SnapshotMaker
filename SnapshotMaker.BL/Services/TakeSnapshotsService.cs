using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.BL.Services
{
    public class TakeSnapshotsService : ITakeSnapshotService
    {
        private readonly FrameCapturerModel _frameCapturer;
        private readonly IFrameProcessorService _frameProcessor;

        public TakeSnapshotsService(FrameCapturerModel frameCapturer, IFrameProcessorService frameProcessor)
        {
            _frameCapturer = frameCapturer;
            _frameProcessor = frameProcessor;
        }

        public void StartTakeSnapshots()
        {
            _frameCapturer.StartCaptureAsync();
            _frameProcessor.StartProcessing();
        }
    }
}
