using Microsoft.Extensions.Configuration;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker
{
    public class AppSettingConfigurationSource : IConfigurationSource
    {
        public string VideoSource { get; set; }
        public string OutputFolder { get; set; }
        public long StartDelay { get; set; }
        public long Interval { get; set; }
        public long SnapshotDelay { get; set; }

        public AppSettingConfigurationSource(AppSettings settings)
        {
            VideoSource = settings.VideoSource;
            OutputFolder = settings.OutputFolder;
            StartDelay = settings.StartPause;
            Interval = settings.Interval;
            SnapshotDelay = settings.SnapshotDelay;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ParsedCmdLineConfigurationProvider(this);
        }
    }
}
