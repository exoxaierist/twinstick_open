using UnityEngine;

public class MushroomItem : Item
{
    protected override void OnAcquire()
    {
        if (Random.Range(0, 1f) > 0.5f)
        {
            Player.main.hp.Heal(new() { damage = Random.Range(1, 20) + PlayerStats.itemHealAmount, isHeal = true });

        }
        else
        {
            Player.main.hp.Damage(new() { damage = Mathf.Min(Player.main.hp.health - 1, Random.Range(1, 20)), isHeal = true });
        }
    }
}
