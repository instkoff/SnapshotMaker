using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Emgu.CV.CvEnum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;

namespace SnapshotMaker.BL.Models
{
    public class FrameCapturerFromCamera : FrameCapturerModel
    {
        private Stopwatch _stopwatch;
        private readonly long _durationInMsec;

        public FrameCapturerFromCamera(ILogger<FrameCapturerFromCamera> logger, IOptions<AppSettings> appSettings, IFrameClassifierService frameClassifier) 
            : base(logger, appSettings, frameClassifier)
        {
            _durationInMsec = Settings.Duration * 1000;
        }

        protected override void CaptureFrames()
        {
            if (Capture == null)
                return;

            if (Settings.Duration > 0)
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }

            NextFramePosition = Settings.StartPause;
            Logger.LogInformation("Stream opened");
            Capture.ImageGrabbed += CaptureFromCamera;
            Capture.Start();
            StreamOpened = true;
        }

        private void CaptureFromCamera(object sender, EventArgs args)
        {
            if (!StreamOpened || Capture == null)
                return;

            if (_stopwatch != null && _stopwatch.ElapsedMilliseconds >= _durationInMsec)
            {
                Capture.Stop();
                Capture.ImageGrabbed -= CaptureFromCamera;
                Capture.Dispose();
                StreamOpened = false;
                Logger.LogInformation("Stream stopped.");
                Environment.Exit(0);
            }

            var pos = (long) Capture.GetCaptureProperty(CapProp.PosMsec);

            if (pos < NextFramePosition)
                return;

            if (!Capture.Retrieve(InputFrame))
            {
                Logger.LogWarning("Stream connection lost");
                StreamOpened = false;
            }
            else
            {
                if (InputFrame == null || InputFrame.IsEmpty) 
                    return;
                FrameClassifier.ClassifyFrame(InputFrame, NextFramePosition);
                NextFramePosition += Settings.SnapshotDelay;
            }
        }
    }
}
