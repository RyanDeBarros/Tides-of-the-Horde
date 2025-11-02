using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class PlayerUnlockNodeData
{
    [Serializable]
    public class Tier
    {
        public int cost;
        public string description;
        public string icon;
        public List<float> values;
        public float weight = 0f;
    }

    public string id;
    public string name;
    public string defaultIcon;
    public string defaultDescription;
    public float defaultWeight = 1f;
    public List<string> prerequisites;
    public string action;
    public List<string> parameters;
    public List<Tier> tiers;
}

[Serializable]
public class PlayerUnlockTreeData
{
    public List<PlayerUnlockNodeData> nodes;
}

public class PlayerUnlockNode
{
    private class Tier
    {
        public string description;
        public Texture iconTexture;
        public int cost;
        public float weight;
        public float[] values;
        public Action<float[]> onActivate;

        public void Activate()
        {
            onActivate.Invoke(values);
        }
    }

    private string id;
    private string name;
    private List<Tier> tiers;
    private int currentTier = -1;

    private readonly List<PlayerUnlockNode> preRequisites = new();
    private readonly List<PlayerUnlockNode> postRequisites = new();

    private readonly PlayerCurrency currency;

    public PlayerUnlockNode(PlayerCurrency currency)
    {
        this.currency = currency;
    }

    public string GetID()
    {
        return id;
    }

    public string GetName()
    {
        return name;
    }

    public string GetDescription()
    {
        return tiers[currentTier].description;
    }

    public int GetCost()
    {
        return currency.ShopPrice(tiers[currentTier].cost);
    }

    public float GetWeight()
    {
        return tiers[currentTier].weight;
    }

    public Texture GetIconTexture()
    {
        return tiers[currentTier].iconTexture;
    }

    public void Activate()
    {
        Assert.IsTrue(CanActivate());
        tiers[currentTier++].Activate();
        postRequisites.ForEach(postRequisite => { postRequisite.CheckPrerequisites(); });
    }

    public void CheckPrerequisites()
    {
        if (currentTier == -1 && preRequisites.All(preRequisite => preRequisite.currentTier > 0))
            currentTier = 0;
    }

    public bool CanActivate()
    {
        return currentTier >= 0 && currentTier < tiers.Count;
    }

    public void LoadData(PlayerUnlockTree unlocker, PlayerUnlockNodeData data, UnlockActionTable actionTable)
    {
        Assert.IsNotNull(data.id);
        Assert.IsNotNull(data.name);
        Assert.IsNotNull(data.action);
        Assert.IsNotNull(data.tiers);

        id = data.id;
        name = data.name;

        tiers = data.tiers.Select(tier => new Tier() {
            description = tier.description ?? data.defaultDescription,
            iconTexture = unlocker.GetIconTexture(tier.icon ?? data.defaultIcon),
            cost = tier.cost,
            weight = tier.weight > 0f ? tier.weight : data.defaultWeight,
            values = tier.values.ToArray(),
            onActivate = actionTable.GetAction(data.action, data.parameters)
        }).ToList();
        
        tiers.ForEach(tier => {
            Assert.IsNotNull(tier.description);
            Assert.IsNotNull(tier.onActivate);
        });
    }

    public void LoadRequisites(PlayerUnlockNodeData data, Dictionary<string, PlayerUnlockNode> nodes)
    {
        data.prerequisites?.ForEach(prereq => {
            PlayerUnlockNode prerequisite = nodes[prereq];
            preRequisites.Add(prerequisite);
            prerequisite.postRequisites.Add(this);
        });
    }
}

public class PlayerUnlockTree : MonoBehaviour
{
    [SerializeField] private TextAsset upgradeTreeFile;
    [SerializeField] private string unlockMeleeID = "MeleeSpell-Unlock";
    [SerializeField] private string upgradeHealthID = "Health-Upgrade";
    [SerializeField] private DashCooldownUI dashCooldownUI;

    [Header("Icons")]
    [SerializeField] private Texture meleeSpellIcon;
    [SerializeField] private Texture bombSpellIcon;
    [SerializeField] private Texture bubbleSpellIcon;
    [SerializeField] private Texture sniperSpellIcon;
    [SerializeField] private Texture healthIcon;
    [SerializeField] private Texture dashIcon;

    private readonly Dictionary<string, PlayerUnlockNode> nodes = new();

    private void Awake()
    {
        Assert.IsNotNull(dashCooldownUI);

        Assert.IsNotNull(upgradeTreeFile);
        LoadUnlockTree(upgradeTreeFile.text);
    }

    private void Start()
    {
        // Activate melee spell unlock on start
        nodes[unlockMeleeID].Activate();
    }

    private void LoadUnlockTree(string json)
    {
        PlayerUnlockTreeData data = JsonUtility.FromJson<PlayerUnlockTreeData>(json);
        UnlockActionTable unlockActionTable = new(gameObject, dashCooldownUI);

        PlayerCurrency playerCurrency = FindObjectsByType<PlayerCurrency>(FindObjectsSortMode.None).GetUniqueElement();

        // Load data
        data.nodes.ForEach(d => {
            PlayerUnlockNode node = new(playerCurrency);
            node.LoadData(this, d, unlockActionTable);
            nodes[d.id] = node;
        });

        // Construct tree
        data.nodes.ForEach(d => {
            PlayerUnlockNode node = nodes[d.id];
            node.LoadRequisites(d, nodes);
        });

        // Unlock all initially unlockable upgrades
        nodes.Values.ToList().ForEach(node => node.CheckPrerequisites());
    }

    public List<PlayerUnlockNode> GetRandomUnlocks(int count, int maxCost)
    {
        Assert.IsTrue(count > 0);
        List<PlayerUnlockNode> randomUnlocks = new();
        PlayerUnlockNode healthUpgrade = nodes[upgradeHealthID];

        // Non-health upgrades
        if (healthUpgrade.CanActivate())
            --count;

        if (count > 0)
        {
            var unlocks = nodes.Values
                .Where(node => node.CanActivate() && node.GetID() != upgradeHealthID)
                .OrderBy(node => node.GetCost())
                .TakeWhile((node, index) => index < count || node.GetCost() <= maxCost);

            randomUnlocks.AddRange(unlocks.ToList().GetWeightedRandomDistinctElements(count, unlocks.Select(unlock => unlock.GetWeight()).ToList()));
        }

        // Health upgrade
        if (healthUpgrade.CanActivate())
            randomUnlocks.Add(healthUpgrade);

        return randomUnlocks;
    }

    public Texture GetIconTexture(string icon)
    {
        return icon switch
        {
            "MeleeSpell" => meleeSpellIcon,
            "BombSpell" => bombSpellIcon,
            "BubbleSpell" => bubbleSpellIcon,
            "SniperSpell" => sniperSpellIcon,
            "Health" => healthIcon,
            "Dash" => dashIcon,
            _ => throw new NotImplementedException(),
        };
    }
}
