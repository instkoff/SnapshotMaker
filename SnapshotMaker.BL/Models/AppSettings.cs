using CommandLine;
using CommandLine.Text;
using Emgu.CV.Ocl;

namespace SnapshotMaker.BL.Models
{
    public sealed class AppSettings
    {
        [Option('s', "source", Required = true, HelpText = "Set the stream source (video file, IP camera stream, etc...)")]
        public string VideoSource { get; set; }
        [Option('o', "output-folder", Required = true, HelpText = "Set output folder")]
        public string OutputFolder { get; set; }
        [Option("start-pause", Required = false, HelpText = "Set pause in ms before stream start capturing")]
        public long StartPause { get; set; }
        [Option("duration",Default = 0, Required = false, HelpText = "Set capturing time in ms (Default: 0)")]
        public long Duration { get; set; }
        [Option('d', "delay", Default = 200, Required = false, HelpText = "Set wait time in ms until the next snapshot (Default: 200)")]
        public long SnapshotDelay { get; set; }
        [Option("save-original", Default = false, Required = false, HelpText = "Save original file")]
        public bool SaveOriginal { get; set; }
        [Option("vertical", Default = false, Required = false, HelpText = "Vertical image or not (Default: false)")]
        public bool IsVertical { get; set; }

    }
}
