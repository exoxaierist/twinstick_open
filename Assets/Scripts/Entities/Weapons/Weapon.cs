using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public bool isEnemy = false;

    [Header("Orbit")]
    public Vector2 orbitOffset;
    public float orbitDistance = 0.5f;

    private Transform visual;

    [Header("Attack Params")]
    [SerializeField] private float fireInterval = 0.2f;
    [SerializeField] public float autoFirePenalty = 0f;
    private float penaltyTimer;
    private bool canShoot = true;
    public float baseCritPercent = 5;

    //reload
    public int baseMagSize = 10;
    private int _magSize = 10;
    public int magSize
    {
        get { return _magSize; }
        set
        {
            UIManager.main.ammoCounter.SetMagazineCount(value);
            _magSize = value;
        }
    }
    private int _ammoCount;
    public int ammoCount
    {
        get { return _ammoCount; }
        set 
        { 
            if (value != _ammoCount) UIManager.main.ammoCounter.SetCount(value);
            _ammoCount = value;
        }
    }
    public float reloadDuration = 0.5f;
    public bool isReloading = false;
    public bool canReload = false;
    private float reloadProgress = 0;

    private void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        visual = transform.Find("VISUAL");
        ammoCount = _magSize;
    }

    public void Attack(AttackInput context) 
    {
        if (!canShoot) return;
        if (context.isStarted || (context.isPressing && penaltyTimer <= 0))
        {
            AttackProcess(context);

            canShoot = false;
            penaltyTimer = autoFirePenalty;
            this.Delay(fireInterval * (1/PlayerStats.attackSpeed), () => canShoot = true);
        }
        else if (context.isPressing)
        {
            penaltyTimer -= Time.deltaTime;
        }
    }

    private void AttackProcess(AttackInput context)
    {
        if (isReloading)
        {
            Player.main.ShowHeadUpText(Locale.Get("MISC_RELOADING"));
            return;
        }

        AttackInfo info = Player.main.attackInfo.Direction(context.direction);
        info.bulletAdditionalCost = -1;
        List<AttackInfo> list = new();

        //xl
        if (Player.HasPerk(Perk.PERK_XL))
        {
            info.bulletType = BulletType.Large;
        }

        //blind shooter
        if (Player.HasPerk(Perk.PERK_BLINDSHOOTER))
        {
            AttackInfo instance = info;
            for (int i = 0; i < Player.GetPerk(Perk.PERK_BLINDSHOOTER).level; i++)
            {
                instance.direction = info.direction.Rotate(15 * i + 12);
                list.Add(instance);
                instance.direction = info.direction.Rotate(-(15 * i + 12));
                list.Add(instance);
            }
        }
        else list.Add(info);

        //cover six
        if (Player.HasPerk(Perk.PERK_COVERSIX))
        {
            AttackInfo copy = info;
            copy.direction *= -1;
            list.Add(copy);
        }

        //radial
        if (Player.HasPerk(Perk.PERK_RADIAL))
        {
            int radialCount = 3 + Player.GetPerk(Perk.PERK_RADIAL).level;
            float radialTheta = 360f / radialCount;
            float radialAngle = 0;
            for (int i = 0; i < radialCount; i++)
            {
                AttackInfo copy = info;
                copy.direction = Vector2.up.Rotate(radialAngle);
                list.Add(copy);
                radialAngle += radialTheta;
            }
        }

        //add cost of 1 for entire list
        AttackInfo temp = list[0];
        temp.bulletAdditionalCost = 0;
        list[0] = temp;

        foreach (AttackInfo item in list)
        {
            AttackAction(item);
        }

        SoundSystem.Play(SoundSystem.ACTION_SHOOT.GetRandom(),Player.main.transform.position,0.5f);
        CamController.main.Shake(0.05f);
    }
    protected virtual void AttackAction(AttackInfo info) { }

    public void SetOrbitTransform(Vector2 direction)
    {
        direction.Normalize();
        transform.SetLocalPositionAndRotation(direction * orbitDistance + orbitOffset, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction)));
        if (direction.x < -0.1f) transform.localScale = new(1, -1, 1);
        if (direction.x > 0.1f) transform.localScale = new(1, 1, 1);
    }

    public void StartReload()
    {
        if (isReloading || ammoCount == _magSize) return;
        SoundSystem.Play(SoundSystem.PLAYER_RELOAD_START, transform.position,0.1f);
        isReloading = true;
        DOTween.To(() => reloadProgress, x => { reloadProgress = x; UIManager.main.SetReloadProgress(x); }, 1, reloadDuration * PlayerStats.reloadDuration)
            .SetEase(Ease.InOutCirc)
            .OnComplete(() =>
            {
                reloadProgress = 0;
                isReloading = false;
                SoundSystem.Play(SoundSystem.PLAYER_RELOAD_END, transform.position,0.1f);
                this.Delay(0.1f, () => UIManager.main.HideReloadProgress());
                OnReloadComplete();
            });
    }
    protected virtual void OnReloadComplete()
    {
        ammoCount = magSize;
    }
}
