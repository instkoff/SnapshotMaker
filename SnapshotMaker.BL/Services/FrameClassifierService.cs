using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Logging;

namespace SnapshotMaker.BL.Services
{
    public class FrameClassifierService : IFrameClassifierService
    {
        private readonly ILogger<FrameClassifierService> _logger;
        private readonly AppSettings _settings;
        private readonly ConcurrentQueue<FrameInfo> _frameQueue;
        private readonly IFrameClassifier _frameClassifier;
        private int _nextFrameIndex;
        private long _prevFramePosition;
        private Image<Bgr, byte> _prevImage;
        private int _noMoveCount;

        public event Action<ConcurrentQueue<FrameInfo>> OnFrameAdded;

        public FrameClassifierService(IOptions<AppSettings> appSettings, IFrameClassifier frameClassifier, ILogger<FrameClassifierService> logger)
        {
            _settings = appSettings.Value;
            _frameClassifier = frameClassifier;
            _logger = logger;
            _frameQueue = new ConcurrentQueue<FrameInfo>();
            _nextFrameIndex = 0;
            _prevFramePosition = -3000;
        }

        public void ClassifyFrame(Mat frame, long framePosition)
        {
            if (!DetectMovement(frame)) 
                return;
            var source = Path.GetFileNameWithoutExtension(_settings.VideoSource);
            var posDiff = framePosition - _prevFramePosition;
            if (_frameClassifier.IsSuitableFrame(frame) && posDiff >= 2500)
            {
                var newFrameInfo = new FrameInfo(frame.Clone(), _nextFrameIndex, framePosition, source, _settings.IsVertical);
                _frameQueue.Enqueue(newFrameInfo);
                OnFrameAdded?.Invoke(_frameQueue);
                _prevFramePosition = framePosition;
                _nextFrameIndex++;
            }
        }

        private bool DetectMovement(Mat frame)
        {
            if (_noMoveCount != 0 &&_noMoveCount % 30 == 0)
            {
                _logger.LogWarning("No movement detect!");
            }
            _prevImage ??= frame.ToImage<Bgr, byte>();
            using var currentImage = frame.ToImage<Bgr, byte>();
            using var diffFrame = currentImage.AbsDiff(_prevImage);
            var moves = diffFrame.CountNonzero()[0];
            _prevImage = frame.ToImage<Bgr, byte>();
            if (moves >= 4400000) return true;
            _noMoveCount++;
            return false;

        }



    }
}
