using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Interfaces
{
    public interface IUObject
    {
        /// <summary>
        /// Получить значение свойства объекта с именем <paramref name="name"/>
        /// </summary>
        /// <param name="name">Название свойства</param>
        /// <returns>Значение свойства <paramref name="name"/></returns>
        object GetProperty(string name);

        /// <summary>
        /// Задать значение <paramref name="value"/> свойства объекта с именем <paramref name="name"/>
        /// </summary>
        /// <param name="name">Название свойства</param>
        /// <param name="value">Значение свойства</param>
        void SetProperty(string name, object value);
    }
}
