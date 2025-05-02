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
        /// ���� �� ��, ��� � ������� ������������ � �������� ��������� ������ ����� � ��������� ���������� �����
        /// </summary>
        /// <remarks>��� �������� ������ ���������� ���������� �� 8 ������. �� ������� �������� (-7, 3), 
        /// �� ����� ����� ������ ������� ���� 157 ��������, � ��� ��� �������� ����� ��� ������� �����.
        /// ������� �������� ����� (-7, 7) � ��������� ��������� �������������� (5, 12)</remarks>
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
        /// ������� �������� ������, � �������� ���������� ��������� ��������� � ������������, �������� � ������
        /// </summary>
        [Fact]
        public void DontSetPosition()
        {
            IUObject gameObject = new Spaceship();
            IMovableObject moveCommand = new MovingObjectAdapter(gameObject);

            Assert.Throws<KeyNotFoundException>(moveCommand.Execute);
        }

        /// <summary>
        /// ������� �������� ������, � �������� ���������� ��������� �������� ���������� ��������, �������� � ������
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
        /// ������� �������� ������, � �������� ���������� �������� ��������� � ������������, �������� � ������
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