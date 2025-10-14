using UnityEngine;
using UnityEngine.AI;

public class RestrictToNavMesh : MonoBehaviour
{
    [SerializeField] float maxDistance = 0.2f;

    void LateUpdate()
    {
        // Check if still on NavMesh
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
        {
            // Snap back to closest valid NavMesh position
            transform.position = hit.position;
        }
    }
}
