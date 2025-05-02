using OtusSpaceBattle;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;

namespace OtusSpaceBattle.Tests
{
    public class MoveObjectTests
    {
        /// <summary>
        /// Тест на то, что с заданым направлением и заданной скоростью объект будет в правильно посчитаной точке
        /// </summary>
        /// <remarks>Для удобства сделал разделение окружности на 8 частей. По уловиям скорость (-7, 3), 
        /// но тогда нужно задать объекту угол 157 градусов, а там при расчётах будут уже дробные числа.
        /// Поэтому скорость будет (-7, 7) и ожидаемый результат соответственно (5, 12)</remarks>
        [Fact]
        public void CorrectlyMoveGameObject()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IMovableObject.Position), (12, 5));
            gameObject.SetProperty(Constants.VELOCITY, 10);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 3);
            IMovableObject moveCommand = new MovingObjectAdapter(gameObject);
            ValueTuple<int, int> expectedPosition = (5, 12);

            moveCommand.Execute();

            Assert.Equal(expectedPosition, gameObject.GetProperty(nameof(IMovableObject.Position)));
        }

        /// <summary>
        /// Попытка сдвинуть объект, у которого невозможно прочитать положение в пространстве, приводит к ошибке
        /// </summary>
        [Fact]
        public void DontSetPosition()
        {
            IUObject gameObject = new Spaceship();
            IMovableObject moveCommand = new MovingObjectAdapter(gameObject);

            Assert.Throws<KeyNotFoundException>(moveCommand.Execute);
        }

        /// <summary>
        /// Попытка сдвинуть объект, у которого невозможно прочитать значение мгновенной скорости, приводит к ошибке
        /// </summary>
        [Fact]
        public void DontSetVelocity()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IMovableObject.Position), (12, 5));
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 8);
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 3);
            IMovableObject moveCommand = new MovingObjectAdapter(gameObject);

            Assert.Throws<KeyNotFoundException>(moveCommand.Execute);
        }

        /// <summary>
        /// Попытка сдвинуть объект, у которого невозможно изменить положение в пространстве, приводит к ошибке
        /// </summary>
        [Fact]
        public void DontChangePosition()
        {
            IUObject gameObject = new Spaceship();
            gameObject.SetProperty(nameof(IMovableObject.Position), (12, 5));
            gameObject.SetProperty(Constants.VELOCITY, 10);
            gameObject.SetProperty(nameof(IRotatableObject.DirectionsCount), 0);
            gameObject.SetProperty(nameof(IRotatableObject.Direction), 3);
            IMovableObject moveCommand = new MovingObjectAdapter(gameObject);

            Assert.Throws<OverflowException>(moveCommand.Execute);
        }
    }
}