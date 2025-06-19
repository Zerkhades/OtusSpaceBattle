using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.Numerics;

namespace OtusSpaceBattle.Adapters
{
    public class RotatableObjectAdapter : OtusSpaceBattle.Interfaces.IRotatableObject
    {
        private readonly IUObject gameObject;

        public RotatableObjectAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public int GetDirection()
        {
            return IoC.Resolve<int>("OtusSpaceBattle.Interfaces.IRotatableObject:direction.get", gameObject);
        }

        public int GetAngularVelocity()
        {
            return IoC.Resolve<int>("OtusSpaceBattle.Interfaces.IRotatableObject:angularvelocity.get", gameObject);
        }

        public void SetDirection(int newV)
        {
            IoC.Resolve<ICommand>("OtusSpaceBattle.Interfaces.IRotatableObject:direction.set", gameObject, newV).Execute();
        }

        public int GetDirectionsNumber()
        {
            return IoC.Resolve<int>("OtusSpaceBattle.Interfaces.IRotatableObject:directionsnumber.get", gameObject);
        }

    }
}
