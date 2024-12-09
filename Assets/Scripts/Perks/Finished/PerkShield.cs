using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PerkShield : Perk
{
    public PerkShield()
    {
        ID = PERK_SHIELD;
        level = 1;
        maxLevel = 4;
        OnInstantiate();
    }

    private List<GameObject> shields = new(); 

    public override void OnFirstActive()
    {
        if (Player.main == null) Player.onPlayerSpawn += Activate;
        else Activate();
    }

    public override void OnLevelUp()
    {
        if (Player.main == null) Player.onPlayerSpawn += RefreshShield;
        else RefreshShield();
    }

    private void RefreshShield()
    {
        Player.onPlayerSpawn -= RefreshShield;
        RemoveShields();
        CreateShields();
    }

    public override void OnDiscard()
    {
        RemoveShields();
    }

    private void Activate()
    {
        CreateShields();
        Player.onPlayerSpawn -= Activate;
    }

    private void RemoveShields()
    {
        for (int i = 0; i < shields.Count; i++)
        {
            shields[i].transform.DOKill();
            Object.Destroy(shields[i]);
        }
        shields.Clear();
    }

    private void CreateShields()
    {
        float angleOffset = 0;
        for (int i = 0; i < level; i++)
        {
            GameObject instance = Object.Instantiate(Prefab.Get("PerkShieldObject"));
            instance.transform.SetParent(Player.main.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.Euler(0, 0, angleOffset);
            instance.transform.DOLocalRotate(new(0, 0, 360), 5, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            shields.Add(instance);
            angleOffset += 360f / level;
        }
    }
}
