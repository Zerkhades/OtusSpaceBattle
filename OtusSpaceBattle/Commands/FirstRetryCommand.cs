using OtusSpaceBattle.Interfaces;

namespace OtusSpaceBattle.Commands
{
    public class FirstRetryCommand : ICommand
    {
        ICommand _command;
        public FirstRetryCommand(ICommand c) { _command = c; }
        public void Execute() => _command.Execute();
    }
}
