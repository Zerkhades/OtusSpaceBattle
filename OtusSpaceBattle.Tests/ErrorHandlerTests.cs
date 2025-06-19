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
                setPosition = new Vector2(12, 5),
                setVelocity = new Vector2(-7, 3),
                setFuelTank = 100,                  // Объем бака
                setFuel = 50,                       // Топлива в баке
                setFuelConsumption = 10,            // Топлива сжигается при перемещении
                wantPosition = new Vector2(5, 8),
                wantFuel = 40,
            };

            // Arrange
            var actPosition = Vector2.Zero;
            var actFuel = 0;

            var mockFuelCheckable = new Mock<ICheckFuel>();
            mockFuelCheckable.Setup(move => move.CheckFuel()).Returns(testdata.setFuel >= testdata.setFuelConsumption);

            var mockMovable = new Mock<IMovableObject>();
            mockMovable.Setup(m => m.GetPosition()).Returns(testdata.setPosition);
            mockMovable.Setup(m => m.GetVelocity()).Returns(testdata.setVelocity);
            mockMovable.Setup(m => m.SetPosition(It.IsAny<Vector2>())).Callback<Vector2>((p) => actPosition = p);

            var mockFuelBurnable = new Mock<IFuelableObject>();
            mockFuelBurnable.Setup(m => m.BurnFuel()).Callback(() => actFuel = testdata.setFuel - testdata.setFuelConsumption);

            // Act
            //var command = new MoveAndBurnFuelCommand(mockFuelCheckable.Object, mockMovable.Object, mockFuelBurnable.Object);
            List<ICommand> cmds = new List<ICommand>()
            {
                new CheckFuelCommand(mockFuelCheckable.Object),
                new MoveCommand(mockMovable.Object),
                new BurnFuelCommand(mockFuelBurnable.Object),
            };

            var moveAndBurnFuelCommand = new MacroCommand(cmds);
            moveAndBurnFuelCommand.Execute();

            // Assert
            Assert.Equal(actPosition, testdata.wantPosition);
            Assert.Equal(actFuel, testdata.wantFuel);

        }

        [Fact]
        public void ChangeVelocityCommand_ExecuteTest()
        {
            var testdata = new
            {
                setVelocity = new Vector2(5, 8),
                wantVelocity = new Vector2(5, 8)
            };

            var actVelocity = Vector2.Zero;

            // Arrange
            var mock = new Mock<IChangeVelocity>();
            mock.Setup(m => m.SetVelocity(It.IsAny<Vector2>())).Callback<Vector2>((p) => actVelocity = p);

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
                setVelocity = new Vector2(5, 8),
                wantDirection = 20,
                wantVelocity = new Vector2(5, 8),
            };

            // Arrange
            var actDirection = 0;
            var actVelocity = Vector2.Zero;

            var mockRotable = new Mock<IRotatableObject>();
            mockRotable.Setup(m => m.GetAngularVelocity()).Returns(testdata.setAngularVelocity);
            mockRotable.Setup(m => m.GetDirectionsNumber()).Returns(testdata.setDirectionsNumber);
            mockRotable.Setup(m => m.GetDirection()).Returns(testdata.setDirection);
            mockRotable.Setup(m => m.SetDirection(It.IsAny<int>())).Callback<int>((v) => actDirection = v);

            var mockChangeVelocity = new Mock<IChangeVelocity>();
            mockChangeVelocity.Setup(m => m.SetVelocity(It.IsAny<Vector2>())).Callback<Vector2>((p) => actVelocity = p);

            // Act
            List<ICommand> cmds = new List<ICommand>()
            {
                new RotateCommand(mockRotable.Object),
                new ChangeVelocityCommand(mockChangeVelocity.Object, testdata.setVelocity),
            };

            var rotateAndChangeVelocityCommand = new MacroCommand(cmds);
            rotateAndChangeVelocityCommand.Execute();

            // Assert
            Assert.Equal(actDirection, testdata.wantDirection);
            Assert.Equal(actVelocity, testdata.wantVelocity);
        }
    }
}
