using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.Numerics;

namespace OtusSpaceBattle.Adapters
{
    public class CheckFuelAdapter : OtusSpaceBattle.Interfaces.ICheckFuel
    {
        private readonly IUObject gameObject;

        public CheckFuelAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public bool CheckFuel()
        {
            return IoC.Resolve<bool>("OtusSpaceBattle.Interfaces.ICheckFuel:ckfuel", gameObject);
        }

    }
}
