using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class StopProcessingCommandCollectionCommand : ICommand
    {
        CommandCollection _commandCollection;
        bool _force = false;
        public StopProcessingCommandCollectionCommand(CommandCollection cc, bool force) { _commandCollection = cc; _force = force; }

        public void Execute() => _commandCollection.Stop(_force);
    }
}
