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
        private readonly IUObject gameObject;
        public RotateCommand(IRotatableObject ro, IUObject go)
        {
            rotatableObject = ro;
            gameObject = go;
        }
        public void Execute()
        {
            if (rotatableObject.DirectionsCountPerStep < 1)
                throw new Exception("Кол-во поворотов за шаг не может быть меньше 1");
            if (rotatableObject.DirectionsCount < 1)
                throw new Exception("Общее кол-во углов не может быть меньше 1");
            gameObject.SetProperty(nameof(rotatableObject.Direction),
                (rotatableObject.Direction + rotatableObject.DirectionsCountPerStep) % rotatableObject.DirectionsCount);
        }
    }
}
