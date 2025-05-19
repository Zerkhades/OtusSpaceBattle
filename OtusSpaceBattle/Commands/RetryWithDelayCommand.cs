using OtusSpaceBattle.Interfaces;
using System;

namespace OtusSpaceBattle.Commands
{
    public class RetryWithDelayCommand : ICommand
    {
        ICommand _command;
        CommandCollection _commandCollection;
        public RetryWithDelayCommand(CommandCollection cc, ICommand c) { _commandCollection = cc; _command = c; }
        public void Execute() => _commandCollection.Add(_command);
    }
}
