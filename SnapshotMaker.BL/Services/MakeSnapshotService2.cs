using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.BL.Services
{
    public class MakeSnapshotService2 : IMakeSnapshotService
    {
        private readonly ILogger<MakeSnapshotService> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        private Queue _frameQueue;
        private VideoCapture _capture;
        private Mat _inputFrame;
        private bool _streamOpened;

        public MakeSnapshotService2(ILogger<MakeSnapshotService> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public async void StartCaptureAsync()
        {
            var lastTimeSnapshotSaved = Environment.TickCount - _appSettings.Value.SnapshotDelay;
            _inputFrame = new Mat();
            var q = new Queue();
            _frameQueue = Queue.Synchronized(q);
            try
            {
                if (!_streamOpened)
                {
                    _logger.LogInformation("Opening camera stream...");
                    var openTask = Task.Run(() => _capture = new VideoCapture(_appSettings.Value.VideoSource));
                    if (await Task.WhenAny(openTask, Task.Delay(20000)) != openTask)
                    {
                        _logger.LogWarning("Unable to open camera stream");
                    }
                    else
                    {
                        _logger.LogInformation("Camera stream opened");
                        _streamOpened = true;
                    }
                }
                if (_streamOpened)
                {
                    while (_streamOpened)
                    {
                        var captureTask = Task.Run(() => _inputFrame = _capture.QueryFrame());
                        if (await Task.WhenAny(captureTask, Task.Delay(5000)) != captureTask)
                        {
                            _logger.LogWarning("Camera connection lost");
                            _streamOpened = false;
                        }
                        else
                        {
                            var ticksNow = Environment.TickCount;
                            if (Math.Abs(ticksNow - lastTimeSnapshotSaved) < _appSettings.Value.SnapshotDelay)
                                continue;
                            lastTimeSnapshotSaved = ticksNow;
                            if (_inputFrame != null && !_inputFrame.IsEmpty)
                            {
                                _frameQueue.Enqueue(_inputFrame.Clone());
                            }
                            ProcessFrame();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                Thread.Sleep(5000);
            }
        }

        private void ProcessFrame()
        {
            var frame = (Mat)_frameQueue.Dequeue();
            var cvImage = frame.ToImage<Bgr, byte>();
            var jpegImageBytes = cvImage.ToJpegData(100);
            var snapshotName = DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";
            var path = Path.Combine(_appSettings.Value.OutputFolder, snapshotName);
            using var stream = File.OpenWrite(path);
            stream.Write(jpegImageBytes, 0, jpegImageBytes.Length);
            _logger.LogInformation($"{snapshotName} saved in {_appSettings.Value.OutputFolder}");
        }
    }
}
