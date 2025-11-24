using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnEffect : MonoBehaviour
{
    public enum SpawnType { RiseFromGround, FlyDown }
    public SpawnType spawnType = SpawnType.RiseFromGround;

    public float duration = 1.2f;
    public float heightOffset = 2f;

    private Vector3 startPos;
    private Vector3 endPos;
    private float timer;

    [Tooltip("These components will be disabled at spawn and re-enabled after the spawn animation.")]
    [SerializeField] private List<Behaviour> disabledComponents;

    void Awake()
    {
        foreach (var comp in disabledComponents)
        {
            if (comp != null)
                comp.enabled = false;
        }
    }

    void Start()
    {
        endPos = transform.position;

        switch (spawnType)
        {
            case SpawnType.RiseFromGround:
                startPos = endPos - new Vector3(0, heightOffset, 0);
                break;

            case SpawnType.FlyDown:
                startPos = endPos + new Vector3(0, heightOffset, 0);
                break;
        }

        transform.position = startPos;

    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        switch (spawnType)
        {
            case SpawnType.RiseFromGround:
            case SpawnType.FlyDown:
                transform.position = Vector3.Lerp(startPos, endPos, t);
                break;
        }

        if (t >= 1f)
            FinishSpawn();
    }

    void FinishSpawn()
    {

        foreach (var comp in disabledComponents)
        {
            if (comp != null)
                comp.enabled = true;
        }

        Destroy(this);
    }
}
