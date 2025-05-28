using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class ChangeVelocityCommand : ICommand
    {
        private readonly IChangeVelocity _changeVelocity;
        private readonly int _velocity;

        public ChangeVelocityCommand(IChangeVelocity changeVelocity, int velocity)
        {
            _changeVelocity = changeVelocity;
            _velocity = velocity;
        }

        public void Execute() => _changeVelocity.SetVelocity(_velocity);
    }
}
