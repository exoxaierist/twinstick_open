using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverlayManager : MonoBehaviour
{
    private GameObjectPool perkUIPool = new("PerkUI");
    private GameObjectPool perkUIEmptyPool = new("PerkUIEmpty");
    private GameObjectPool perkUILockedPool = new("PerkUILocked");

    private List<PerkUI> perkUIList = new();
    private List<GameObject> perkUIEmptyList = new();
    private List<GameObject> perkUILockedList = new();

    public Transform perkUIParent;

    public Transform perkInfoBox;
    public TextMeshProUGUI perkName;
    public TextMeshProUGUI perkDesc;
    public TextMeshProUGUI perkLevel;

    public void UpdatePerks()
    {
        //clear existing perk UIs
        if (perkUIList.Count > 0)
        {
            for (int i = 0; i < perkUIList.Count; i++)
            {
                perkUIPool.Release(perkUIList[i].gameObject);
            }
            perkUIList.Clear();
        }
        if (perkUIEmptyList.Count > 0)
        {
            for (int i = 0; i < perkUIEmptyList.Count; i++)
            {
                perkUIEmptyPool.Release(perkUIEmptyList[i].gameObject);
            }
            perkUIEmptyList.Clear();
        }
        if (perkUILockedList.Count > 0)
        {
            for (int i = 0; i < perkUILockedList.Count; i++)
            {
                perkUILockedPool.Release(perkUILockedList[i].gameObject);
            }
            perkUILockedList.Clear();
        }

        //create new perk UIs
        for (int i = 0; i < Player.perks.Count; i++)
        {
            PerkUI ui = perkUIPool.Get().GetComponent<PerkUI>();
            ui.transform.SetParent(transform);
            ui.transform.localScale = Vector3.one;
            ui.Set(Player.perks[i],this);
            perkUIList.Add(ui);

            if (i == 0) EventSystem.current.SetSelectedGameObject(ui.gameObject);
        }
        for (int i = Player.perks.Count; i < Player.maxPerkCount; i++)
        {
            GameObject instance = perkUIEmptyPool.Get();
            instance.transform.SetParent(transform);
            instance.transform.localScale = Vector3.one;
            perkUIEmptyList.Add(instance);
        }
        for (int i = Player.maxPerkCount; i < Player.maxMaxPerkCount; i++)
        {
            GameObject instance = perkUILockedPool.Get();
            instance.transform.SetParent(transform);
            instance.transform.localScale = Vector3.one;
            perkUILockedList.Add(instance);
        }
        perkInfoBox.transform.SetAsLastSibling();
    }
}
