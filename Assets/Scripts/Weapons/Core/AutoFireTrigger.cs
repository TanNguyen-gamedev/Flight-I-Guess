namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// A trigger that fires continuously as long as the input is held, based on a fire rate.
    /// </summary>
    public class AutoFireTrigger : IWeaponTrigger
    {
        private readonly float _fireCooldown;
        private float _currentTimer;

        /// <summary>
        /// </summary>
        /// <param name="fireRatePerSecond">How many times per second this weapon fires.</param>
        public AutoFireTrigger(float fireRatePerSecond)
        {
            _fireCooldown = 1f / fireRatePerSecond;
            _currentTimer = 0f;
        }

        public bool Evaluate(float deltaTime, bool isInputHeld)
        {
            if (_currentTimer > 0f)
            {
                _currentTimer -= deltaTime;
            }

            if (isInputHeld && _currentTimer <= 0f)
            {
                _currentTimer = _fireCooldown;
                return true;
            }

            return false;
        }
    }
}
