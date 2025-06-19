using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly IMovableObject movableObject;
        public MoveCommand(IMovableObject mo)
        {
            movableObject = mo;
        }

        public void Execute() => movableObject.SetPosition(
            Vector2.Add(movableObject.GetPosition(),
                movableObject.GetVelocity()));
    }
}
