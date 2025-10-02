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
}
