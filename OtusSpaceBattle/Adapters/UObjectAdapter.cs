using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.Numerics;

namespace OtusSpaceBattle.Adapters
{
    public class UObjectAdapter : OtusSpaceBattle.Interfaces.IUObject
    {
        private readonly IUObject gameObject;

        public UObjectAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public object GetProperty(string name)
        {
            return IoC.Resolve<object>("OtusSpaceBattle.Interfaces.IUObject:property.get", gameObject, name);
        }

        public void SetProperty(string name, object value)
        {
            IoC.Resolve<ICommand>("OtusSpaceBattle.Interfaces.IUObject:property.set", gameObject, name, value).Execute();
        }

    }
}
