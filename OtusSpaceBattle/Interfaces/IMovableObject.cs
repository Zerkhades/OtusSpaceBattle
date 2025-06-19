using OtusSpaceBattle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OtusSpaceBattle.Interfaces
{
    public interface IMovableObject
    {
        Vector2 GetPosition();
        Vector2 GetVelocity();
        void SetPosition(Vector2 newV);
    }
}
