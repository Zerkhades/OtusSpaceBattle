using OtusSpaceBattle.Interfaces;
using System.Numerics;


namespace OtusSpaceBattle.Commands
{
    public class ChangeVelocityCommand : ICommand
    {
        private readonly IChangeVelocity _changeVelocity;
        private readonly Vector2 _velocity;

        public ChangeVelocityCommand(IChangeVelocity changeVelocity, Vector2 velocity)
        {
            _changeVelocity = changeVelocity;
            _velocity = velocity;
        }

        public void Execute() => _changeVelocity.SetVelocity(_velocity);
    }
}
