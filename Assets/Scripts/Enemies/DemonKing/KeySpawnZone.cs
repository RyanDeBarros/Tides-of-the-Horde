using UnityEngine;

public class KeySpawnZone : MonoBehaviour
{
    [SerializeField] private Vector3 size = new(5, 1, 5);
    [SerializeField] private bool spawnable = true;

    public Vector3 GetRandomPoint()
    {
        return transform.TransformPoint(new Vector3(
            Random.Range(-0.5f, 0.5f) * size.x,
            Random.Range(-0.5f, 0.5f) * size.y,
            Random.Range(-0.5f, 0.5f) * size.z
        ));
    }

    public bool IsSpawnable()
    {
        return spawnable;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = oldMatrix;
    }
}
