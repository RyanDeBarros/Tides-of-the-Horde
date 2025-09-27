using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public UnityEvent OnDied;

    void Awake() => currentHP = Mathf.Max(1, maxHP);

    public void TakeDamage(int dmg)
    {
        if (currentHP <= 0) return;
        currentHP -= Mathf.Max(0, dmg);
        if (currentHP <= 0)
        {
            currentHP = 0;
            OnDied?.Invoke();
        }
    }
}
