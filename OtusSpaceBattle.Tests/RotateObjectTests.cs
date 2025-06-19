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
        public void RotationTest()
        {

            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            int act = default;

            // Arrange
            var mock = new Mock<IRotatableObject>();
            mock.Setup(move => move.GetAngularVelocity()).Returns(testdata.AngularVelocity);
            mock.Setup(move => move.GetDirectionsNumber()).Returns(testdata.DirectionsNumber);
            mock.Setup(move => move.GetDirection()).Returns(testdata.Direction);
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Callback<int>((v) => act = v);

            // Act
            RotateCommand command = new RotateCommand(mock.Object);
            command.Execute();

            // Assert
            Assert.Equal(act, testdata.want);
        }

        [Fact]
        public void RotationTest_WithoutAngularVelocity()
        {
            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            int act = default;

            // Arrange
            var mock = new Mock<IRotatableObject>();
            mock.Setup(move => move.GetAngularVelocity()).Throws(new NotSupportedException());
            mock.Setup(move => move.GetDirectionsNumber()).Returns(testdata.DirectionsNumber);
            mock.Setup(move => move.GetDirection()).Returns(testdata.Direction);
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Callback<int>((v) => act = v);

            // Act
            RotateCommand command = new RotateCommand(mock.Object);

            // Assert
            Assert.Throws<NotSupportedException>(command.Execute);
        }

        [Fact]
        public void RotationTest_DirectionsNumber()
        {
            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            int act = default;

            // Arrange
            var mock = new Mock<IRotatableObject>();
            mock.Setup(move => move.GetAngularVelocity()).Returns(testdata.AngularVelocity);
            mock.Setup(move => move.GetDirectionsNumber()).Throws(new NotSupportedException());
            mock.Setup(move => move.GetDirection()).Returns(testdata.Direction);
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Callback<int>((v) => act = v);

            // Act
            RotateCommand command = new RotateCommand(mock.Object);

            // Assert
            Assert.Throws<NotSupportedException>(command.Execute);
        }

        [Fact]
        public void RotationTest_GetDirection()
        {
            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            int act = default;

            // Arrange
            var mock = new Mock<IRotatableObject>();
            mock.Setup(move => move.GetAngularVelocity()).Returns(testdata.AngularVelocity);
            mock.Setup(move => move.GetDirectionsNumber()).Returns(testdata.DirectionsNumber);
            mock.Setup(move => move.GetDirection()).Throws(new NotSupportedException());
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Callback<int>((v) => act = v);

            // Act
            RotateCommand command = new RotateCommand(mock.Object);

            // Assert
            Assert.Throws<NotSupportedException>(command.Execute);
        }

        [Fact]
        public void RotationTest_NoChangeDirection()
        {
            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            // Arrange
            var mock = new Mock<IRotatableObject>();
            mock.Setup(move => move.GetAngularVelocity()).Returns(testdata.AngularVelocity);
            mock.Setup(move => move.GetDirectionsNumber()).Returns(testdata.DirectionsNumber);
            mock.Setup(move => move.GetDirection()).Returns(testdata.Direction);
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Throws(new Exception());

            // Act
            RotateCommand command = new RotateCommand(mock.Object);

            // Assert
            Assert.Throws<Exception>(command.Execute);
        }
    }
}
