using Emgu.CV;

namespace SnapshotMaker.BL.Interfaces
{
    public interface IFrameClassifier
    {
        bool IsSuitableFrame(Mat frame);
    }
}
