using Emgu.CV;

namespace SnapshotMaker.BL.Models
{
    public class FrameInfo
    {
        public Mat Frame { get; }
        public int FrameIndex { get; }
        public long FramePosition { get; }
        public string SourceName { get; }
        public bool IsVertical { get; }

        public FrameInfo(Mat frame, int frameIndex, long framePosition, string sourceName, bool isVertical = false)
        {
            Frame = frame;
            FrameIndex = frameIndex;
            FramePosition = framePosition;
            SourceName = sourceName;
            IsVertical = isVertical;
        }
    }
}
