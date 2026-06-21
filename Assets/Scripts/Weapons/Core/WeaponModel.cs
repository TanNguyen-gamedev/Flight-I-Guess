using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Pure C# model representing a weapon. 
    /// Composes a Trigger (When) and an Emitter (How) to execute firing logic.
    /// </summary>
    public class WeaponModel
    {
        private readonly IWeaponTrigger _trigger;
        private readonly IWeaponEmitter _emitter;
        private readonly string _projectileId;

        public WeaponModel(IWeaponTrigger trigger, IWeaponEmitter emitter, string projectileId)
        {
            _trigger = trigger;
            _emitter = emitter;
            _projectileId = projectileId;
        }

        public void Tick(float deltaTime, bool isInputHeld, IProjectileSpawner spawner, Vector2 hardpointPosition, Vector2 hardpointDirection)
        {
            if (_trigger.Evaluate(deltaTime, isInputHeld))
            {
                _emitter.Emit(spawner, _projectileId, hardpointPosition, hardpointDirection);
            }
        }
    }
}
