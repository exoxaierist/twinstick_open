using UnityEngine.UI;

public class PerkDiscardElement : PickerElement
{
    public Perk perk;
    public string levelText;
    public Image image;

    public override void OnSpawn()
    {
        info = new()
        {
            name = perk.name,
            description = perk.description,
        };
        image.sprite = SpriteLib.Get(perk.ID);
        image.color = ColorLib.lightBlueGray;
        levelText = perk.level + "/" + perk.maxLevel;
    }

    public override bool CanConfirm() => true;

    public override void OnConfirm()
    {
        Player.RemovePerk(perk);
        Item.SpawnPerk(perk, Player.main.transform.position, UnityEngine.Random.insideUnitCircle); 
    }
}
