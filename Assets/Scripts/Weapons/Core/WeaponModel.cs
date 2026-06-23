using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Pure C# model representing a weapon. 
    /// Composes a Trigger (When) and an Emitter (How) to execute firing logic.
    /// </summary>
    public class WeaponModel
    {
        public event System.Action OnFired;

        private readonly IWeaponTrigger _trigger;
        private readonly IWeaponEmitter _emitter;
        private readonly string _projectileId;
        private readonly string _effectId;

        public WeaponModel(IWeaponTrigger trigger, IWeaponEmitter emitter, string projectileId, string effectId)
        {
            _trigger = trigger;
            _emitter = emitter;
            _projectileId = projectileId;
            _effectId = effectId;
        }

        public void Tick(float deltaTime, bool isInputHeld, IProjectileSpawner projectileSpawner, IEffectSpawner effectSpawner, Vector2 hardpointPosition, Vector2 hardpointDirection)
        {
            if (_trigger.Evaluate(deltaTime, isInputHeld))
            {
                _emitter.Emit(projectileSpawner, effectSpawner, _projectileId, _effectId, hardpointPosition, hardpointDirection);
                OnFired?.Invoke();
            }
        }
    }
}