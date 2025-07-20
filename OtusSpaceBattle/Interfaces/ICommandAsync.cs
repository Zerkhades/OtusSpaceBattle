using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Interfaces
{
    public interface ICommandAsync
    {
        public Task ExecuteAsync(CancellationToken ct = default);
    }
}
