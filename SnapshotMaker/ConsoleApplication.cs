using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SnapshotMaker.BL.Interfaces;
using SnapshotMaker.BL.Services;

namespace SnapshotMaker
{
    public class ConsoleApplication : IHostedService
    {
        private readonly IMakeSnapshotService _makeSnapshotService;
        private readonly IFrameProcessorService _frameProcessorService;

        //Пробрасываем нужные зависимости
        public ConsoleApplication(IMakeSnapshotService makeSnapshotService, IFrameProcessorService frameProcessorService)
        {
            _makeSnapshotService = makeSnapshotService;
            _frameProcessorService = frameProcessorService;
        }
        //Стартуем хост
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _makeSnapshotService.StartCaptureAsync();
            _frameProcessorService.StartProcessing();
            Console.ReadKey();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}