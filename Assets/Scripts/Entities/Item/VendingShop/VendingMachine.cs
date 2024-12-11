using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour, IInteractable
{
    private bool beingUsed = false;
    private InteractPopup popup;
    public List<ShopItemElement> items = new();

    private void Start()
    {
        if (items.Count >0 ) return;
        int itemCount = Random.Range(3, 6);
        for (int i = 0; i < itemCount; i++)
        {
            ShopItemElement instance = Instantiate(Prefab.Get("ShopElement")).GetComponent<ShopItemElement>();
            instance.transform.SetParent(transform);
            instance.gameObject.SetActive(false);
            instance.itemId = ItemList.GetRandomId();
            items.Add(instance);
        }
    }

    public bool IsInteractable() => !beingUsed;
    public void InspectStart()
    {
        if (beingUsed) return;
        ShowPopup();
    }
    public void InspectEnd() => HidePopup();
    public void Interact()
    {
        beingUsed = true;
        HidePopup();
        UIManager.main.shopHandler.Open(this);
    }
    public void OnInteractEnd()
    {
        foreach (ShopItemElement item in items)
        {
            item.transform.SetParent(transform);
            item.gameObject.SetActive(false);
        }
        beingUsed = false;
    }

    private void ShowPopup()
    {
        if (popup != null) HidePopup();
        popup = InteractPopup.Get(Locale.Get("MISC_USE"), transform.position, Vector3.zero);
    }
    private void HidePopup()
    {
        if (popup == null) return;
        InteractPopup.Release(popup);
        popup = null;
    }
}
