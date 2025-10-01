using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerSpellAttack : MonoBehaviour
{
    [SerializeField] private GameObject body;

    private ISpellCaster activeSpell;
    private PlayerCamera cam;
    private Transform staffTip;

    private void Awake()
    {
        activeSpell = GetComponentInChildren<MeleeSpellCaster>();
        cam = GetComponent<PlayerCamera>();
        staffTip = FindStaffTip(transform);
    }

    private Transform FindStaffTip(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name == "StaffTip")
                return child;

            Transform grandchild = FindStaffTip(child);
            if (grandchild != null)
                return grandchild;
        }
        return null;
    }

    void Start()
    {
        Assert.IsNotNull(body);
        Assert.IsNotNull(activeSpell);
        Assert.IsNotNull(cam);
        Assert.IsNotNull(staffTip);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 0 = left click
        {
            activeSpell.CastSpell(body.transform.position, staffTip.position, body.transform.forward, cam.transform.forward);
        }
    }
}
