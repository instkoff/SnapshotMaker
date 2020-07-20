using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddFrameCapturer(this IServiceCollection services,
            IConfiguration configuration)
        {
            var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            if (appSettings.VideoSource.StartsWith(@"rtsp://"))
            {
                services.AddScoped<FrameCapturerModel, FrameCapturerFromCamera>();
            }
            else
            {
                if(File.Exists(appSettings.VideoSource))
                    services.AddScoped<FrameCapturerModel, FrameCapturerFromFile>();
                else
                {
                    Log.Error("Can't determine the resource type.");
                    throw new FileNotFoundException("File not found.", appSettings.VideoSource);
                }
            }
            return services;
        }
        public static IServiceCollection AddFrameClassifier(this IServiceCollection services,
            IConfiguration configuration)
        {
            var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            if (appSettings.IsVertical)
            {
                services.AddScoped<IFrameClassifier, VerticalFrameClassifier>();
            }
            else
            {
                services.AddScoped<IFrameClassifier, HorizontalFrameClassifier>();
            }
            return services;
        }
    }
}
