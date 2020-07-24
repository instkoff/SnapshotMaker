using Microsoft.Extensions.Configuration;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.Configuration
{
    public class AppSettingConfigurationSource : IConfigurationSource
    {
        public string VideoSource { get; set; }
        public string OutputFolder { get; set; }
        public long StartPause { get; set; }
        public long Duration { get; set; }
        public long SnapshotDelay { get; set; }
        public bool SaveOriginal { get; set; }
        public bool IsVertical { get; set; }
        public bool OneSnapshot { get; set; }

        public AppSettingConfigurationSource(AppSettings settings)
        {
            VideoSource = settings.VideoSource;
            OutputFolder = settings.OutputFolder;
            StartPause = settings.StartPause;
            Duration = settings.Duration;
            SnapshotDelay = settings.SnapshotDelay;
            SaveOriginal = settings.SaveOriginal;
            IsVertical = settings.IsVertical;
            OneSnapshot = settings.OneSnapshot;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ParsedCmdLineConfigurationProvider(this);
        }
    }
}
