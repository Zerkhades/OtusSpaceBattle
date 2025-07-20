using Confluent.Kafka;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Services
{
    public class GameCommandConsumer : ICommandAsync
    {
        private readonly IGameRouter _router;

        public GameCommandConsumer(IGameRouter router)
            => _router = router;

        async Task ICommandAsync.ExecuteAsync(CancellationToken ct)
        {
            var cfg = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "game-server",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(cfg).Build();
            consumer.Subscribe("game.commands");

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    // ждём максимум 500 мс, потом возвращаемся в цикл
                    var cr = consumer.Consume(TimeSpan.FromMilliseconds(500));
                    if (cr is null)             // нет сообщений
                        continue;

                    if (string.IsNullOrWhiteSpace(cr.Message?.Value))
                        continue;

                    var env = JsonSerializer.Deserialize<AgentEnvelope>(cr.Message.Value);
                    if (env == null) continue;

                    if (_router.TryGet(env.GameId, out var game))
                    {
                        Console.WriteLine($"Adding command to queue for game: {env.GameId}");
                        new InterpretCommand(env).Execute();
                        //game.CommandCollection.Add(new InterpretCommand(env));
                    }

                    consumer.Commit(cr);
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Kafka error: {ex.Error.Reason}");
                    continue;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation canceled.");
                    break;
                }
            }

            Console.WriteLine("Exiting consumer loop.");
        }

    }
}
