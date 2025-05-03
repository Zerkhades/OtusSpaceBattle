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
            var calculedVelocity = movableObject.Velocity;
            gameObject.SetProperty(nameof(movableObject.Position),
                (movableObject.Position.Item1 + calculedVelocity.Item1,
                movableObject.Position.Item2 + calculedVelocity.Item2));
        }
    }
}
