using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class PlayerUnlockNodeData
{
    public string id;
    public string name;  // TODO add description string here or in action data
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

    public int GetCost()
    {
        return cost;
    }

    public void Activate()
    {
        Assert.IsTrue(CanActivate());
        onActivate.Invoke();
        activated = true;
        UnlockInChain();
    }

    public void UnlockInChain()
    {
        unlocked = true;
        postRequisites.ForEach(postRequisite => { postRequisite.CheckForUnlock(); });
    }

    private void CheckForUnlock()
    {
        if (!unlocked && preRequisites.All(preRequisite => preRequisite.unlocked))
            UnlockInChain();
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
        Assert.IsNotNull(data.onActivateID);
        name = data.name;
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
    private PlayerUnlockNode unlockTreeRoot;

    private readonly UnlockActionTable unlockActionTable = new();

    private void Awake()
    {
        Assert.IsNotNull(upgradeTreeFile);
        LoadUnlockTree(upgradeTreeFile.text);
        unlockTreeRoot.UnlockInChain();
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

        unlockTreeRoot = nodes[data.nodes[0].id];
    }
}
