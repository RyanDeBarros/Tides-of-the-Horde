using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpell : MonoBehaviour
{
    public Vector3 blastPosition;
    public float lifetime = 0.3f;

    public MeleeSpell_Shockwave shockwave;
    public MeleeSpell_Blast blast;
    private float timeElapsed = 0f;

    void Awake()
    {
        shockwave = GetComponentInChildren<MeleeSpell_Shockwave>();
        blast = GetComponentInChildren<MeleeSpell_Blast>();
        Assert.IsNotNull(shockwave);
        Assert.IsNotNull(blast);
    }

    private void Start()
    {
        blast.transform.position = blastPosition;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > lifetime) Destroy(gameObject);
    }
}
