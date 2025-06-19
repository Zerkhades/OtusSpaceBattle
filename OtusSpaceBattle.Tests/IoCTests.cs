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
using System.Numerics;

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
            string? resultA = null, resultB = null;
            IoC.Resolve("Scopes.New", "scopeA");
            IoC.Resolve("Scopes.New", "scopeB");
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
            var mockMovable = mockUObject.As<IMovableObject>();
            mockMovable.Setup(x => x.GetPosition()).Returns(new Vector2(12, 5));
            mockMovable.Setup(x => x.SetPosition(Moq.It.IsAny<Vector2>())).Callback<Vector2>(v => { });
            mockUObject.Setup(x => x.GetProperty(nameof(IMovableObject.GetPosition))).Returns(new Vector2(12, 5));
            mockUObject.Setup(x => x.GetProperty(Constants.Velocity)).Returns(10);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetAngularVelocity))).Returns(8);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetDirection))).Returns(3);
            Vector2 actualSetPosition = default;
            mockMovable.Setup(x => x.SetPosition(Moq.It.IsAny<Vector2>())).Callback<Vector2>(v => actualSetPosition = v);

            IoC.Resolve("IoC.Register", "move", new Func<object[], object>(args =>
                new MoveCommand((IMovableObject)args[0])
            ));
            var moveCommand = (ICommand)IoC.Resolve("move", mockUObject.Object);
            moveCommand.Execute();
            Assert.Equal(new Vector2(12,5), actualSetPosition);
        }

        [Fact]
        public void CanRegisterAndResolveRotateCommand()
        {
            var mockUObject = new Moq.Mock<IUObject>();
            var mockRotatable = mockUObject.As<IRotatableObject>();
            mockRotatable.Setup(x => x.GetDirection()).Returns(3);
            mockRotatable.Setup(x => x.GetDirectionsNumber()).Returns(8);
            mockRotatable.Setup(x => x.GetAngularVelocity()).Returns(2);
            int actualDirection = 0;
            mockRotatable.Setup(x => x.SetDirection(Moq.It.IsAny<int>())).Callback<int>(v => actualDirection = v);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetDirection))).Returns(3);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetDirectionsNumber))).Returns(8);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetAngularVelocity))).Returns(2);

            IoC.Resolve("IoC.Register", "rotate", new Func<object[], object>(args =>
                new RotateCommand((IRotatableObject)args[0])
            ));
            var rotateCommand = (ICommand)IoC.Resolve("rotate", mockUObject.Object);
            rotateCommand.Execute();
            Assert.Equal(5, actualDirection);
        }

        [Fact]
        public void CanRegisterAndResolveMacroCommand()
        {
            var mockUObject = new Moq.Mock<IUObject>();
            var mockMovable = mockUObject.As<IMovableObject>();
            var mockRotatable = mockUObject.As<IRotatableObject>();
            mockMovable.Setup(x => x.GetPosition()).Returns(new Vector2(12, 5));
            mockRotatable.Setup(x => x.GetDirectionsNumber()).Returns(8);
            mockRotatable.Setup(x => x.GetAngularVelocity()).Returns(2);
            mockRotatable.Setup(x => x.GetDirection()).Returns(3);
            Vector2 actualSetPosition = default;
            mockMovable.Setup(x => x.SetPosition(Moq.It.IsAny<Vector2>())).Callback<Vector2>(v => actualSetPosition = v);
            int actualDirection = 0;
            mockRotatable.Setup(x => x.SetDirection(Moq.It.IsAny<int>())).Callback<int>(v => actualDirection = v);
            mockUObject.Setup(x => x.GetProperty(nameof(IMovableObject.GetPosition))).Returns(new Vector2(12, 5));
            mockUObject.Setup(x => x.GetProperty(Constants.Velocity)).Returns(10);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetDirectionsNumber))).Returns(8);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetAngularVelocity))).Returns(2);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetDirection))).Returns(3);

            IoC.Resolve("IoC.Register", "move", new Func<object[],object>(args =>
                new MoveCommand((IMovableObject)args[0])
            ));
            IoC.Resolve("IoC.Register", "rotate", new Func<object[], object>(args =>
                new RotateCommand((IRotatableObject)args[0])
            ));
            IoC.Resolve("IoC.Register", "macro", new Func<object[], object>(args =>
                new MacroCommand(new List<ICommand> {
                    (ICommand)IoC.Resolve("move", args),
                    (ICommand)IoC.Resolve("rotate", args)
                })
            ));
            var macroCommand = (ICommand)IoC.Resolve("macro", mockUObject.Object);
            macroCommand.Execute();
            Assert.Equal(new Vector2(12, 5), actualSetPosition);
            Assert.Equal(5, actualDirection);
        }
    }
}
