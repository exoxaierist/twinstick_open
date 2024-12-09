using UnityEngine;

public class BulletWeapon : Weapon
{
    [Header("Muzzle")]
    public bool hasMuzzle = false;
    public Vector2 muzzleOffset;

    [Header("Bullet Info")]
    public GameObject bullet;
    public float bulletSpread;

    private void Reset()
    {
        hasMuzzle = true;
    }

    protected override void AttackAction(AttackInfo info)
    {
        if (isReloading) return;

        info.direction = info.direction.Rotate(Random.Range(-PlayerStats.attackAccuracy,PlayerStats.attackAccuracy)*0.5f);
        Vector2 pos = transform.position; ;
        Bullet.Fire(pos, info);
        ammoCount -= 1+info.bulletAdditionalCost;
        Effect.PlayColored(!isEnemy, "MuzzleFlash",
            EffectInfo.Pos(pos))
            .transform.SetParent(transform.parent);

        OnShoot();

        if (ammoCount == 0) this.DelayFrame(() => StartReload());
    }

    protected virtual void OnShoot()
    {
        
    }
}
