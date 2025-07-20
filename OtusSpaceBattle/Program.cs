using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using OtusSpaceBattle.Services;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;

namespace OtusSpaceBattle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hemlo wormld! <3");

            var router = IoC.Resolve<object>("IoC.Register", "router", new Func<object[], object>(args =>
            {
                return new InMemoryRouter();
            }));

            var consumer = IoC.Resolve<object>("IoC.Register", "consume", new Func<object[], object>(args =>
            {
                return new GameCommandConsumer((IGameRouter)IoC.Resolve<object>("router"));
            }));

            IoC.Resolve("IoC.Register", "GameObjects", new Func<object[], object>(args =>
            {
                return new Spaceship();
            }));

            IoC.Resolve("IoC.Register", "move", new Func<object[], object>(args =>
                new MoveCommand((IMovableObject)args[0])));

            var gcc = (ICommandAsync)IoC.Resolve("consume");
            gcc.ExecuteAsync();


            //var moveCommand = (ICommand)IoC.Resolve("move", mockUObject.Object);
            //IoC.Resolve<IoCInitializer>().Initialize();

            //// Генерация и сохранение адаптеров для всех интерфейсов
            //var outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedAdapters");
            //AdapterGenerator.GenerateAndSaveAllAdapters(outputDir, Assembly.GetExecutingAssembly());
            //Console.WriteLine($"Adapters generated and saved to {outputDir}");
        }
    }
}
