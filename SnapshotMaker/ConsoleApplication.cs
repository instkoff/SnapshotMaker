using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Models;

namespace SnapshotMaker
{
    public class ConsoleApplication : IHostedService
    {
        private readonly ITakeSnapshotService _takeSnapshotService;
        private readonly IOptions<AppSettings> _options;

        //Пробрасываем нужные зависимости
        public ConsoleApplication(ITakeSnapshotService takeSnapshotService, IOptions<AppSettings> options)
        {
            _takeSnapshotService = takeSnapshotService;
            _options = options;
        }
        //Стартуем хост
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _takeSnapshotService.StartTakeSnapshots();
            Console.ReadKey();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}