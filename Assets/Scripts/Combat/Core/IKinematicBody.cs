using System.Numerics;

namespace FlightIGuess.Combat.Core
{
    public interface IKinematicBody
    {
        float Mass {get;}
        Vector2 ForwardDirection {get;}
    }
}