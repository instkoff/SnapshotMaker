using System;
using Microsoft.Extensions.Configuration;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.Configuration
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
