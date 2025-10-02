using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairsController : MonoBehaviour
{
    private RawImage image;

    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    public void SetShowing(bool show)
    {
        image.gameObject.SetActive(show);
    }

    public Vector3 GetWorldDirection()
    {
        return Camera.main.ScreenPointToRay(transform.position).direction.normalized;
    }

    public Vector3 GetWorldDirection(Vector3 fromPosition, float maxClip = 1000f)
    {
        Ray ray = Camera.main.ScreenPointToRay(transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, maxClip))
            return (hit.point - fromPosition).normalized;
        else
            return ray.direction.normalized;
    }
}
