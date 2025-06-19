using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.Numerics;

namespace OtusSpaceBattle.Adapters
{
    public class FuelableObjectAdapter : OtusSpaceBattle.Interfaces.IFuelableObject
    {
        private readonly IUObject gameObject;

        public FuelableObjectAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public void BurnFuel()
        {
            IoC.Resolve<ICommand>("OtusSpaceBattle.Interfaces.IFuelableObject:nfuel", gameObject).Execute();
        }

    }
}
