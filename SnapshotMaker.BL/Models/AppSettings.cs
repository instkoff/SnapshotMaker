using CommandLine;

namespace SnapshotMaker.BL.Models
{
    public class AppSettings
    {
        [Option('s', "source", Required = true, HelpText = "Set the stream source (video file, IP camera stream, etc...)")]
        public string VideoSource { get; set; }
        [Option('o', "output-folder", Required = true, HelpText = "Set output folder")]
        public string OutputFolder { get; set; }
        [Option('d', "start-delay", Required = false, HelpText = "Set the delay before stream start capturing")]
        public long StartDelay { get; set; }
        [Option('i', "interval", Required = false, HelpText = "Set the capturing time in ms")]
        public long Interval { get; set; }
        [Option('p', "snapshot-delay", Default = 1000, Required = true, HelpText = "Set pause between snapshots")]
        public long SnapshotDelay { get; set; }

    }
}
