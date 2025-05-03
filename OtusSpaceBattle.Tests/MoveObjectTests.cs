using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;

namespace OtusSpaceBattle.Tests
{
    public class MoveObjectTests
    {
        [Fact]
        public void CorrectlyMoveGameObject()
        {
            // Arrange
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IMovableObject.Position), (12, 5));
            gameObject.SetProperty(Constants.Velocity, 10);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 3);
            ICommand cm = new MoveCommand(new MovingObjectAdapter(gameObject), gameObject);
            // Act
            cm.Execute();
            ValueTuple<int, int> expectedPosition = (5, 12);
            // Assert
            Assert.Equal(expectedPosition, gameObject.GetProperty(nameof(IMovableObject.Position)));
        }

        [Fact]
        public void DontSetPosition()
        {
            // Arrange
            IUObject gameObject = new Spaceship();
            ICommand cm = new MoveCommand(new MovingObjectAdapter(gameObject), gameObject);

            // Act & assert
            Assert.Throws<KeyNotFoundException>(cm.Execute);
        }

        [Fact]
        public void DontSetVelocity()
        {
            // Arrange
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IMovableObject.Position), (12, 5));
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 3);
            ICommand cm = new MoveCommand(new MovingObjectAdapter(gameObject), gameObject);

            // Act & assert
            Assert.Throws<KeyNotFoundException>(cm.Execute);
        }

        [Fact]
        public void DontChangePosition()
        {
            // Arrange
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IMovableObject.Position), (12, 5));
            gameObject.SetProperty(Constants.Velocity, 10);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 0);
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 3);
            ICommand cm = new MoveCommand(new MovingObjectAdapter(gameObject), gameObject);

            // Act & assert
            Assert.Throws<OverflowException>(cm.Execute);
        }
    }
}