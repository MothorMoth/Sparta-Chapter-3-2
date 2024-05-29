using UnityEngine;

public class MovingFloorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MovingFloor movingFloor))
        {
            movingFloor.moveSpeed *= -1f;
        }
    }
}
