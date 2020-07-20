using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SnapshotMaker.BL.Interfaces;

namespace SnapshotMaker
{
    public class ConsoleApplication : IHostedService
    {
        private readonly ITakeSnapshotService _takeSnapshotService;

        //Пробрасываем нужные зависимости
        public ConsoleApplication(ITakeSnapshotService takeSnapshotService)
        {
            _takeSnapshotService = takeSnapshotService;
        }
        //Стартуем хост
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _takeSnapshotService.StartTakeSnapshots();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}