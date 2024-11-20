using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopPopup : Selectable, IMoveHandler, ISubmitHandler, ICancelHandler
{
    public Transform itemLayout;
    public Transform selection;
    public GameObject boughtOverlay;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI itemDescription;

    private VendingMachine caller;
    private List<ShopUIItem> items = new();
    public int currentIndex = -1;

    public void Show(VendingMachine _caller)
    {
        gameObject.SetActive(true);
        caller = _caller;
        //items.AddRange(_caller.items);
        foreach (ShopUIItem item in items)
        {
            item.transform.SetParent(itemLayout);
            item.gameObject.SetActive(true);
        }

        currentIndex = -1;
        
        this.DelayFrame(()=> {
            Select(0);
            UpdateItem();
        });
    }

    public void Hide()
    {
        items.Clear();
        selection.gameObject.SetActive(false);
        gameObject.SetActive(false);
        Player.main.Sleep(false);

        caller.OnInteractEnd();
    }

    public void Select(int index)
    {
        index = (index + items.Count) % items.Count;
        if (currentIndex == index) return;
        selection.gameObject.SetActive(true);

        selection.SetParent(items[index].transform);
        selection.localPosition = Vector3.zero;

        SetDescription(items[index].itemName, items[index].price.ToString(), items[index].description);

        currentIndex = index;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        items[currentIndex].Buy();
    }

    public void OnCancel(BaseEventData eventData)
    {
        Hide();
    }

    void IMoveHandler.OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Right) Select(currentIndex + 1);
        else if (eventData.moveDir == MoveDirection.Left) Select(currentIndex - 1);
    }

    public void SetDescription(string name, string price, string desc)
    {
        itemName.text = name;
        itemPrice.text = price;
        itemDescription.text = desc;
    }

    public void UpdateItem()
    {
        selection.SetParent(transform);
        foreach (ShopUIItem item in items)
        {
            if (item.isBought)
            {
                if(item.transform.childCount==0)
                {
                    GameObject instance = Instantiate(boughtOverlay);
                    instance.transform.SetParent(item.transform);
                    instance.transform.localPosition = Vector3.zero;
                }
                item.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                if (item.transform.childCount > 0)
                {
                    Destroy(item.transform.GetChild(0).gameObject);
                }
                item.GetComponent<Image>().color = Color.white;
            }
        }
        selection.SetParent(items[currentIndex].transform);
        selection.localPosition = Vector3.zero;
    }
}
