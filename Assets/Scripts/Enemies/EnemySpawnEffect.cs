using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnEffect : MonoBehaviour
{
    enum SpawnType
    {
        RiseFromGround,
        FlyDown
    }
    
    [SerializeField] private SpawnType spawnType = SpawnType.RiseFromGround;

    [SerializeField] private float duration = 1.2f;
    [SerializeField] private float heightOffset = 2f;

    private Vector3 startPos;
    private Vector3 endPos;
    private float timer;

    [Tooltip("These components will be disabled at spawn and re-enabled after the spawn animation.")]
    [SerializeField] private List<Behaviour> disabledComponents;

    private void Start()
    {
        foreach (var comp in disabledComponents)
        {
            if (comp != null)
                comp.enabled = false;
        }

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

    private void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        transform.position = Vector3.Lerp(startPos, endPos, t);

        if (t >= 1f)
            FinishSpawn();
    }

    private void FinishSpawn()
    {
        foreach (var comp in disabledComponents)
        {
            if (comp != null)
                comp.enabled = true;
        }

        Destroy(this);
    }
}
