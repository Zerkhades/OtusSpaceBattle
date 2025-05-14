using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Models
{
    public class Spaceship : IUObject
    {
        private readonly Dictionary<string, object> properties = new();

        public object GetProperty(string name) => properties[name];

        public void SetProperty(string name, object value) => properties[name] = value;
    }
}
