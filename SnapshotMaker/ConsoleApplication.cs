using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker
{
    public class ConsoleApplication : IHostedService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IMakeSnapshotService _makeSnapshotService;

        //Пробрасываем нужные зависимости
        public ConsoleApplication(IOptions<AppSettings> appSettings, IMakeSnapshotService makeSnapshotService)
        {
            _appSettings = appSettings;
            _makeSnapshotService = makeSnapshotService;
        }
        //Стартуем хост
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _makeSnapshotService.StartCaptureAsync();
            Console.ReadKey();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}