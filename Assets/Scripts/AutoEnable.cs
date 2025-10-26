using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoEnable : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects;

    private void Awake()
    {
        objects.ForEach(go => go.SetActive(true));
    }
}
