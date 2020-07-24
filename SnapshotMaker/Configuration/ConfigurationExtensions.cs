using System;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.Configuration;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker.Configuration
{
    public static class ConfigurationExtensions
    {
        //public static IConfigurationBuilder AddParsedCmdLineConfiguration(this IConfigurationBuilder configuration,
        //   AppSettings settings)
        //{
        //    _ = settings ?? throw new ArgumentNullException(nameof(settings));
        //    configuration.Add(new AppSettingConfigurationSource(settings));
        //    return configuration;
        //}
        public static IConfigurationBuilder AddSuitableConfiguration(this IConfigurationBuilder configuration, string[] args)
        {
            if (args.Any())
            {
                Parser.Default.ParseArguments<AppSettings>(args)
                    .WithParsed(parsedSettings => configuration.Add(new AppSettingConfigurationSource(parsedSettings)))
                    .WithNotParsed((errors) =>
                    {
                        Environment.Exit(1);
                    });
            }
            else
            {
                if (!File.Exists("appsettings.json"))
                {
                    throw new FileNotFoundException("Configuration file not found", "appsettings.json");
                }
                configuration.AddJsonFile("appsettings.json", optional: true);
            }
            configuration.AddJsonFile("logger_config.json", false);
            return configuration;
        }
    }
}
