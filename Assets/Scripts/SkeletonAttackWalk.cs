using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simonTriggerW3 : MonoBehaviour
{
    public float attackRange = 4.57f;      
    public float attackInterval = 1.0f;    
    public string attackTrigger = "Fire";  
    public bool ignoreVertical = true;     

    Animator anim;
    Transform player;
    int trigHash;
    float nextAttackTime;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player")?.transform;
        if (!player) Debug.LogError("No object tagged 'Player' found!");
        trigHash = Animator.StringToHash(attackTrigger);
    }

    // Update is called once per frame
    void Update()
    {
        if (!player) return;

        float dist = ignoreVertical
            ? HorizontalDistance(transform.position, player.position)
            : Vector3.Distance(transform.position, player.position);

        
        if (dist <= attackRange && Time.time >= nextAttackTime && !IsAttacking())
        {
            anim.ResetTrigger(trigHash);   
            anim.SetTrigger(trigHash);
            nextAttackTime = Time.time + attackInterval;
        }
    }

    float HorizontalDistance(Vector3 a, Vector3 b) { a.y = 0; b.y = 0; return Vector3.Distance(a, b); }

    bool IsAttacking()
    {
        var st = anim.GetCurrentAnimatorStateInfo(0);
        return st.IsTag("Attack") || st.IsName("Attack01"); 
}
}
