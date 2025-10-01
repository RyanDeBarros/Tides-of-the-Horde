using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpell : MonoBehaviour
{
    public Vector3 blastPosition;
    public float lifetime = 0.3f;
    public float shockwaveGrowSpeed = 20f;
    public float blastGrowSpeed = 10f;
    public float moveSpeed = 10f;

    private SphereCollider shockwaveCollider;
    private SphereCollider blastCollider;
    private float timeElapsed = 0f;

    void Awake()
    {
        shockwaveCollider = GetComponent<SphereCollider>();
        foreach (SphereCollider c in GetComponentsInChildren<SphereCollider>())
        {
            if (c != shockwaveCollider)
            {
                blastCollider = c;
                break;
            }    
        }
    }

    private void Start()
    {
        Assert.IsNotNull(shockwaveCollider);
        Assert.IsNotNull(blastCollider);
        shockwaveCollider.radius = 0.0f;
        blastCollider.transform.position = blastPosition;
        blastCollider.radius = 0.0f;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed <  lifetime)
        {
            shockwaveCollider.radius += shockwaveGrowSpeed * Time.deltaTime;
            blastCollider.radius += blastGrowSpeed * Time.deltaTime;
            blastCollider.transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(shockwaveCollider.transform.position, shockwaveCollider.radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(blastCollider.transform.position, blastCollider.radius);
    }
}
