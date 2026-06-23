using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EnemyDeathEventChannel", menuName = "Event/EnemyDeathEventChannel")]
public class EnemyDeathEventChannel : ScriptableObject
{
    public UnityAction<Vector3, int, ResourceType> OnEventRaise;

    public void RaiseEvent(Vector3 position, int amount, ResourceType resourceType)
    {
        OnEventRaise?.Invoke(position, amount, resourceType);
    }
}
