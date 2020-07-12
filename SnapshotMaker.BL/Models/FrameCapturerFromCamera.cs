using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SnapshotMaker.BL.Models
{
    public class FrameCapturerFromCamera : FrameCapturerModel
    {
        private readonly ILogger<FrameCapturerFromCamera> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ConcurrentQueue<Mat> _frameQueue;
        private VideoCapture _capture;
        private bool _streamOpened;
        private Timer _timer;
        private Stopwatch _stopwatch;
        private Mat _inputFrame = new Mat();

        public override event Action<ConcurrentQueue<Mat>, string> OnFrameAdded;

        public FrameCapturerFromCamera(ILogger<FrameCapturerFromCamera> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
            _frameQueue = new ConcurrentQueue<Mat>();
        }

        protected override async void OpenVideoSource()
        {
            if (_streamOpened) 
                return;
            _logger.LogInformation("Try opening stream...");
            var openTask = Task.Run(() => _capture = new VideoCapture(_appSettings.Value.VideoSource));
            if (await Task.WhenAny(openTask, Task.Delay(20000)) != openTask)
            {
                _logger.LogWarning("Unable to open stream");
            }
            else
            {
                if (_appSettings.Value.Interval > 0)
                {
                    _stopwatch = new Stopwatch();
                    _stopwatch.Start();
                }

                _logger.LogInformation("Stream opened");
                _streamOpened = true;
            }
        }

        protected override void CaptureFrames()
        {
            SetCaptureTimer();
        }

        private async void CaptureFromCamera(object obj)
        {
            if (!_streamOpened || _capture == null)
                return;

            if (_stopwatch != null && _stopwatch.ElapsedMilliseconds >= _appSettings.Value.Interval)
            {
                _capture.Dispose();
                _streamOpened = false;
                _timer.Dispose();
                _logger.LogInformation("Stream stopped.");
                return;
            }

            var captureTask = Task.Run(() => _inputFrame = _capture.QueryFrame());
            if (await Task.WhenAny(captureTask, Task.Delay(5000)) != captureTask)
            {
                _logger.LogWarning("Stream connection lost");
                _streamOpened = false;
            }
            else
            {
                if (_inputFrame != null && !_inputFrame.IsEmpty)
                {
                    _frameQueue.Enqueue(_inputFrame.Clone());
                    OnFrameAdded?.Invoke(_frameQueue, _appSettings.Value.OutputFolder);
                }

            }
        }

        private void SetCaptureTimer()
        {
            _timer = new Timer(CaptureFromCamera, null, 0, _appSettings.Value.SnapshotDelay);
        }

   }
}
