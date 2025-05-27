using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;

namespace OtusSpaceBattle.Commands
{
    public class MacroCommand : ICommand
    {
        private readonly IEnumerable<ICommand> _cmds;

        public MacroCommand(IEnumerable<ICommand> cmds) { _cmds = cmds; }

        public void Execute()
        {
            foreach (var cmd in _cmds)
            {
                try
                {
                    cmd.Execute();
                }
                catch (Exception)
                {
                    throw new CommandException();
                }
            }
        }
    }
}
