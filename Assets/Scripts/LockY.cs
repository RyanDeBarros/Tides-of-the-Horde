using UnityEngine;

public class LockY : MonoBehaviour
{
    public float y = 0f;

    private void Start()
    {
        Recalibrate();
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }

    public void Recalibrate()
    {
        y = transform.position.y;
    }
}
