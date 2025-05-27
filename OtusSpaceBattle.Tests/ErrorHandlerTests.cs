using Moq;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.IO;
using System.Numerics;
using Xunit;

namespace OtusSpaceBattle.Tests
{
    public class ExceptionHandlerTests
    {
        [Fact]
        public void LogThrownException()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            var mock = new Mock<ICommand>(MockBehavior.Strict);
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException")).Verifiable();

            commandCollection.Add(mock.Object);

            ExceptionHandler.RegisterHandler(
                mock.Object.GetType(),
                typeof(IOException),
                (c, e) => { return new LogCommand(e); });

            // Act
            commandCollection.LoopUntilNotEmpty();

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(1));
        }

        [Fact]
        public void RetryWithDelayCommand()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            var mock = new Mock<ICommand>(MockBehavior.Strict);
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException")).Verifiable();

            commandCollection.Add(mock.Object);

            ExceptionHandler.RegisterHandler(
                mock.Object.GetType(),
                typeof(IOException),
                (c, e) => { return new RetryWithDelayCommand(commandCollection, new LogCommand(e)); });

            // Act
            commandCollection.LoopUntilNotEmpty();

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(1));
        }

        [Fact]
        public void RetryWithDelayCommandThrowExceptions()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            var mock = new Mock<ICommand>(MockBehavior.Strict);
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException")).Verifiable();

            commandCollection.Add(mock.Object);

            ExceptionHandler.RegisterHandler(
                mock.Object.GetType(),
                typeof(IOException),
                (c, e) => { return new RetryWithDelayCommand(commandCollection, c); });

            // Act
            commandCollection.LoopPerCount(5);

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(5));
        }

        [Fact]
        public void RetryWithDelayNowCommandThrowExceptions()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            var mock = new Mock<ICommand>(MockBehavior.Strict);
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException")).Verifiable();

            commandCollection.Add(mock.Object);

            ExceptionHandler.RegisterHandler(
                mock.Object.GetType(),
                typeof(IOException),
                (c, e) => { return new RetryWithDelayNowCommand(commandCollection, c); });

            // Act
            commandCollection.LoopPerCount(10);

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(5));
        }

        [Fact]
        public void FirstRetryThenThrowExceptions()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            var mock = new Mock<ICommand>(MockBehavior.Strict);
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException")).Verifiable();

            commandCollection.Add(mock.Object);

            ExceptionHandler.RegisterHandler(mock.Object.GetType(), typeof(IOException), (c, e) => { return new RetryWithDelayCommand(commandCollection, new FirstRetryCommand(c)); });
            ExceptionHandler.RegisterHandler(typeof(FirstRetryCommand), typeof(IOException), (c, e) => { return new LogCommand(e); });

            // Act
            commandCollection.LoopUntilNotEmpty();

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(2));
        }

        [Fact]
        public void SecondRetryThenThrowExceptions()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            var mock = new Mock<ICommand>(MockBehavior.Strict);
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException")).Verifiable();

            commandCollection.Add(mock.Object);

            ExceptionHandler.RegisterHandler(mock.Object.GetType(), typeof(IOException), (c, e) => { return new RetryWithDelayCommand(commandCollection, new FirstRetryCommand(c)); });
            ExceptionHandler.RegisterHandler(typeof(FirstRetryCommand), typeof(IOException), (c, e) => { return new RetryWithDelayCommand(commandCollection, new SecondRetryCommand(c)); });
            ExceptionHandler.RegisterHandler(typeof(SecondRetryCommand), typeof(IOException), (c, e) => { return new LogCommand(e); });

            // Act
            commandCollection.LoopUntilNotEmpty();

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(3));
        }
        [Fact]
        public void CheckFuelCommand_CheckTest()
        {
            var testdata = new
            {
                CheckFuelLevel = false
            };

            // Arrange
            var mock = new Mock<ICheckFuel>();
            mock.Setup(m => m.CheckFuel()).Returns(testdata.CheckFuelLevel);

            // Act
            var command = new CheckFuelCommand(mock.Object);

            // Assert
            Assert.Throws<CommandException>(() => command.Execute());
        }

        [Fact]
        public void BurnFuelCommand_BurnTest()
        {
            // Arrange
            var mock = new Mock<IFuelableObject>();
            mock.Setup(m => m.BurnFuel());

            // Act
            var command = new BurnFuelCommand(mock.Object);
            command.Execute();

            // Assert
            mock.Verify(c => c.BurnFuel(), Times.Exactly(1));
        }

        [Fact]
        public void MoveAndBurnFuelCommand_ExecuteTest()
        {
            var testdata = new
            {
                setPosition = new ValueTuple<int, int>(12, 5),
                setVelocity = new ValueTuple<int, int>(-7, 3),
                setFuelTank = 100,                  // Объем бака
                setFuel = 50,                       // Топлива в баке
                setFuelConsumption = 10,            // Топлива сжигается при перемещении
                wantPosition = new ValueTuple<int, int>(5, 8),
                wantFuel = 40,
            };

            // Arrange
            var actPosition = new ValueTuple<int, int>(0, 0);
            var actFuel = 0;

            var mockFuelCheckable = new Mock<ICheckFuel>();
            mockFuelCheckable.Setup(move => move.CheckFuel()).Returns(testdata.setFuel >= testdata.setFuelConsumption);

            var mockMovable = new Mock<IMovableObject>();
            mockMovable.SetupProperty(m => m.Position, testdata.setPosition);
            mockMovable.Setup(m => m.Velocity).Returns(testdata.setVelocity);
            mockMovable.SetupSet(m => m.Position = It.IsAny<ValueTuple<int, int>>())
                .Callback<ValueTuple<int, int>>(p => actPosition = p);

            var mockFuelBurnable = new Mock<IFuelableObject>();
            mockFuelBurnable.Setup(m => m.BurnFuel()).Callback(() => actFuel = testdata.setFuel - testdata.setFuelConsumption);

            var mockUObject = new Mock<IUObject>();
            // Setup SetProperty to update actPosition
            mockUObject.Setup(x => x.SetProperty("Position", It.IsAny<object>()))
                .Callback<string, object>((name, value) => actPosition = ((ValueTuple<int, int>)value));

            // Act
            List<ICommand> cmds = new List<ICommand>()
            {
                new CheckFuelCommand(mockFuelCheckable.Object),
                new MoveCommand(mockMovable.Object, mockUObject.Object),
                new BurnFuelCommand(mockFuelBurnable.Object),
            };

            var moveAndBurnFuelCommand = new MacroCommand(cmds);
            moveAndBurnFuelCommand.Execute();

            // Assert
            Assert.Equal(testdata.wantPosition, actPosition);
            Assert.Equal(testdata.wantFuel, actFuel);
        }

        [Fact]
        public void ChangeVelocityCommand_ExecuteTest()
        {
            var testdata = new
            {
                setVelocity = 5,
                wantVelocity = 5
            };

            var actVelocity = 0;

            // Arrange
            var mock = new Mock<IChangeVelocity>();
            mock.Setup(m => m.SetVelocity(It.IsAny<int>())).Callback<int>((v) => actVelocity = v);

            // Act
            var command = new ChangeVelocityCommand(mock.Object, testdata.setVelocity);
            command.Execute();

            // Assert
            Assert.Equal(testdata.wantVelocity, actVelocity);
        }

        [Fact]
        public void RotateAndChangeVelocityCommand_ExecuteTest()
        {
            var testdata = new
            {
                setAngularVelocity = 100,
                setDirectionsNumber = 90,
                setDirection = 100,
                setVelocity = 5,
                wantDirection = 20,
                wantVelocity = 5,
            };

            // Arrange
            var actDirection = 0;
            var actVelocity = 0;

            var mockRotable = new Mock<IRotatableObject>();
            mockRotable.SetupProperty(m => m.Direction, testdata.setDirection);
            mockRotable.SetupGet(m => m.DirectionsCountPerStep).Returns(testdata.setAngularVelocity);
            mockRotable.SetupGet(m => m.DirectionsCount).Returns(testdata.setDirectionsNumber);
            mockRotable.SetupSet(m => m.Direction = It.IsAny<int>()).Callback<int>(v => actDirection = v);

            var mockChangeVelocity = new Mock<IChangeVelocity>();
            mockChangeVelocity.Setup(m => m.SetVelocity(It.IsAny<int>())).Callback<int>(v => actVelocity = v);

            var mockUObject = new Mock<IUObject>();
            mockUObject.Setup(x => x.SetProperty("Direction", It.IsAny<object>()))
                .Callback<string, object>((name, value) => actDirection = (int)value);

            mockUObject.Setup(x => x.GetProperty("Direction")).Returns(() => actDirection == 0 ? testdata.setDirection : actDirection);
            mockUObject.Setup(x => x.GetProperty("DirectionsCountPerStep")).Returns(testdata.setAngularVelocity);
            mockUObject.Setup(x => x.GetProperty("DirectionsCount")).Returns(testdata.setDirectionsNumber);

            // Act
            List<ICommand> cmds = new List<ICommand>()
            {
                new RotateCommand(mockRotable.Object, mockUObject.Object),
                new ChangeVelocityCommand(mockChangeVelocity.Object, testdata.setVelocity),
            };

            var rotateAndChangeVelocityCommand = new MacroCommand(cmds);
            rotateAndChangeVelocityCommand.Execute();

            // Assert
            Assert.Equal(testdata.wantDirection, actDirection);
            Assert.Equal(testdata.wantVelocity, actVelocity);
        }
    }
}
