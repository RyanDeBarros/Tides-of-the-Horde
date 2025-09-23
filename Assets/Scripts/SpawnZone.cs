using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] private Vector3 size = new(5, 1, 5);
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, size);
    }

    public Vector3 GetRandomPoint()
    {
        return transform.position + 0.5f * new Vector3(
            Random.Range(-size.x, size.x),
            Random.Range(-size.y, size.y),
            Random.Range(-size.z, size.z)
        );
    }
}
