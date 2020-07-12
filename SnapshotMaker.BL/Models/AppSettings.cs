using CommandLine;

namespace SnapshotMaker.BL.Models
{
    public class AppSettings
    {
        [Option('s', "source", Required = true, HelpText = "Set the stream source (video file, IP camera stream, etc...)")]
        public string VideoSource { get; set; }
        [Option('o', "output-folder", Required = true, HelpText = "Set output folder")]
        public string OutputFolder { get; set; }
        [Option('p', "start-pause", Required = false, HelpText = "Set pause in ms before stream start capturing")]
        public long StartPause { get; set; }
        [Option('d', "duration", Required = true, HelpText = "Set capturing time in ms")]
        public long Interval { get; set; }
        [Option('w', "wait", Default = 1000, Required = true, HelpText = "Set wait time in ms until the next snapshot")]
        public long SnapshotDelay { get; set; }

    }
}
