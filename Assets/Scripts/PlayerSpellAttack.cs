using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerSpellAttack : MonoBehaviour
{
    [SerializeField] private GameObject body;

    private List<ISpellCaster> spellCasters = new();

    private int activeSpellIndex = 1;
    private PlayerCamera cam;
    private Transform staffTip;

    private void Awake()
    {
        spellCasters.Add(GetComponentInChildren<MeleeSpellCaster>());
        spellCasters.Add(GetComponentInChildren<BombSpellCaster>());
        foreach (ISpellCaster spellCaster in spellCasters)
            Assert.IsNotNull(spellCaster);

        cam = GetComponent<PlayerCamera>();
        staffTip = FindStaffTip(transform);
        Assert.IsNotNull(body);
        Assert.IsNotNull(cam);
        Assert.IsNotNull(staffTip);
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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 0 = left click
        {
            Vector3 cameraDirection = cam.transform.forward;
            spellCasters[activeSpellIndex].CastSpell(body.transform.position, staffTip.position, body.transform.forward, new Vector3(cameraDirection.x, 0f, cameraDirection.z));
        }
    }

    public void SetActiveSpellIndex(int index)
    {
        activeSpellIndex = index;
    }
}
