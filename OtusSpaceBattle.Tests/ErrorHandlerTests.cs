using Moq;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.IO;
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
    }
}
