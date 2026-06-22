namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// A trigger that requires the input to be held for a specific duration before firing.
    /// Resets the charge if the input is released early.
    /// </summary>
    public class ChargeTrigger : IWeaponTrigger
    {
        private readonly float _chargeTimeRequired;
        private float _currentCharge;

        public ChargeTrigger(float chargeTimeRequired)
        {
            _chargeTimeRequired = chargeTimeRequired;
            _currentCharge = 0f;
        }

        public bool Evaluate(float deltaTime, bool isInputHeld)
        {
            if (isInputHeld)
            {
                _currentCharge += deltaTime;
                
                // Fire when the charge time is reached
                if (_currentCharge >= _chargeTimeRequired)
                {
                    _currentCharge = 0f; // Reset for the next charge
                    return true;
                }
            }
            else
            {
                // Reset charge if the player lets go early
                _currentCharge = 0f;
            }

            return false;
        }
    }
}