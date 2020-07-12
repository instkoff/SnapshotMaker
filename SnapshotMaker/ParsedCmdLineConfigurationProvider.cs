using Microsoft.Extensions.Configuration;

namespace SnapshotMaker
{
    public class ParsedCmdLineConfigurationProvider : ConfigurationProvider
    {
        public AppSettingConfigurationSource Source { get; }

        public ParsedCmdLineConfigurationProvider(AppSettingConfigurationSource source)
        {
            Source = source;
        }

        public override void Load()
        {
            Set($"AppSettings:{nameof(Source.VideoSource)}", Source.VideoSource);
            Set($"AppSettings:{nameof(Source.Interval)}", Source.Interval.ToString());
            Set($"AppSettings:{nameof(Source.StartDelay)}", Source.StartDelay.ToString());
            Set($"AppSettings:{nameof(Source.OutputFolder)}", Source.OutputFolder);
            Set($"AppSettings:{nameof(Source.SnapshotDelay)}", Source.SnapshotDelay.ToString());
        }
    }
}