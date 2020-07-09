using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace SnapshotMaker.BL.Models
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddParsedCmdLineConfiguration(this IConfigurationBuilder configuration,
           AppSettings settings)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            configuration.Add(new AppSettingConfigurationSource(settings));
            return configuration;
        }
    }
}
