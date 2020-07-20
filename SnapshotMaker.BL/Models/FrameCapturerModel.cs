using System;
using Emgu.CV;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;

namespace SnapshotMaker.BL.Models
{
    public abstract class FrameCapturerModel
    {
        protected readonly IFrameClassifierService FrameClassifier;
        protected readonly ILogger<FrameCapturerModel> Logger;
        protected readonly AppSettings Settings;
        protected VideoCapture Capture;
        protected bool StreamOpened; 
        protected Mat InputFrame;
        protected long NextFramePosition;

        protected FrameCapturerModel(ILogger<FrameCapturerModel> logger, IOptions<AppSettings> appSettings, IFrameClassifierService frameClassifier)
        {
            Logger = logger;
            FrameClassifier = frameClassifier;
            InputFrame = new Mat();
            Settings = appSettings.Value;
        }


        public void StartCaptureAsync()
        {
            OpenVideoSource();
            CaptureFrames();
        }

        protected virtual void OpenVideoSource()
        {
            if (StreamOpened)
                return;
            try
            {
                Logger.LogInformation("Try opening stream...");
                Capture = new VideoCapture(Settings.VideoSource);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.InnerException);
                throw new ApplicationException("Unable to open stream.", e);
            }
        }

        protected abstract void CaptureFrames();
    }
}
