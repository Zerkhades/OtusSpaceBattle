﻿using Microsoft.VisualBasic;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Constants = OtusSpaceBattle.Models.Constants;

namespace OtusSpaceBattle.Adapters
{
    public class MovingObjectAdapter : IMovableObject
    {
        private readonly IUObject gameObject;

        public MovingObjectAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public ValueTuple<int, int> Position
        {
            get => (ValueTuple<int, int>)gameObject.GetProperty(nameof(Position));
            set => gameObject.SetProperty(nameof(Position), value);
        }

        public ValueTuple<int, int> Velocity
        {
            get
            {
                int currentDirection = (int)gameObject.GetProperty(nameof(IRotatableObject.Direction));
                int speed = (int)gameObject.GetProperty(Constants.Velocity);
                int directionsCount = (int)gameObject.GetProperty(nameof(IRotatableObject.DirectionsCount));
                double degree = (double)currentDirection / directionsCount * 360;
                var (sin, cos) = Math.SinCos(Math.PI * degree / 180.0);
                return (
                    Convert.ToInt32(speed * cos),
                    Convert.ToInt32(speed * sin)
                );
            }
        }
    }
}
