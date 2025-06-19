using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using System.Numerics;

namespace OtusSpaceBattle.Adapters
{
    public class CommandAdapter : OtusSpaceBattle.Interfaces.ICommand
    {
        private readonly IUObject gameObject;

        public CommandAdapter(IUObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public void Execute()
        {
            IoC.Resolve<ICommand>("OtusSpaceBattle.Interfaces.ICommand:cute", gameObject).Execute();
        }

    }
}
