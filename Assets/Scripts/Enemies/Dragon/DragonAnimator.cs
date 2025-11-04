using UnityEngine;
using UnityEngine.Assertions;

public class DragonAnimator : MonoBehaviour
{
    [SerializeField] private float deathSinkDelay = 0.4f;
    [SerializeField] private float deathSinkSpeed = 8f;
    // TODO set death SFX in OnDieAnimation inspector
    // TODO spatial SFX for telegraph

    private Animator animator;
    private DragonAOEAttack attacker;
    private TargetDetector detector;

    private bool dead = false;
    private float timeElapsed = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator);

        attacker = GetComponentInParent<DragonAOEAttack>();
        Assert.IsNotNull(attacker);

        detector = GetComponentInParent<TargetDetector>();
        Assert.IsNotNull(detector);
        detector.attackConditions.Add(CanAttack);
    }

    private void Update()
    {
        if (dead)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > deathSinkDelay)
            {
                Vector3 pos = transform.position;
                pos.y -= deathSinkSpeed * Time.deltaTime;
                transform.position = pos;
            }
        }
    }

    public void SetFlying(Vector3 direction)
    {
        int moveVec = Mathf.RoundToInt(Vector3.Dot(direction, transform.forward));
        animator.SetInteger("MoveVec", moveVec);
    }

    public void StartAttackAnimation()
    {
        Assert.IsTrue(CanAttack());
        animator.SetTrigger("Attack");
    }

    public void OnAttackCallback()
    {
        attacker.Explode();
    }

    public void OnDieCallback()
    {
        dead = true;
    }

    public bool CanFly()
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);
        return state.IsName("Flying Forward") || state.IsName("Flying Backward") || state.IsName("Idle");
    }

    private bool CanAttack()
    {
        return CanFly() && attacker.CanExplode();
    }
}
