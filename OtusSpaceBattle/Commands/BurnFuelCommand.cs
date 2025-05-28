using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class BurnFuelCommand : ICommand
    {
        IFuelableObject _fuelObj;
        public BurnFuelCommand(IFuelableObject obj) { _fuelObj = obj; }
        public void Execute()
        {
            _fuelObj.BurnFuel();
        }
    }
}
