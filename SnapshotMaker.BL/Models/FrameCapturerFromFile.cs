using System;
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
        private readonly long _durationInMsec;

        public FrameCapturerFromFile(ILogger<FrameCapturerModel> logger, IOptions<AppSettings> appSettings, IFrameClassifierService frameClassifierService)
            : base(logger, appSettings, frameClassifierService)
        {
            _frameClassifierService = frameClassifierService;
            _durationInMsec = Settings.Duration * 1000;
        }

        protected override void CaptureFrames()
        {
            Task.Run(TakeFramesFromFile);
            
        }

        private void TakeFramesFromFile()
        {
            _currentDuration = 0;
            NextFramePosition = Settings.StartPause;
            while (Capture.IsOpened)
            {
                Capture.SetCaptureProperty(CapProp.PosMsec, NextFramePosition);
                InputFrame = Capture.QueryFrame();

                if (_currentDuration > _durationInMsec || InputFrame == null)
                {
                    Capture.Dispose();
                    StreamOpened = false;
                    Logger.LogInformation("Stream stopped.");
                    Environment.Exit(0);
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
