using Moq;
using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using Xunit;

namespace OtusSpaceBattle.Tests
{
    public class MoveObjectTests
    {
        public MoveObjectTests()
        {
        }

        [Fact]
        public void MovementTest()
        {
            // ��� �������, ������������ � ����� (12, 5) � ����������� �� ��������� (-7, 3) �������� ������ ��������� ������� �� (5, 8)

            var testdata = new
            {
                position = new Vector2(12, 5),
                velocity = new Vector2(-7, 3),
                want = new Vector2(5, 8)
            };

            Vector2 act = default;

            // Arrange
            var mock = new Mock<IMovableObject>();
            mock.Setup(move => move.GetPosition()).Returns(testdata.position);
            mock.Setup(move => move.GetVelocity()).Returns(testdata.velocity);
            mock.Setup(move => move.SetPosition(It.IsAny<Vector2>())).Callback<Vector2>((v) => act = v);

            // Act
            MoveCommand command = new MoveCommand(mock.Object);
            command.Execute();

            // Assert
            Assert.Equal(act, testdata.want);
        }

        [Fact]
        public void MovementTest_WithoutPosition()
        {
            // ������� �������� ������, � �������� ���������� ��������� ��������� � ������������, �������� � ������

            var testdata = new
            {
                position = new Vector2(12, 5),
                velocity = new Vector2(-7, 3),
                want = new Vector2(5, 8)
            };

            // Arrange
            var mock = new Mock<IMovableObject>();
            mock.Setup(move => move.GetPosition()).Throws(new NotSupportedException());
            mock.Setup(move => move.GetVelocity()).Returns(testdata.velocity);
            mock.Setup(move => move.SetPosition(It.IsAny<Vector2>()));

            // Act
            MoveCommand command = new MoveCommand(mock.Object);

            // Assert
            Assert.Throws<NotSupportedException>(command.Execute);
        }

        [Fact]
        public void MovementTest_WithoutVelocity()
        {
            // ������� �������� ������, � �������� ���������� ��������� �������� ���������� ��������, �������� � ������

            var testdata = new
            {
                position = new Vector2(12, 5),
                velocity = new Vector2(-7, 3),
                want = new Vector2(5, 8)
            };

            // Arrange
            var mock = new Mock<IMovableObject>();
            mock.Setup(move => move.GetPosition()).Returns(testdata.position);
            mock.Setup(move => move.GetVelocity()).Throws(new NotSupportedException());
            mock.Setup(move => move.SetPosition(It.IsAny<Vector2>()));

            // Act
            MoveCommand command = new MoveCommand(mock.Object);

            // Assert
            Assert.Throws<NotSupportedException>(command.Execute);
        }


        [Fact]
        public void MovementTest_NoChangePosition()
        {
            // ������� �������� ������, � �������� ���������� �������� ��������� � ������������, �������� � ������

            var testdata = new
            {
                position = new Vector2(12, 5),
                velocity = new Vector2(-7, 3),
                want = new Vector2(5, 8)
            };

            // Arrange
            var mock = new Mock<IMovableObject>();
            mock.Setup(move => move.GetPosition()).Returns(testdata.position);
            mock.Setup(move => move.GetVelocity()).Returns(testdata.velocity);
            mock.Setup(move => move.SetPosition(It.IsAny<Vector2>())).Throws(new Exception());

            // Act
            MoveCommand command = new MoveCommand(mock.Object);

            // Assert
            Assert.Throws<Exception>(command.Execute);
        }
    }
}
