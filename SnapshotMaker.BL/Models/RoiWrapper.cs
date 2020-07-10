using System.Drawing;

namespace SnapshotMaker.BL.Models
{
    public class RoiWrapper
    {
        public Rectangle Roi { get; }
        public string Location { get; }

        public RoiWrapper(Rectangle roi, string location)
        {
            Roi = roi;
            Location = location;
        }
    }
}
