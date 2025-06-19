using System;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Infrastructure;

namespace OtusSpaceBattle.Adapters
{
    public class MovableAdapter
    {
        private readonly IUObject obj;
        public MovableAdapter(IUObject obj)
        {
            this.obj = obj;
        }

        public (int, int) getPosition()
        {
            return IoC.Resolve<(int, int)>("Spaceship.Operations.IMovable:position.get", obj);
        }

        public (int, int) getVelocity()
        {
            return IoC.Resolve<(int, int)>("Spaceship.Operations.IMovable:velocity.get", obj);
        }

        public void setPosition((int, int) newValue)
        {
            IoC.Resolve<ICommand>("Spaceship.Operations.IMovable:position.set", obj, newValue).Execute();
        }
    }
    //public class MovableAdapterTests
    //{
    //    [Fact]
    //    public void getPosition_ReturnsCorrectPosition()
    //    {
    //        Arrange
    //       var obj = new Mock<IUObject>().Object;
    //        var expectedPosition = (10, 20);
    //        IoC.Register<(int, int)>("Spaceship.Operations.IMovable:position.get", args => expectedPosition);

    //        var adapter = new MovableAdapter(obj);

    //        Act
    //       var position = adapter.getPosition();

    //        Assert
    //        Assert.Equal(expectedPosition, position);
    //    }

    //    [Fact]
    //    public void getVelocity_ReturnsCorrectVelocity()
    //    {
    //        Arrange
    //       var obj = new Mock<IUObject>().Object;
    //        var expectedVelocity = (1, 2);
    //        IoC.Register<(int, int)>("Spaceship.Operations.IMovable:velocity.get", args => expectedVelocity);

    //        var adapter = new MovableAdapter(obj);

    //        Act
    //       var velocity = adapter.getVelocity();

    //        Assert
    //        Assert.Equal(expectedVelocity, velocity);
    //    }

    //    [Fact]
    //    public void setPosition_ExecutesCommand()
    //    {
    //        Arrange
    //       var obj = new Mock<IUObject>().Object;
    //        var newPosition = (5, 6);
    //        var commandMock = new Mock<ICommand>();
    //        bool executed = false;
    //        commandMock.Setup(c => c.Execute()).Callback(() => executed = true);
    //        IoC.Register<ICommand>("Spaceship.Operations.IMovable:position.set", args => commandMock.Object);

    //        var adapter = new MovableAdapter(obj);

    //        Act
    //        adapter.setPosition(newPosition);

    //        Assert
    //        Assert.True(executed);
    //    }
    //}
    //public static class MovableAdapterUseCases
    //{
    //    public static void ExampleUsage()
    //    {
    //        Создание объекта IUObject(заглушка)
    //        var obj = new Mock<IUObject>().Object;

    //        Регистрация IoC для получения позиции
    //        IoC.Register<(int, int)>("Spaceship.Operations.IMovable:position.get", args => (100, 200));
    //        Регистрация IoC для получения скорости
    //        IoC.Register<(int, int)>("Spaceship.Operations.IMovable:velocity.get", args => (3, 4));
    //        Регистрация IoC для установки позиции
    //        IoC.Register<ICommand>("Spaceship.Operations.IMovable:position.set", args =>
    //        {
    //            var command = new Mock<ICommand>();
    //            command.Setup(c => c.Execute()).Callback(() =>
    //            {
    //                Console.WriteLine($"Позиция изменена на: {args[1]}");
    //            });
    //            return command.Object;
    //        });

    //        Использование адаптера
    //        var adapter = new MovableAdapter(obj);
    //        var pos = adapter.getPosition();
    //        var vel = adapter.getVelocity();
    //        adapter.setPosition((150, 250));

    //        Console.WriteLine($"Позиция: {pos}");
    //        Console.WriteLine($"Скорость: {vel}");
    //    }
    //}
}
