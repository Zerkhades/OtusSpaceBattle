using Moq;
using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace OtusSpaceBattle.Tests
{
    public class MoveObjectTests
    {
        public MoveObjectTests()
        {
        }

        [Fact]
        public void CorrectlyMoveGameObject()
        {
            // Arrange
            var mockUObject = new Mock<IUObject>();
            var position = (12, 5);
            var direction = 3;
            var directionsCount = 8;
            var speed = 10;
            
            mockUObject.Setup(x => x.GetProperty(nameof(IMovableObject.Position))).Returns(position);
            mockUObject.Setup(x => x.GetProperty(nameof(Constants.Velocity))).Returns(speed);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(directionsCount);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(direction);
            
            (int, int) actualSetPosition = default;
            mockUObject.Setup(x => x.SetProperty(nameof(IMovableObject.Position), It.IsAny<object>()))
                .Callback<string, object>((name, value) => actualSetPosition = ((int, int))value);

            ICommand moveCommand = new MoveCommand(new MovingObjectAdapter(mockUObject.Object), mockUObject.Object);
            // Act
            moveCommand.Execute();

            // Assert
            var expectedPosition = (5, 12);
            Assert.Equal(expectedPosition, actualSetPosition);
        }

        [Theory]
        [InlineData(nameof(IMovableObject.Position))]
        [InlineData(nameof(Constants.Velocity))]
        public void MissingRequiredPropertyThrowsException(string missingProperty)
        {
            // Arrange
            var mockUObject = new Mock<IUObject>();
            var position = (12, 5);
            var direction = 3;
            var directionsCount = 8;
            var speed = 10;

            if (missingProperty != nameof(IMovableObject.Position))
                mockUObject.Setup(x => x.GetProperty(nameof(IMovableObject.Position))).Returns(position);
            else
                mockUObject.Setup(x => x.GetProperty(nameof(IMovableObject.Position))).Throws<KeyNotFoundException>();

            if (missingProperty != nameof(Constants.Velocity))
                mockUObject.Setup(x => x.GetProperty(nameof(Constants.Velocity))).Returns(speed);
            else
                mockUObject.Setup(x => x.GetProperty(nameof(Constants.Velocity))).Throws<KeyNotFoundException>();

            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(directionsCount);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(direction);

            ICommand moveCommand = new MoveCommand(new MovingObjectAdapter(mockUObject.Object), mockUObject.Object);

            // Act & assert
            Assert.Throws<KeyNotFoundException>(moveCommand.Execute);
        }

        [Fact]
        public void InvalidDirectionsCountThrowsException()
        {
            // Arrange
            var mockUObject = new Mock<IUObject>();
            var position = (12, 5);
            var direction = 3;
            var directionsCount = 0;
            var speed = 10;

            mockUObject.Setup(x => x.GetProperty(nameof(IMovableObject.Position))).Returns(position);
            mockUObject.Setup(x => x.GetProperty(Constants.Velocity)).Returns(speed);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(directionsCount);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(direction);

            ICommand moveCommand = new MoveCommand(new MovingObjectAdapter(mockUObject.Object), mockUObject.Object);

            // Act & assert
            Assert.Throws<OverflowException>(moveCommand.Execute);
        }
    }
}
