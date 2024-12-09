public class HealItem : Item
{
    public int healAmount = 10;

    protected override void OnAcquire()
    {
        Player.main.hp.Heal(new() { damage = healAmount + PlayerStats.itemHealAmount, isHeal = true });
    }
}
