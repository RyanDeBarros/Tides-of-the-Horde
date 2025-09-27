using UnityEngine;

[RequireComponent(typeof(Health))]
public class RewardOnDeath : MonoBehaviour
{
    [Min(1)] public int xpAmount = 10;
    public string playerTag = "Player";

    Health health;
    PlayerXP playerXP;

    void Awake()
    {
        health = GetComponent<Health>();

        
        var playerGO = GameObject.FindWithTag(playerTag);
        if (playerGO != null)
            playerXP = playerGO.GetComponentInChildren<PlayerXP>(); 

        if (playerXP == null)
            Debug.LogWarning("[RewardOnDeath] PlayerXP not found.");
    }

    void OnEnable() => health.OnDied.AddListener(GrantXP);
    void OnDisable() => health.OnDied.RemoveListener(GrantXP);

    void GrantXP()
    {
        if (playerXP != null)
            playerXP.AddXP(xpAmount);
        else
            Debug.LogWarning("[RewardOnDeath] No PlayerXP to grant.");

        
        Destroy(gameObject);
    }
}
