using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System.Numerics;

namespace OtusSpaceBattle
{
    internal class Program
    {
        private static ExceptionHandler _handler = new();
        static IUObject CreateTestObject()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IMovableObject.Position), (12, 5));
            gameObject.SetProperty(Constants.Velocity, 10);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 3);
            return gameObject;
        }


        static void Main(string[] args)
        {
            RegisterExceptionHandlers();

            var spaceShip = CreateTestObject();

            var moveCommand = new MoveCommand(new MovingObjectAdapter(spaceShip),spaceShip);

            var rotateCommand = new RotateCommand(new RotatingObjectAdapter(spaceShip),spaceShip);
            var commandCollection = new CommandCollection();
            commandCollection.Add(moveCommand);
            commandCollection.Add(rotateCommand);
            commandCollection.LoopUntilNotEmpty();
        }

        static void RegisterExceptionHandlers()
        {
            ExceptionHandler.RegisterHandler(typeof(MoveCommand), typeof(Exception), (c, e) => { return new LogCommand(e); });
            ExceptionHandler.RegisterHandler(typeof(RotateCommand), typeof(Exception), (c, e) => { return new LogCommand(e); });
        }
    }
}
