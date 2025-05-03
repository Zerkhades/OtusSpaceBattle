using OtusSpaceBattle.Adapters;
using OtusSpaceBattle.Commands;
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

        [Fact]
        public void CorrectlyRotateGameObject()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);

            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(gameObject), gameObject);
            int expectedDirection = 3;

            for (int i = 0; i < 10; i++)
                rotateCommand.Execute();

            Assert.Equal(expectedDirection, gameObject.GetProperty(nameof(IRotatableObject.Direction)));
        }

        [Fact]
        public void DontSetDirection()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(gameObject), gameObject);

            Assert.Throws<KeyNotFoundException>(rotateCommand.Execute);
        }

        [Fact]
        public void DontSetDirectionsCountPerStep()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(gameObject), gameObject);

            Assert.Throws<KeyNotFoundException>(rotateCommand.Execute);
        }

        [Fact]
        public void DontSetDirectionsCount()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), 1);
            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(gameObject), gameObject);

            Assert.Throws<KeyNotFoundException>(rotateCommand.Execute);
        }

        [Fact]
        public void SetDirectionsCountLessThan1()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), -2);
            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(gameObject), gameObject);

            Assert.Throws<Exception>(rotateCommand.Execute);
        }

        [Fact]
        public void SetDirectionsCountPerStepLessThan1()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), -1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 0);
            ICommand rotateCommand = new RotateCommand(new RotatingObjectAdapter(gameObject), gameObject);

            Assert.Throws<Exception>(rotateCommand.Execute);
        }
    }
}
