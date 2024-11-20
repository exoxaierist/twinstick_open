using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemElement : PickerElement
{
    public string itemId;
    public Vector3 spawnPos;
    public bool isBought = false;

    private Perk perk;
    private Image image;

    public override void OnSpawn()
    {
        image = GetComponent<Image>();
        if (itemId == "ITEM_PERK")
        {
            if (perk != null) return;
            perk = Perk.Get(Perk.GetRandomID(Player.GetPerkIDList()));
            info.name = perk.name;
            info.description = perk.description;
            image.sprite = SpriteLib.Get(perk.ID);
            transform.localScale = new(1.5f, 1.5f, 1);
        }
        else
        {
            info.name = Locale.Get(itemId + "_NAME");
            info.description = Locale.Get(itemId + "_DESC");
            image.sprite = SpriteLib.Get(itemId);
            transform.localScale = new(1, 1, 1);
        }
        if (isBought) { transform.GetChild(0).gameObject.SetActive(true); }
    }

    public override bool CanConfirm()
    {
        if (Player.coinCount >= info.price && !isBought) return true;
        else return false;
    }

    public override void OnConfirm()
    {
        isBought = true;
        image.color = Color.gray;
        Player.coinCount -= info.price;
        if (perk != null) Item.SpawnPerk(perk, spawnPos, Vector2.down.Rotate(UnityEngine.Random.Range(-40,40)));
        else Item.Spawn(itemId, spawnPos, Vector2.down.Rotate(UnityEngine.Random.Range(-40, 40)));
    }
}
