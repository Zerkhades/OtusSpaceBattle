using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class InterpretCommand : ICommand
    {
        private readonly AgentEnvelope _env;
        public InterpretCommand(AgentEnvelope env) => _env = env;

        public void Execute()
        {

            var router = (IGameRouter)IoC.Resolve("router");
            router.TryGet(_env.GameId, out var gameContext);
            // Получить игровой объект через IoC
            var gameObject = IoC.Resolve("GameObjects", _env.TargetId);
            // Получить команду для объекта через IoC
            var command = (ICommand)IoC.Resolve(_env.OpId, gameObject, _env.Args);
            // Получить очередь команд игры через IoC
            
            // Поместить команду в очередь
            gameContext.CommandCollection.Add(command);
        }

        //public ValueTask ExecuteAsync(GameContext ctx, CancellationToken ct = default)
        //{
        //    // Пока просто выводим — потом можно вызывать IoC, игровые объекты и т.д.
        //    Console.WriteLine($"[Game: {_env.GameId}] {_env.OpId} → {_env.TargetId}");
        //    return ValueTask.CompletedTask;
        //}
    }
}
