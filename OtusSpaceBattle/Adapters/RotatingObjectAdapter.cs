using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;



namespace OtusSpaceBattle.Adapters
{
    public class RotatingObjectAdapter : IRotatableObject
    {
        private readonly IUObject gameObject;

        public RotatingObjectAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public int GetAngularVelocity()
        {
            return IoC.Resolve<int>("Spaceship.Operations.IMovable:position.get", gameObject);
        }

        public int GetDirection()
        {
            return IoC.Resolve<int>("Spaceship.Operations.IMovable:position.get", gameObject);
        }

        public int GetDirectionsNumber()
        {
            return IoC.Resolve<int>("Spaceship.Operations.IMovable:position.get", gameObject);
        }

        public void SetDirection(int newV)
        {
            IoC.Resolve<int>("Spaceship.Operations.IRotatableObject:position.get", gameObject);
        }
    }
}
