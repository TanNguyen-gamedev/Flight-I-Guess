using UnityEngine;

public class ProjectileMisslile : MonoBehaviour
{

    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifeTime = 1f;
    private Rigidbody2D _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector3 direction)
    {
        _rb.AddForce(direction * _speed, ForceMode2D.Impulse);
        Destroy(gameObject, _lifeTime);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
