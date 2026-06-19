using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloatEventChannel", menuName = "Event/FloatEventChannel")]
public class FloatEventChannel : ScriptableObject
{
    public UnityAction<float> OnEventRaise;

    public void RaiseEvent(float value)
    {
        OnEventRaise?.Invoke(value);
    }
}
