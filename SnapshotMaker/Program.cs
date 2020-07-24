using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;
using SnapshotMaker.BL.Services;
using SnapshotMaker.Configuration;

namespace SnapshotMaker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Пытаемся стартануть узел
            try
            {
                Log.Information("Programm start..."); 
                await CreateHostBuilder(args).RunConsoleAsync();

            }
            catch (Exception e)
            {
                Log.Fatal(e, "Необработанное исключение");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                //получаем файл конфигурации
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    builder.AddSuitableConfiguration(args);
                })
                //настраиваем сервисы
                .ConfigureServices((hostContext, services) =>
                    {
                        services.Configure<AppSettings>(hostContext.Configuration.GetSection(nameof(AppSettings)));
                        services.AddHostedService<ConsoleApplication>()
                            .AddScoped<ITakeSnapshotService, TakeSnapshotsService>()
                            .AddScoped<IFrameProcessorService, FrameProcessorService>()
                            .AddScoped<IFrameClassifierService, FrameClassifierService>();
                        services.AddFrameCapturer(hostContext.Configuration);
                        services.AddFrameClassifier(hostContext.Configuration);

                    }
                )
                //Добавляем логгирование, в данном случае Serilog
                .ConfigureLogging((hostContext, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .CreateLogger();

                    logging.AddSerilog();
                });

    }
}