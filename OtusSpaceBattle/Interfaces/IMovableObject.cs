using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Interfaces
{
    public interface IMovableObject
    {
        /// <summary>
        /// Текущая позиция объекта на плоскости (x, y)
        /// </summary>
        ValueTuple<int, int> Position { get; set; }

        /// <summary>
        /// Вычисленная скорость объекта с учётом направления
        /// </summary>
        /// <returns></returns>
        ValueTuple<int, int> Velocity { get; }
    }
}
