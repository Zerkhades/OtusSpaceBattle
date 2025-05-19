using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly IMovableObject movableObject;
        private readonly IUObject gameObject;
        public MoveCommand(IMovableObject mo, IUObject go)
        {
            movableObject = mo;
            gameObject = go;
        }

        public void Execute()
        {
            var velocity = movableObject.Velocity;
            var position = movableObject.Position;
            var newPosition = (position.Item1 + velocity.Item1, position.Item2 + velocity.Item2);
            gameObject.SetProperty(nameof(movableObject.Position), newPosition);

        }
    }
}
