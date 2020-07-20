using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SnapshotMaker.BL.Models
{
    public class CvImageInfo
    {
        private Image<Bgr, byte> _image;
        private char _label;
        private string _videoSourceName;
        private int _frameNumber;

        public CvImageInfo(Image<Bgr, byte> image, char label, string videoSourceName, int frameNumber)
        {
            _image = image;
            _label = label;
            _videoSourceName = videoSourceName;
            _frameNumber = frameNumber;
        }

        public string Save(string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            var ingotNumber = _label switch
            {
                'B' => 3 + _frameNumber,
                'M' => 2 + _frameNumber,
                'T' => 1 + _frameNumber,
                'O' => _frameNumber,
                _ => 0
            };
            var snapshotName = ingotNumber + "_" + _label + "_" + _videoSourceName + ".jpg";
            var path = Path.Combine(outputFolder, snapshotName);
            _image.Save(path);
            _image.Dispose();
            return snapshotName;
        }
    }
}
