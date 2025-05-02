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

        /// <summary>
        /// Для объекта, повернутого на 45 градусов (значение 1, значит всего 8 углов) 10 поворотов меняет направление на 75 градусов (значение 3)
        /// </summary>
        [Fact]
        public void CorrectlyRotateGameObject()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            IRotatableObject rotateCommand = new RotatingObjectAdapter(gameObject);
            int expectedDirection = 3;

            for (int i = 0; i < 10; i++)
                rotateCommand.Execute();

            Assert.Equal(expectedDirection, gameObject.GetProperty(nameof(IRotatableObject.Direction)));
        }

        /// <summary>
        /// Попытка повернуть объект, у которого невозможно прочитать направление, приводит к ошибке
        /// </summary>
        [Fact]
        public void DontSetDirection()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            IRotatableObject rotateCommand = new RotatingObjectAdapter(gameObject);

            Assert.Throws<KeyNotFoundException>(rotateCommand.Execute);
        }

        /// <summary>
        /// Попытка повернуть объект, у которого невозможно прочитать кол-во углов за шаг, приводит к ошибке
        /// </summary>
        [Fact]
        public void DontSetDirectionsCountPerStep()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            IRotatableObject rotateCommand = new RotatingObjectAdapter(gameObject);

            Assert.Throws<KeyNotFoundException>(rotateCommand.Execute);
        }

        /// <summary>
        /// Попытка повернуть объект, у которого невозможно прочитать кол-во углов, приводит к ошибке
        /// </summary>
        [Fact]
        public void DontSetDirectionsCount()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), 1);
            IRotatableObject rotateCommand = new RotatingObjectAdapter(gameObject);

            Assert.Throws<KeyNotFoundException>(rotateCommand.Execute);
        }

        /// <summary>
        /// Ошибка при задании кол-ва углов меньше 1
        /// </summary>
        [Fact]
        public void SetDirectionsCountLessThan1()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), -2);
            IRotatableObject rotateCommand = new RotatingObjectAdapter(gameObject);

            Assert.Throws<Exception>(rotateCommand.Execute);
        }

        /// <summary>
        /// Ошибка при задании шага поворота меньше 1
        /// </summary>
        [Fact]
        public void SetDirectionsCountPerStepLessThan1()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCountPerStep), -1);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 0);
            IRotatableObject rotateCommand = new RotatingObjectAdapter(gameObject);

            Assert.Throws<Exception>(rotateCommand.Execute);
        }
    }
}
