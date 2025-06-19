using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.Numerics;


namespace OtusSpaceBattle.Adapters
{
    [Obsolete("Use MovableAdapter instead")]
    public class MovingObjectAdapter : IMovableObject
    {
        private readonly IUObject gameObject;

        public MovingObjectAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public Vector2 GetPosition()
        {
            return IoC.Resolve<Vector2>("OtusSpaceBattle.Interfaces.IMovableObject:position.get", gameObject);
        }

        public Vector2 GetVelocity()
        {
            return IoC.Resolve<Vector2>("OtusSpaceBattle.Interfaces.IMovableObject:velocity.get", gameObject);
        }

        public void SetPosition(Vector2 newV)
        {
            IoC.Resolve<ICommand>("OtusSpaceBattle.Interfaces.IMovableObject:position.set", gameObject, newV).Execute();
        }
    }
}
