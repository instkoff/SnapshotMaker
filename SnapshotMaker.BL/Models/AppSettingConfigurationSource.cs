using Microsoft.Extensions.Configuration;

namespace SnapshotMaker.BL.Models
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
            StartDelay = settings.StartDelay;
            Interval = settings.Interval;
            SnapshotDelay = settings.SnapshotDelay;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ParsedCmdLineConfigurationProvider(this);
        }
    }
}
