namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Strategy for determining WHEN a weapon fires (e.g., auto-fire, charge up, burst).
    /// </summary>
    public interface IWeaponTrigger
    {
        /// <summary>
        /// Evaluates whether the weapon should fire this frame.
        /// </summary>
        /// <param name="deltaTime">Time since last tick.</param>
        /// <param name="isInputHeld">Whether the player is currently holding the fire button.</param>
        /// <returns>True if a projectile should be emitted.</returns>
        bool Evaluate(float deltaTime, bool isInputHeld);
    }
}
