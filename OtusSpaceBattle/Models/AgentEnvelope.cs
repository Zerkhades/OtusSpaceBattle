using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Models
{
    public record AgentEnvelope(
        string GameId,
        string TargetId,
        string OpId,
        JsonElement Args,
        long Seq,
        DateTime Ts);
}
