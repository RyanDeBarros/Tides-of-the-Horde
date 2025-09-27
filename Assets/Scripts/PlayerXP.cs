using UnityEngine;
using UnityEngine.Events;

public class PlayerXP : MonoBehaviour
{
    [Min(0)] public int currentXP = 0;
    public UnityEvent<int> OnXPChanged;  

    public void AddXP(int amount)
    {
        if (amount <= 0) return;
        currentXP += amount;
        OnXPChanged?.Invoke(currentXP);
        // TODO: level up check?
        // if (currentXP >= nextLevelNeed) { LevelUp(); }
    }
}
