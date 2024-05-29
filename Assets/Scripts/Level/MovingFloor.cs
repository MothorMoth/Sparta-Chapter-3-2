using UnityEngine;

public class MovingFloor : MonoBehaviour
{
    public float moveSpeed;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = Vector3.forward;
        direction *= moveSpeed;

        _rigidbody.velocity = direction;
    }
}
