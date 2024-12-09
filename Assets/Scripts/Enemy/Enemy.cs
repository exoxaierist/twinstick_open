using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hp))]
public class Enemy : MonoBehaviour
{
    public const string ENEMY_BAT = "ENEMY_BAT";
    public const string ENEMY_KNIGHT = "ENEMY_KNIGHT";
    public const string ENEMY_BULLET = "ENEMY_BULLET";
    public const string ENEMY_JUMPER = "ENEMY_JUMPER";
    public const string ENEMY_PED = "ENEMY_PED";
    public const string ENEMY_WANDER = "ENEMY_WANDER";
    public static void Spawn(Vector2 position, string id)
    {
        if (Physics2D.OverlapCircle(position, 0.4f, LayerMask.GetMask("WorldStatic","PawnBlock"))) return;
        LevelManager.currentRoom.enemyCount += 1;
        Utility.GetMono().Delay(UnityEngine.Random.Range(0f, 1f), () =>
        {
            EnemySpawnEffect.SpawnEnemy(position, ()=>SpawnImmediate(position, id));
            LevelManager.currentRoom.enemyCount -= 1;
        });
    }
    public static bool SpawnImmediate(Vector2 position, string id)
    {
        if (Physics2D.OverlapCircle(position, 0.4f, LayerMask.GetMask("WorldStatic","PawnBlock"))) return false;
        LevelManager.currentRoom.enemyCount += 1;
        SoundSystem.Play(SoundSystem.ENEMY_SPAWN, position, 0.4f);
        GameObject instance = Instantiate(EnemyList.Get(id));
        instance.gameObject.name = id;
        instance.transform.position = position;
        Enemy enemy = instance.GetComponent<Enemy>();
        enemy.OnSpawn();
        return true;
    }

    //components
    [HideInInspector] public VisualHandler visual;
    [HideInInspector] public Hp hp;
    [HideInInspector] public PathFinder nav;
    [HideInInspector] public Pawn pawn;

    public bool doContactDamage = true;
    private bool canContactDamage = true;
    public bool canBeHit = true;
    public float immuneAfterHit = 0.1f;
    public float knockBackAlpha = 1;
    public bool isDead = false;
    public bool flipSprite = false;
    protected bool hasLineOfSight = false;
    protected bool flinchOnHit = true;
    protected bool isActive = false; //to prevent attacking the moment they spawn
    public float activationDelay = 1;

    public MovementBehaviour moveBehaviour = MovementBehaviour.None;

    //interval timers;
    private float pathFindInterval = 0.5f;
    private float wanderDuration;
    private float wanderInterval;
    private float wanderDist;
    private Vector2 wanderDirection = Vector2.zero;
    private float intervalMin = 0.4f;
    private float intervalMax = 0.6f;
    private Coroutine intervalUpdateCoroutine;

    //attack infos
    protected AttackInfo attackInfo;
    protected AttackInfo contactAttackToPlayer;
    protected AttackInfo contactAttackToSelf;

    protected virtual void OnSpawn()
    {
        Initialize();
        SpawnEffect();
        this.Delay(activationDelay, () => { isActive = true; OnActivation(); });

        attackInfo = GetDefaultAttackInfo();
        contactAttackToPlayer = GetDefaultAttackInfo();
        contactAttackToPlayer.attackType = AttackType.Contact;

        contactAttackToSelf.attacker = Entity.Player;
        contactAttackToSelf.attackerName = Locale.Get("PLAYER");
        contactAttackToSelf.attackType = AttackType.Contact;

        hp.maxHealth = (int)(hp.maxHealth * PlayerStats.enemyHpMul);
        hp.health = hp.maxHealth;
    }

    protected virtual void OnActivation() { }

    protected virtual void OnIntervalUpdate() { }
    private IEnumerator IntervalUpdate()
    {
        while (!hp.isDead)
        {
            OnIntervalUpdate();
            yield return new WaitForSeconds(UnityEngine.Random.Range(intervalMin, intervalMax));
        }
    }

    protected void Initialize()
    {
        hp = GetComponent<Hp>();
        hp.onReceiveAttack += OnReceiveAttack;
        hp.onDeath += OnDeath;

        TryGetComponent(out pawn);
        TryGetComponent(out nav);

        visual = GetComponent<VisualHandler>();

        intervalUpdateCoroutine = StartCoroutine(IntervalUpdate());
    }

    public void SetMovementBehaviour(MovementBehaviour newBehaviour)
    {
        if (newBehaviour == moveBehaviour) return;
        if (newBehaviour == MovementBehaviour.Wander)
        {
        }
        else if (newBehaviour == MovementBehaviour.FollowPlayer)
        {
            if(nav!=null) nav.FindPath(Player.main.transform.position);
        }
        moveBehaviour = newBehaviour;
        pathFindInterval = 0.5f;
    }

    //called on update
    protected void Movement()
    {
        if (hp.isDead) return;
        if (moveBehaviour == MovementBehaviour.None) return;
        if (moveBehaviour == MovementBehaviour.Wander)
        {
            if(wanderDuration > 0)
            {
                wanderDuration -= Time.deltaTime;
                pawn.MoveInput(wanderDirection);
            }
            else if(wanderInterval > 0)
            {
                wanderInterval -= Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    wanderDirection = UnityEngine.Random.insideUnitCircle.normalized;
                    wanderDist = 1;
                    RaycastHit2D hit = Physics2D.CircleCast(transform.position, pawn.radius + 0.5f, wanderDirection, 10, LayerMask.GetMask("WorldStatic","PawnBlock"));
                    if (hit.distance > 1.5f)
                    {
                        wanderDist = UnityEngine.Random.Range(0.5f, hit.distance);
                    }
                }
                wanderDuration = Mathf.Min(2f, wanderDist / pawn.moveSpeed);
                wanderInterval = UnityEngine.Random.Range(0.5f, 2f);
            }
        }
        else if (moveBehaviour == MovementBehaviour.FollowPlayer)
        {
            if (nav == null) return;
            pathFindInterval -= Time.deltaTime;
            if (pathFindInterval <= 0)
            {
                nav.FindPath(Player.main.transform.position);
                pathFindInterval = UnityEngine.Random.Range(0.3f, 0.5f);
            }
            if(pawn!=null) pawn.MoveInput(nav.GetDirection());
        }
    }

    //called on update
    protected void SetSpriteDirection(SpriteDirMode mode)
    {
        if(mode == SpriteDirMode.FaceDirection)
        {
            if (pawn == null) return;
            if (pawn.unscaledVelocity.x < -0.1f) visual.sprite.flipX = flipSprite; //left
            else if (pawn.unscaledVelocity.x > 0.1f) visual.sprite.flipX = !flipSprite; //right
        }
        else if(mode == SpriteDirMode.FacePlayer)
        {
            float distToPlayer = transform.GetDirToPlayer().x;
            if (distToPlayer < -0.1f) visual.sprite.flipX = flipSprite;
            else if (distToPlayer > 0.1f) visual.sprite.flipX = !flipSprite;
        }
    }

    private void SpawnEffect()
    {
        //white effect
        visual.sprite.material.SetFloat("_Solidity", 1);
        DOTween.To(x => { if (visual != null) visual.sprite.material.SetFloat("_Solidity", x); }, 1, 0, 0.5f).SetDelay(0.1f);

        //squish effect
        visual.sprite.transform.DORewind();
        visual.sprite.transform.DOPunchScale(new(0.2f, 0.2f, 0), 0.3f);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (isDead || !doContactDamage || !collider.CompareTag("Player") || !canContactDamage) return;
        canContactDamage = false;
        this.Delay(0.5f, () => canContactDamage = true);

        Hp otherHp = collider.GetComponent<Hp>();
        contactAttackToPlayer.direction = collider.transform.position - transform.position;
        contactAttackToSelf.direction = transform.position - collider.transform.position;
        otherHp.Damage(contactAttackToPlayer);
        hp.Damage(contactAttackToSelf);
    }

    public void KnockBack(Vector2 direction)
    {
        pawn.AddForce(direction * knockBackAlpha);
    }

    protected virtual AttackInfo OnReceiveAttack(AttackInfo info)
    {
        if (!canBeHit) return new() { damage = 0 };

        //perk spike
        if(Player.HasPerk(Perk.PERK_SPIKE) && info.attackType == AttackType.Contact)
        {
            info.damage = 10 + (10 * Player.GetPerk(Perk.PERK_SPIKE).level);
        }

        //critical
        float random = UnityEngine.Random.Range(0, 1f);
        if (random <= PlayerStats.critChance)
        {
            info.damage = (int)(info.damage * PlayerStats.critMul);
            info.isCrit = true;
        }

        //audio
        SoundSystem.Play(SoundSystem.ACTION_HIT.GetRandom(), transform.position);

        //visual effects
        KnockBack(info.direction * info.knockBack);
        BloodParticleHandler.main.Emit(transform.position, 1, info.direction);
        Effect.Play("Slash0",
            EffectInfo.PosRotScale((Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 0.1f + Vector2.up * 0.3f
                , UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(0.2f, 0.8f))
            .SetLayer("Overlay")
            .SetColor(new(0.61f, 0.65f, 0.71f))
            );
        visual.sprite.HitEffect(flinchOnHit);

        return info;
    }

    protected virtual void OnDeath(AttackInfo info)
    {
        if (info.attacker == Entity.Player) Player.onEnemyKill?.Invoke();
        if (Player.HasPerk(Perk.PERK_EYEFOREYE))
        {
            float random = UnityEngine.Random.Range(0, 1f);
            if (random < PlayerStats.bananaChance) Item.Spawn("ITEM_BANANA", transform.position, UnityEngine.Random.insideUnitCircle);
        }

        StopCoroutine(intervalUpdateCoroutine);
        intervalUpdateCoroutine = null;

        gameObject.layer = LayerMask.NameToLayer("Ignore");
        visual.sprite.SetGray();
        pawn.accelRate = 30;
        pawn.AddForce(info.direction * 4);
        isDead = true;
        enabled = false;
        LevelManager.currentRoom.enemyCount -= 1;

        Utility.GetMono().Delay(0.5f, () =>
        {
            if(gameObject != null)
            {
                if(Random.Range(0,1f)<PlayerStats.coinChance) Coin.Spawn(transform.position);
                //SoundSystem.Play(SoundSystem.ENEMY_DEATH_01, transform.position);
                Effect.Play("Explosion2", EffectInfo.PosRotScale(transform.position + Vector3.up * 0.2f, 0, 0.7f).SetColor(Color.gray));
                Destroy(gameObject);
            }
        });
    }

    protected void CalcLineOfSight()
    {
        hasLineOfSight = HasLineOfSight();
    }
    protected bool HasLineOfSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.GetDirToPlayer(), 30, LayerMask.GetMask(new string[] { "Player", "WorldStatic" }));
        if (hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) return true;
        else return false;
    }

    protected AttackInfo GetDefaultAttackInfo()
    {
        return new()
        {
            attacker = Entity.Enemy,
            attackerName = gameObject.name,
            damage = Bullet.defaultDamage,
            knockBack = Bullet.defaultKnockBack,
            bulletSpeed = Bullet.enemyBulletSpeed * PlayerStats.enemyBulletSpeedMul,
        };
    }
}
