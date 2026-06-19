using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _offset;

    private void Update()
    {
        if(_playerTransform == null)
        {
            return;
        }
        transform.position = _playerTransform.position + _offset;
    }
}
