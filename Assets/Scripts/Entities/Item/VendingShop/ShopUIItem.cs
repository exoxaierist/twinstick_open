using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIItem : MonoBehaviour
{
    public string itemName;
    public int price;
    public string description;

    public bool isBought = false;

    public virtual void Buy()
    {
        if (isBought) { CannotBuy(); return; }

        if (Player.coinCount >= price) OnBuy();
        else CannotBuy();
    }

    protected virtual void OnBuy()
    {
        isBought = true;
        UIManager.main.shopPopup.UpdateItem();
        Player.coinCount -= price;
    }

    protected virtual void CannotBuy()
    {
        transform.DORewind();
        transform.DOPunchRotation(new(0, 0, 7), 0.2f, 20);
        //transform.DOShakeRotation(0.2f, strength:10, vibrato:20);
    }
}
