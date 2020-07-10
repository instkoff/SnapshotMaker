using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.BL.Services
{
    public class MakeSnapshotService : IMakeSnapshotService
    {
        private readonly ILogger<MakeSnapshotService> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        private ConcurrentQueue<Mat> _frameQueue;
        private VideoCapture _capture;
        private bool _streamOpened;
        private Timer _timer;
        private Stopwatch _stopwatch;
        private int _frameCount;
        private long _elapsedTime;

        public event Action<ConcurrentQueue<Mat>, string> onFrameAdded;

        public MakeSnapshotService(ILogger<MakeSnapshotService> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
            _elapsedTime = _appSettings.Value.StartDelay;
        }

        public async void StartCaptureAsync()
        {
            _frameQueue = new ConcurrentQueue<Mat>();
            try
            {
                await OpenStream();
                SetTimerCapturing();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                Thread.Sleep(5000);
            }
        }

        private async Task OpenStream()
        {
            if (!_streamOpened)
            {
                _logger.LogInformation("Try opening stream...");
                var openTask = Task.Run(() => _capture = new VideoCapture(_appSettings.Value.VideoSource));
                if (await Task.WhenAny(openTask, Task.Delay(20000)) != openTask)
                {
                    _logger.LogWarning("Unable to open stream");
                }
                else
                {
                    _stopwatch = new Stopwatch();
                    if (_appSettings.Value.Interval > 0)
                    {
                        _stopwatch.Start();
                    }

                    _logger.LogInformation("Stream opened");
                    _streamOpened = true;
                }
            }
        }

        private async void CaptureFrame(object state)
        {
            if (!_streamOpened || _capture == null) return;
            var inputFrame = new Mat();
            _capture.SetCaptureProperty(CapProp.PosMsec, _elapsedTime);
            var captureTask = Task.Run(() => inputFrame = _capture.QueryFrame());
            if (await Task.WhenAny(captureTask, Task.Delay(5000)) != captureTask)
            {
                _logger.LogWarning("Stream connection lost");
                _streamOpened = false;
            }
            else
            {
                if (inputFrame != null && !inputFrame.IsEmpty)
                {
                    _frameQueue.Enqueue(inputFrame.Clone());
                    _frameCount++;
                    _elapsedTime += _appSettings.Value.SnapshotDelay;
                    onFrameAdded?.Invoke(_frameQueue, _appSettings.Value.OutputFolder);
                    if (_frameCount == 10)
                    {
                        GC.Collect();
                        _frameCount = 0;
                    }

                }

                if (_stopwatch.ElapsedMilliseconds >= _appSettings.Value.Interval)
                {
                    _capture.Stop();
                    _capture.Dispose();
                    _streamOpened = false;
                    _timer.Dispose();
                    _logger.LogInformation("Stream stopped.");
                }
            }
        }

        private void SetTimerCapturing()
        {
            _timer = new Timer(CaptureFrame, null, 0, _appSettings.Value.SnapshotDelay);
        }
    }
}
