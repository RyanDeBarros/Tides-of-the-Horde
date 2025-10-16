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
        public float value;
    }

    public string id;
    public string name;
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
        public int cost;
        public float value;
        public Action<float> onActivate;

        public void Activate()
        {
            onActivate.Invoke(value);
        }
    }

    private string id;
    private string name;
    private List<Tier> tiers;
    private int currentTier = -1;

    private readonly List<PlayerUnlockNode> preRequisites = new();
    private readonly List<PlayerUnlockNode> postRequisites = new();

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
        return tiers[currentTier].cost;
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

    public void LoadData(PlayerUnlockNodeData data, UnlockActionTable actionTable)
    {
        Assert.IsNotNull(data.id);
        Assert.IsNotNull(data.name);
        Assert.IsNotNull(data.action);
        Assert.IsNotNull(data.tiers);

        id = data.id;
        name = data.name;
        tiers = data.tiers.Select(tier => new Tier() {
            description = tier.description,
            cost = tier.cost,
            value = tier.value,
            onActivate = actionTable.GetAction(data.action, data.parameters)
        }).ToList();
        tiers.ForEach(tier => {
            Assert.IsNotNull(tier.description);
            Assert.IsNotNull(tier.onActivate);
        });
    }

    public void LoadRequisites(PlayerUnlockNodeData data, Dictionary<string, PlayerUnlockNode> nodes)
    {
        data.prerequisites.ForEach(prereq => {
            PlayerUnlockNode prerequisite = nodes[prereq];
            preRequisites.Add(prerequisite);
            prerequisite.postRequisites.Add(this);
        });
    }
}

public class PlayerUnlockTree : MonoBehaviour
{
    [SerializeField] private TextAsset upgradeTreeFile;

    private List<PlayerUnlockNode> nodes;

    private void Awake()
    {
        Assert.IsNotNull(upgradeTreeFile);
        LoadUnlockTree(upgradeTreeFile.text);
    }

    private void Start()
    {
        // Activate melee spell unlock on start
        nodes.Where(node => node.GetID() == "MeleeSpell-Unlock").First().Activate();
    }

    private void LoadUnlockTree(string json)
    {
        PlayerUnlockTreeData data = JsonUtility.FromJson<PlayerUnlockTreeData>(json);
        UnlockActionTable unlockActionTable = new();
        Dictionary<string, PlayerUnlockNode> nodeDictionary = new();

        // Load data
        data.nodes.ForEach(d => {
            PlayerUnlockNode node = new();
            node.LoadData(d, unlockActionTable);
            nodeDictionary[d.id] = node;
        });

        // Construct tree
        data.nodes.ForEach(d => {
            PlayerUnlockNode node = nodeDictionary[d.id];
            node.LoadRequisites(d, nodeDictionary);
        });

        // Unlock all initially unlockable upgrades
        nodes = nodeDictionary.Values.ToList();
        nodeDictionary.Values.ToList().ForEach(node => node.CheckPrerequisites());
    }

    public List<PlayerUnlockNode> GetRandomUnlocks(int count, int maxCost)
    {
        // TODO Get random (count - 1) non-health upgrades + next health upgrade if it exists else health refill
        // TODO Currency multiplier and currency bonus could be possible reward for NPC challenges
        return nodes
            .Where(node => node.CanActivate())
            .OrderBy(node => node.GetCost())
            .TakeWhile((node, index) => index < count || node.GetCost() <= maxCost)
            .ToList().GetRandomDistinctElements(count);
    }
}
