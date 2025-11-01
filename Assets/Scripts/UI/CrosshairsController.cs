using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class CrosshairsController : MonoBehaviour
{
    private RawImage image;

    private void Awake()
    {
        image = GetComponent<RawImage>();
        Assert.IsNotNull(image);
    }

    public void SetShowing(bool show)
    {
        image.enabled = show;
    }

    public Vector3 GetWorldDirection()
    {
        return Camera.main.ScreenPointToRay(transform.position).direction.normalized;
    }
}
