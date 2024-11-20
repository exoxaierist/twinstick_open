using UnityEngine;

public class ShopItemPerk : ShopItem
{
    [Header("Perk")]
    public string perkID;
    private Perk perk;

    private void Start()
    {
        perk = Perk.Get(perkID);
        info.name = perk.name;
        info.description = perk.description;
    }

    protected override void Buy()
    {
        if (Player.CanAddPerk(perk)) base.Buy();
        else CannotBuy();
    }

    protected override void OnBuy()
    {
        Player.AddPerk(perk);
    }
}
