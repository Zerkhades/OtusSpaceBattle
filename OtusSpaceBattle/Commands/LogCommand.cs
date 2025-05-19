using OtusSpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Commands
{
    public class LogCommand : ICommand
    {
        private readonly Exception exception;
        public LogCommand(Exception exp) => exception = exp;

        public void Execute() => Console.WriteLine($@"Log command: Exception-{exception.GetType()}, ExceptionMessae-{exception.Message}");
    }
}
