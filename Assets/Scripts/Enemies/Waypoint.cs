using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Vector3 gizmoSize = new(0.5f, 2f, 0.5f);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + new Vector3(0f, 0.5f * gizmoSize.y, 0f), gizmoSize);
    }
}
