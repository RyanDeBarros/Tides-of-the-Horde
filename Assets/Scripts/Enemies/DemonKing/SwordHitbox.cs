using UnityEngine;
using UnityEngine.Events;

public class SwordHitbox : MonoBehaviour
{
    public UnityEvent<Transform> onCollision;

    public void EnableCollision()
    {
        gameObject.SetActive(true);
    }

    public void DisableCollision()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        onCollision?.Invoke(other.transform);
    }
}
