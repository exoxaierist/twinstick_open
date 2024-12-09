using System;
using TMPro;
using UnityEngine;

public class PerkDiscardHandler : PickerHandler 
{
    public Transform elementParent;
    public TextMeshProUGUI levelText;
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

    protected override void Select(int index)
    {
        index = Mathf.Clamp(index, 0, pickers.Count - 1);
        if (index == currentIndex) return;

        base.Select(index);
        levelText.text = ((PerkDiscardElement)pickers[index]).levelText;
    }
}
