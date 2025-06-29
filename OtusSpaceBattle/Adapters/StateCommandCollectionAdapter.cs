using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.Numerics;

namespace OtusSpaceBattle.Commands
{
    public class StateCommandCollectionAdapter : OtusSpaceBattle.Commands.IStateCommandCollection
    {
        private readonly IUObject gameObject;

        public StateCommandCollectionAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public bool Next()
        {
            return IoC.Resolve<bool>("OtusSpaceBattle.Commands.IStateCommandCollection:t", gameObject);
        }

    }
}
