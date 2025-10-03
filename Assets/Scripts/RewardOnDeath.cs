using UnityEngine;

[RequireComponent(typeof(Health))]
public class RewardOnDeath : MonoBehaviour
{
    [Min(1)] public int xpAmount = 10;
    [SerializeField] private string playerTag = "Player";

    private Health health;
    private PlayerXP playerXP;

    void Awake()
    {
        health = GetComponent<Health>();

        
        var playerGO = GameObject.FindWithTag(playerTag);
        if (playerGO != null)
            playerXP = playerGO.GetComponentInChildren<PlayerXP>();

        if (playerXP == null)
            Debug.LogWarning("[RewardOnDeath] PlayerXP not found on player.");
    }

    void OnEnable()
    {
        
        health.onDeath.AddListener(GrantXP);
    }

    void OnDisable()
    {
        health.onDeath.RemoveListener(GrantXP);
    }

    private void GrantXP()
    {
        if (playerXP != null)
        {
            playerXP.AddXP(xpAmount);
        }
        else
        {
            Debug.LogWarning("[RewardOnDeath] No PlayerXP to grant XP to.");
        }
        
    }
}
