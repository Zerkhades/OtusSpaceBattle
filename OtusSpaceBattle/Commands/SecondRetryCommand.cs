using OtusSpaceBattle.Interfaces;

namespace OtusSpaceBattle.Commands
{
    public class SecondRetryCommand : ICommand
    {
        ICommand _command;
        public SecondRetryCommand(ICommand c) { _command = c; }
        public void Execute() => _command.Execute();
    }
}
