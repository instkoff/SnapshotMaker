using Emgu.CV;
using Emgu.CV.Structure;

namespace SnapshotMaker.BL.Models
{
    public class CvImageWrapper
    {
        public Image<Bgr, byte> Image { get; }
        public string Location { get; }

        public CvImageWrapper(Image<Bgr, byte> image, string position)
        {
            Image = image;
            Location = position;
        }
    }
}
