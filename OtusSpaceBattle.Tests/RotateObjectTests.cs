using Moq;
using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Tests
{
    public class RotateObjectTests
    {
        public RotateObjectTests()
        {
        }

        [Fact]
        public void CorrectlyRotateGameObject()
        {
            // Arrange
            var mockUObject = new Mock<IUObject>();
            int direction = 1;
            int directionsCountPerStep = 1;
            int directionsCount = 8;
            int setDirection = direction;

            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(() => setDirection);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCountPerStep))).Returns(directionsCountPerStep);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(directionsCount);
            mockUObject.Setup(x => x.SetProperty(nameof(IRotatableObject.Direction), It.IsAny<object>()))
                .Callback<string, object>((name, value) => setDirection = (int)value);

            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(mockUObject.Object), mockUObject.Object);
            int expectedDirection = 3;
            // Act
            for (int i = 0; i < 10; i++)
                rotateCommand.Execute();
            // Assert
            Assert.Equal(expectedDirection, setDirection);
        }

        [Theory]
        [InlineData(nameof(IRotatableObject.Direction))]
        [InlineData(nameof(IRotatableObject.DirectionsCountPerStep))]
        [InlineData(nameof(IRotatableObject.DirectionsCount))]
        public void MissingRequiredPropertyThrowsException(string missingProperty)
        {
            // Arrange
            var mockUObject = new Mock<IUObject>();
            int direction = 1;
            int directionsCountPerStep = 1;
            int directionsCount = 8;

            if (missingProperty != nameof(IRotatableObject.Direction))
                mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(direction);
            else
                mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Throws<KeyNotFoundException>();

            if (missingProperty != nameof(IRotatableObject.DirectionsCountPerStep))
                mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCountPerStep))).Returns(directionsCountPerStep);
            else
                mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCountPerStep))).Throws<KeyNotFoundException>();

            if (missingProperty != nameof(IRotatableObject.DirectionsCount))
                mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(directionsCount);
            else
                mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Throws<KeyNotFoundException>();

            mockUObject.Setup(x => x.SetProperty(nameof(IRotatableObject.Direction), It.IsAny<object>()));

            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(mockUObject.Object), mockUObject.Object);
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(rotateCommand.Execute);
        }

        [Theory]
        [InlineData(nameof(IRotatableObject.DirectionsCount), -2)]
        [InlineData(nameof(IRotatableObject.DirectionsCountPerStep), -1)]
        public void InvalidPropertyValuesThrowException(string propertyName, object invalidValue)
        {
            // Arrange
            var mockUObject = new Mock<IUObject>();
            int direction = 1;
            int directionsCountPerStep = 1;
            int directionsCount = 8;

            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.Direction))).Returns(direction);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCountPerStep))).Returns(directionsCountPerStep);
            mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.DirectionsCount))).Returns(directionsCount);
            mockUObject.Setup(x => x.SetProperty(nameof(IRotatableObject.Direction), It.IsAny<object>()));

            mockUObject.Setup(x => x.GetProperty(propertyName)).Returns(invalidValue);

            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(mockUObject.Object), mockUObject.Object);
            // Act & assert
            Assert.Throws<Exception>(rotateCommand.Execute);
        }
    }
}
