using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace SnapshotMaker.BL.Services
{
    public class FrameClassifierService : IFrameClassifierService
    {
        private readonly AppSettings _settings;
        private readonly ConcurrentQueue<FrameInfo> _frameQueue;
        private readonly IFrameClassifier _frameClassifier;
        private int _nextFrameIndex;
        private long _prevFramePosition;
        private Image<Bgr, byte> _prevImage;

        public event Action<ConcurrentQueue<FrameInfo>> OnFrameAdded;

        public FrameClassifierService(IOptions<AppSettings> appSettings, IFrameClassifier frameClassifier)
        {
            _settings = appSettings.Value;
            _frameClassifier = frameClassifier;
            _frameQueue = new ConcurrentQueue<FrameInfo>();
            _nextFrameIndex = 0;
            _prevFramePosition = -3000;
        }

        public void ClassifyFrame(Mat frame, long framePosition)
        {
            if (!DetectMovement(frame)) 
                return;
            var sourceFileName = Path.GetFileName(_settings.VideoSource);
            var posDiff = framePosition - _prevFramePosition;
            if (_frameClassifier.IsSuitableFrame(frame) && posDiff >= 2500)
            {
                var newFrameInfo = new FrameInfo(frame.Clone(), _nextFrameIndex, framePosition, sourceFileName, _settings.IsVertical);
                _frameQueue.Enqueue(newFrameInfo);
                OnFrameAdded?.Invoke(_frameQueue);
                _prevFramePosition = framePosition;
                _nextFrameIndex++;
            }
        }

        private bool DetectMovement(Mat frame)
        {
            _prevImage ??= frame.ToImage<Bgr, byte>();
            using var currentImage = frame.ToImage<Bgr, byte>();
            using var diffFrame = currentImage.AbsDiff(_prevImage);
            var moves = diffFrame.CountNonzero()[0];
            _prevImage = frame.ToImage<Bgr, byte>();
            return moves > 4400000;
        }



    }
}
