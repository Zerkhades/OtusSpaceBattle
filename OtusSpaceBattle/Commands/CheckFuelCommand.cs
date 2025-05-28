using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class CheckFuelCommand : ICommand
    {
        ICheckFuel _checkFuel;
        public CheckFuelCommand(ICheckFuel checkFuel)
        {
            _checkFuel = checkFuel;
        }

        public void Execute()
        {
            if (!_checkFuel.CheckFuel())
                throw new CommandException();
        }
    }
}
