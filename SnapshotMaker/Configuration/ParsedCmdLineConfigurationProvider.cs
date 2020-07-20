using Microsoft.Extensions.Configuration;

namespace SnapshotMaker.Configuration
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
            Set($"AppSettings:{nameof(Source.Duration)}", Source.Duration.ToString());
            Set($"AppSettings:{nameof(Source.StartPause)}", Source.StartPause.ToString());
            Set($"AppSettings:{nameof(Source.OutputFolder)}", Source.OutputFolder);
            Set($"AppSettings:{nameof(Source.SnapshotDelay)}", Source.SnapshotDelay.ToString());
            Set($"AppSettings:{nameof(Source.SaveOriginal)}", Source.SaveOriginal.ToString());
            Set($"AppSettings:{nameof(Source.IsVertical)}", Source.IsVertical.ToString());
        }
    }
}