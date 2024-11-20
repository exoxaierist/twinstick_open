using UnityEngine;

public class PerkItem : Item
{
    public Perk perk;
    public SpriteRenderer perkImage;
    public void Set(Perk _perk)
    {
        perk = _perk;
        info.name = "PERK-" + perk.name;
        info.ID = perk.ID;
        info.description = perk.description;
        if (perkImage != null) perkImage.sprite = SpriteLib.Get(info.ID);
    }
    protected override void OnInteract()
    {
        if (Player.CanAddPerk(perk)) Acquire();
        else
        {
            UIManager.main.perkDiscardHandler.Open(() => Acquire());
        }
    }

    protected override void OnAcquire()
    {
        Player.AddPerk(perk);
    }
}
