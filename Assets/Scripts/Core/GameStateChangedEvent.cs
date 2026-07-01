namespace FlightIGuess.Core
{
    public struct GameStateChangedEvent
    {
        public GameState NewState;
        public GameState PreviousState;
    }
}
