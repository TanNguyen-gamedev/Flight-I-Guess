using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VoidEventChannel", menuName = "Event/VoidEventChannel")]
public class VoidEventChannel : ScriptableObject
{
    public Action OnEventRaise;

    public void RaiseEvent()
    {
        OnEventRaise?.Invoke();
    }
}
