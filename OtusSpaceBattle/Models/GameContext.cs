using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Models
{
    public interface IGameRouter
    {
        bool TryGet(string id, out GameContext ctx);
    }
    public sealed class InMemoryRouter : IGameRouter
    {
        private readonly ConcurrentDictionary<string, GameContext> _map = new()
        {
            ["game1"] = new GameContext(),
            ["game2"] = new GameContext(),
            //["string"] = new GameContext(),
        };
        public bool TryGet(string gameId, out GameContext ctx)
        {
            ctx = _map.GetOrAdd(gameId, id =>
            {
                var g = new GameContext();
                // Запускаем цикл в фоне и сохраняем Task, чтобы можно было await/Stop
                //g.Start();
                return g;
            });
            return true;
        }
    }

    public sealed class GameContext
    {

        public CommandCollection CommandCollection { get; } =
            new CommandCollection();

        private readonly CancellationTokenSource _cts = new();
        private Task? _loopTask;

        /// <summary> Запускает игровой цикл; вызывает один раз. </summary>
        //public void Start()
        //{
        //    // Task.Run, чтобы цикл не блокировал вызывающий поток
        //    CommandCollection.Add(new StartProcessingCommandCollectionCommand());
        //}

        /// <summary> Корректно останавливает игру и ждёт завершения цикла. </summary>
        public async Task StopAsync()
        {
            _cts.Cancel();
            if (_loopTask is not null)
                await _loopTask.ConfigureAwait(false);
        }

        private async Task RunAsync(CancellationToken ct)
        {
            try
            {
                //await foreach (var cmd in CommandQueue.Reader.ReadAllAsync(ct))
                //    cmd.Execute();

                CommandCollection.BackgroundLoop();
            }
            catch (OperationCanceledException) { /* нормальный выход */ }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameContext] fatal: {ex}");
                // TODO: логирование, аварийное завершение боя
            }
        }
    }
}
