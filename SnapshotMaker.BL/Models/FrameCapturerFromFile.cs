using System.Threading.Tasks;
using Emgu.CV.CvEnum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;

namespace SnapshotMaker.BL.Models
{
    public class FrameCapturerFromFile : FrameCapturerModel
    {
        private readonly IFrameClassifierService _frameClassifierService;
        private long _currentDuration;

        public FrameCapturerFromFile(ILogger<FrameCapturerModel> logger, IOptions<AppSettings> appSettings, IFrameClassifierService frameClassifierService)
            : base(logger, appSettings, frameClassifierService)
        {
            _frameClassifierService = frameClassifierService;
        }

        protected override void CaptureFrames()
        {
            Task.Run(TakeFramesFromCamera);
        }

        private void TakeFramesFromCamera()
        {
            _currentDuration = 0;
            NextFramePosition = Settings.StartPause;

            while (Capture.IsOpened)
            {
                Capture.SetCaptureProperty(CapProp.PosMsec, NextFramePosition);
                InputFrame = Capture.QueryFrame();

                if (_currentDuration > Settings.Duration || InputFrame == null)
                {
                    Capture.Dispose();
                    StreamOpened = false;
                    Logger.LogInformation("Stream stopped.");
                    return;
                }

                _frameClassifierService.ClassifyFrame(InputFrame, NextFramePosition);

                if (Settings.Duration > 0)
                {
                    _currentDuration += Settings.SnapshotDelay;
                }

                NextFramePosition += Settings.SnapshotDelay;
            }
        }
    }
}
