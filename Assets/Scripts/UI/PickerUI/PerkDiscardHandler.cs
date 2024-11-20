using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkDiscardHandler : PickerHandler 
{
    public Transform elementParent;
    Action onFinish;

    public void Open(Action _onFinish)
    {
        onFinish = _onFinish;
        Open();
    }

    protected override void Confirm(PickerElement picker)
    {
        if (picker.CanConfirm())
        {
            picker.OnConfirm();
            onFinish?.Invoke();
            Close();
        }
        else
        {
            picker.Shake();
        }
    }

    protected override void CreateElements()
    {
        foreach (Perk perk in Player.perks)
        {
            PerkDiscardElement element = Instantiate(Prefab.Get("PerkDiscardElement")).GetComponent<PerkDiscardElement>();
            element.transform.SetParent(elementParent);
            element.perk = perk;
            pickers.Add(element);
            element.OnSpawn();
        }
    }
}
