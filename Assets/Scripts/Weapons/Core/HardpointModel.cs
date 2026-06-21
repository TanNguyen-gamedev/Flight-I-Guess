using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Pure C# model representing a physical slot on the ship where a weapon can be mounted.
    /// </summary>
    public class HardpointModel
    {
        public string HardpointId { get; }
        public WeaponModel EquippedWeapon { get; private set; }

        public HardpointModel(string hardpointId)
        {
            HardpointId = hardpointId;
        }

        public void Equip(WeaponModel weapon)
        {
            EquippedWeapon = weapon;
        }

        public void Tick(float deltaTime, bool isInputHeld, IProjectileSpawner spawner, Vector2 currentPos, Vector2 currentDir)
        {
            EquippedWeapon?.Tick(deltaTime, isInputHeld, spawner, currentPos, currentDir);
        }
    }
}
