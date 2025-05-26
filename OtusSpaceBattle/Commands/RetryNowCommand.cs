using OtusSpaceBattle.Interfaces;

namespace OtusSpaceBattle.Commands
{
    public class RetryNowCommand : ICommand
    {
        ICommand _command;
        public RetryNowCommand(ICommand c) { _command = c; }
        public void Execute() => _command.Execute();
    }
}
