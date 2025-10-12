using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Lifezone : MonoBehaviour
{
    [SerializeField] private Vector2 boundsX = new(-500, 500);
    [SerializeField] private Vector2 boundsY = new(-50, 100);
    [SerializeField] private Vector2 boundsZ = new(-500, 500);

    private void Awake()
    {
        Assert.IsTrue(boundsX[0] <= boundsX[1]);
        Assert.IsTrue(boundsY[0] <= boundsY[1]);
        Assert.IsTrue(boundsZ[0] <= boundsZ[1]);
    }

    private void Update()
    {
        if (transform.position.x < boundsX[0] || transform.position.x > boundsX[1]
            || transform.position.y < boundsY[0] || transform.position.y > boundsY[1]
            || transform.position.z < boundsZ[0] || transform.position.z > boundsZ[1])
            Destroy(gameObject);
    }
}
