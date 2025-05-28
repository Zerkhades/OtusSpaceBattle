using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Adapters
{
    public class BurnFuelAdapter /*: IFuelableObject*/
    {
        private readonly IUObject gameObject;

        public BurnFuelAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public bool PossibleToBurnFuel
        {
            get => (bool)gameObject.GetProperty(nameof(PossibleToBurnFuel));
            set => gameObject.SetProperty(nameof(PossibleToBurnFuel), value);
        }
    }
}
