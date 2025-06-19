using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.Numerics;

namespace OtusSpaceBattle.Adapters
{
    public class ChangeVelocityAdapter : OtusSpaceBattle.Interfaces.IChangeVelocity
    {
        private readonly IUObject gameObject;

        public ChangeVelocityAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public void SetVelocity(Vector2 velocity)
        {
            IoC.Resolve<ICommand>("OtusSpaceBattle.Interfaces.IChangeVelocity:velocity.set", gameObject, velocity).Execute();
        }

    }
}
