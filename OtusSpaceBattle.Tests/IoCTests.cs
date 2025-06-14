using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OtusSpaceBattle.Tests
{
    public class IoCTests
    {
        [Fact]
        public void CanRegisterAndResolveInRootScope()
        {
            IoC.Resolve("IoC.Register", "test.key", new Func<object[], object>(args => 42));
            var result = IoC.Resolve("test.key");
            Assert.Equal(42, result);
        }

        [Fact]
        public void CanRegisterAndResolveWithArgs()
        {
            IoC.Resolve("IoC.Register", "sum", new Func<object[], object>(args => (int)args[0] + (int)args[1]));
            var result = IoC.Resolve("sum", 2, 3);
            Assert.Equal(5, result);
        }

        [Fact]
        public void CanCreateAndSwitchScopes()
        {
            IoC.Resolve("Scopes.New", "scope1");
            IoC.Resolve("Scopes.Current", "scope1");
            IoC.Resolve("IoC.Register", "scoped.key", new Func<object[], object>(args => "scoped"));
            var result = IoC.Resolve("scoped.key");
            Assert.Equal("scoped", result);
            IoC.Resolve("Scopes.Current", "root");
            Assert.Throws<InvalidOperationException>(() => IoC.Resolve("scoped.key"));
        }

        [Fact]
        public void ScopesAreThreadLocal()
        {
            IoC.Resolve("Scopes.New", "threadScope");
            var t = new Thread(() =>
            {
                IoC.Resolve("Scopes.Current", "threadScope");
                IoC.Resolve("IoC.Register", "thread.key", new Func<object[], object>(args => "threaded"));
                var result = IoC.Resolve("thread.key");
                Assert.Equal("threaded", result);
            });
            t.Start();
            t.Join();
            Assert.Throws<InvalidOperationException>(() => IoC.Resolve("thread.key"));
        }

        [Fact]
        public void MultiThreadedScopesWorkIndependently()
        {
            IoC.Resolve("Scopes.New", "scopeA");
            IoC.Resolve("Scopes.New", "scopeB");
            string resultA = null, resultB = null;
            var t1 = new Thread(() =>
            {
                IoC.Resolve("Scopes.Current", "scopeA");
                IoC.Resolve("IoC.Register", "key", new Func<object[], object>(args => "A"));
                resultA = (string)IoC.Resolve("key");
            });
            var t2 = new Thread(() =>
            {
                IoC.Resolve("Scopes.Current", "scopeB");
                IoC.Resolve("IoC.Register", "key", new Func<object[], object>(args => "B"));
                resultB = (string)IoC.Resolve("key");
            });
            t1.Start(); t2.Start();
            t1.Join(); t2.Join();
            Assert.Equal("A", resultA);
            Assert.Equal("B", resultB);
        }

        [Fact]
        public void CanRegisterAndResolveMoveCommand()
        {
            var mockUObject = new Moq.Mock<IUObject>();
            mockUObject.Setup(x => x.GetProperty(nameof(IMovableObject.Position))).Returns((12, 5));
            mockUObject.Setup(x => x.GetProperty(Constants.Velocity)).Returns(10);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(8);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(3);
            (int, int) actualSetPosition = default;
            mockUObject.Setup(x => x.SetProperty(nameof(IMovableObject.Position), Moq.It.IsAny<object>()))
                .Callback<string, object>((name, value) => actualSetPosition = ((int, int))value);

            IoC.Resolve("IoC.Register", "move", new Func<object[], object>(args =>
                new MoveCommand(new MovingObjectAdapter((IUObject)args[0]), (IUObject)args[0])
            ));
            var moveCommand = (ICommand)IoC.Resolve("move", mockUObject.Object);
            moveCommand.Execute();
            Assert.Equal((5, 12), actualSetPosition);
        }

        [Fact]
        public void CanRegisterAndResolveRotateCommand()
        {
            var mockUObject = new Moq.Mock<IUObject>();
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(3);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCountPerStep))).Returns(2);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(8);
            int actualDirection = 0;
            mockUObject.Setup(x => x.SetProperty(nameof(IRotatableObject.Direction), Moq.It.IsAny<object>()))
                .Callback<string, object>((name, value) => actualDirection = (int)value);

            IoC.Resolve("IoC.Register", "rotate", new Func<object[], object>(args =>
                new RotateCommand(new RotatingObjectAdapter((IUObject)args[0]), (IUObject)args[0])
            ));
            var rotateCommand = (ICommand)IoC.Resolve("rotate", mockUObject.Object);
            rotateCommand.Execute();
            Assert.Equal(5, actualDirection);
        }

        [Fact]
        public void CanRegisterAndResolveMacroCommand()
        {
            var mockUObject = new Moq.Mock<IUObject>();
            mockUObject.Setup(x => x.GetProperty(nameof(IMovableObject.Position))).Returns((12, 5));
            mockUObject.Setup(x => x.GetProperty(Constants.Velocity)).Returns(10);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCountPerStep))).Returns(2);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(8);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(3);
            (int, int) actualSetPosition = default;
            mockUObject.Setup(x => x.SetProperty(nameof(IMovableObject.Position), Moq.It.IsAny<object>()))
                .Callback<string, object>((name, value) => actualSetPosition = ((int, int))value);
            int actualDirection = 0;
            mockUObject.Setup(x => x.SetProperty(nameof(IRotatableObject.Direction), Moq.It.IsAny<object>()))
                .Callback<string, object>((name, value) => actualDirection = (int)value);

            IoC.Resolve("IoC.Register", "move", new Func<object[], object>(args =>
                new MoveCommand(new MovingObjectAdapter((IUObject)args[0]), (IUObject)args[0])
            ));
            IoC.Resolve("IoC.Register", "rotate", new Func<object[], object>(args =>
                new RotateCommand(new RotatingObjectAdapter((IUObject)args[0]), (IUObject)args[0])
            ));
            IoC.Resolve("IoC.Register", "macro", new Func<object[], object>(args =>
                new MacroCommand(new List<ICommand> {
                    (ICommand)IoC.Resolve("move", args[0]),
                    (ICommand)IoC.Resolve("rotate", args[0])
                })
            ));
            var macroCommand = (ICommand)IoC.Resolve("macro", mockUObject.Object);
            macroCommand.Execute();
            Assert.Equal((5, 12), actualSetPosition);
            Assert.Equal(5, actualDirection);
        }
    }
}
