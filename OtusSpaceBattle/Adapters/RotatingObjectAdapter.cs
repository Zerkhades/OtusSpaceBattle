using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Adapters
{
    public class RotatingObjectAdapter : IRotatableObject
    {
        private readonly IUObject gameObject;

        public RotatingObjectAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public int Direction
        {
            get => (int)gameObject.GetProperty(nameof(Direction));
            set => gameObject.SetProperty(nameof(Direction), value % DirectionsCount);
        }

        public int DirectionsCountPerStep =>
            (int)gameObject.GetProperty(nameof(DirectionsCountPerStep));

        public int DirectionsCount =>
            (int)gameObject.GetProperty(nameof(DirectionsCount));

    }
}
