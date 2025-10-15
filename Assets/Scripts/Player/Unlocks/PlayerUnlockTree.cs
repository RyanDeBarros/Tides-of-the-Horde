using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class PlayerUnlockNodeData
{
    public string id;
    public string name;
    public string description;
    public int cost;
    public List<string> preRequisiteIds;
    public string onActivateID;
}

[Serializable]
public class PlayerUnlockTreeData
{
    public List<PlayerUnlockNodeData> nodes;
    public List<UnlockActionData> actionData;
}

public class PlayerUnlockNode
{
    private string name;
    private string description;
    private int cost;
    private bool unlocked = false;
    private bool activated = false;

    private Action onActivate;

    private readonly List<PlayerUnlockNode> preRequisites = new();
    private readonly List<PlayerUnlockNode> postRequisites = new();

    public string GetName()
    {
        return name;
    }

    public string GetDescription()
    {
        return description;
    }

    public int GetCost()
    {
        return cost;
    }

    public void Activate()
    {
        Assert.IsTrue(CanActivate());
        onActivate.Invoke();
        activated = true;
        UnlockInGraph();
    }

    public void UnlockInGraph()
    {
        unlocked = true;
        postRequisites.ForEach(postRequisite => { postRequisite.CheckForUnlock(); });
    }

    private void CheckForUnlock()
    {
        if (!unlocked && preRequisites.All(preRequisite => preRequisite.activated))
            UnlockInGraph();
    }

    public bool IsUnlocked()
    {
        return unlocked;
    }

    public bool IsActivated()
    {
        return activated;
    }

    public bool CanActivate()
    {
        return unlocked && !activated;
    }

    public void LoadData(PlayerUnlockNodeData data, UnlockActionTable actionTable)
    {
        Assert.IsNotNull(data.name);
        Assert.IsNotNull(data.description);
        Assert.IsNotNull(data.onActivateID);
        name = data.name;
        description = data.description;
        cost = data.cost;
        onActivate = actionTable.GetAction(data.onActivateID);
        Assert.IsNotNull(onActivate);
    }

    public void LoadRequisites(PlayerUnlockNodeData data, Dictionary<string, PlayerUnlockNode> nodes)
    {
        data.preRequisiteIds.ForEach(preRequisiteId => {
            PlayerUnlockNode preRequisite = nodes[preRequisiteId];
            preRequisites.Add(preRequisite);
            preRequisite.postRequisites.Add(this);
        });
    }
}

public class PlayerUnlockTree : MonoBehaviour
{
    [SerializeField] private TextAsset upgradeTreeFile;

    private readonly Dictionary<string, PlayerUnlockNode> nodes = new();
    private readonly UnlockActionTable unlockActionTable = new();

    private void Awake()
    {
        Assert.IsNotNull(upgradeTreeFile);
    }

    private void Start()
    {
        LoadUnlockTree(upgradeTreeFile.text);
    }

    private void LoadUnlockTree(string json)
    {
        PlayerUnlockTreeData data = JsonUtility.FromJson<PlayerUnlockTreeData>(json);
        unlockActionTable.Load(data.actionData);

        // Load data
        data.nodes.ForEach(d => {
            PlayerUnlockNode node = new();
            node.LoadData(d, unlockActionTable);
            nodes[d.id] = node;
        });

        // Construct tree
        data.nodes.ForEach(d => {
            PlayerUnlockNode node = nodes[d.id];
            node.LoadRequisites(d, nodes);
        });

        // Unlock initial ability - should be "MeleeSpell-Unlock"
        string rootID = data.nodes[0].id;
        Debug.Log($"Root ID in player unlock tree is \"{rootID}\". Expecting \"MeleeSpell-Unlock\".");
        PlayerUnlockNode root = nodes[rootID];
        root.UnlockInGraph();
        root.Activate();
    }

    public List<PlayerUnlockNode> GetRandomUnlocks(int count, int maxCost)
    {
        // TODO Get random (count - 1) non-health upgrades + next health upgrade if it exists else health refill
        // TODO Currency multiplier and currency bonus could be possible reward for NPC challenges
        return nodes.Values
            .Where(node => node.CanActivate())
            .OrderBy(node => node.GetCost())
            .TakeWhile((node, index) => index < count || node.GetCost() <= maxCost)
            .ToList().GetRandomDistinctElements(count);
    }
}
