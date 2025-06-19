using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class RotateCommand : ICommand
    {
        private readonly IRotatableObject rotatableObject;
        public RotateCommand(IRotatableObject ro)
        {
            rotatableObject = ro;
        }
        public void Execute() => 
            rotatableObject.SetDirection((rotatableObject.GetDirection() +
                rotatableObject.GetAngularVelocity()) %
                rotatableObject.GetDirectionsNumber());
    }
}
