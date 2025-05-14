using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Interfaces
{
    public interface IRotatableObject
    {
        /// <summary>
        /// Текущее положение
        /// </summary>
        int Direction { get; set; }

        /// <summary>
        /// Кол-во углов, на сколько повернуть объект за 1 шаг
        /// </summary>
        int DirectionsCountPerStep { get; }

        /// <summary>
        /// Кол-во углов, на которое разделена вся окружность
        /// </summary>
        int DirectionsCount { get; }
    }
}
