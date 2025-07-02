using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class StartProcessingCommandCollectionCommand : ICommand
    {
        CommandCollection _commandCollection;
        public StartProcessingCommandCollectionCommand(CommandCollection cc) { _commandCollection = cc; }

        public void Execute() => _commandCollection.BackgroundLoop();
    }
}
