using System;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;
using SnapshotMaker.BL.Services;

namespace SnapshotMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //Пытаемся стартануть узел
            try
            {
                Log.Information("Programm start..."); 
                CreateHostBuilder(args).RunConsoleAsync();
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
                    if (args.Any())
                    {
                        Parser.Default.ParseArguments<AppSettings>(args)
                            .WithParsed(parsedSettings => builder.AddParsedCmdLineConfiguration(parsedSettings))
                            .WithNotParsed((errors)=>
                            {
                                Console.WriteLine(string.Join("\n", errors));
                                throw new ArgumentException(string.Join("\n", errors));
                            });
                    }
                    else
                    {
                        builder.AddJsonFile("appsettings.json", optional: true);
                    }
                    builder.AddJsonFile("logger_config.json", false);
                })
                //настраиваем сервисы
                .ConfigureServices((hostContext, services) =>
                    {
                        services.Configure<AppSettings>(hostContext.Configuration.GetSection(nameof(AppSettings)));
                        services.AddHostedService<ConsoleApplication>()
                            .AddScoped(x => SelectCapturerModel(hostContext.Configuration, x))
                            .AddScoped<ITakeSnapshotService, TakeSnapshotsService>();
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

        private static FrameCapturerModel SelectCapturerModel(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            var source = configuration.GetSection(nameof(AppSettings)).GetValue<string>("VideoSource");
            const string filePathPattern = @"^(([a-zA-Z]{1}:|\\)(\\[^\\/<>:\|\*\?\""]+)+\.[^\\/<>:\|]{3,4})$";
            const string rtspUrlPattern = @"^(rtsp?:\/\/)?([\da-z\.-]+\.[a-z\.]{2,6}|[\d\.]+)([\/:?=&#]{1}[\da-z\.-]+)*[\/\?]?$";

            if (Regex.IsMatch(source, filePathPattern, RegexOptions.IgnoreCase))
            {
                return new FrameCapturerFromFile();
            }
            else if (Regex.IsMatch(source, rtspUrlPattern, RegexOptions.IgnoreCase))
            {
                return new FrameCapturerFromCamera(serviceProvider.GetRequiredService<ILogger<FrameCapturerFromCamera>>(),
                    serviceProvider.GetRequiredService<IOptions<AppSettings>>());
            }
            else
            {
                throw new ArgumentException("Не могу определить ресурс", source);
            }
        }
    }
}