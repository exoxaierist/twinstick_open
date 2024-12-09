using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static GameObjectPool pool = new("Bullet");
    public static Bullet Fire(Vector2 position, AttackInfo info, Action<BulletHit> onHit = null, Action<BulletFinish> onFinish = null)
    {
        Bullet instance = SpawnBullet(position, info,onHit,onFinish);

        info.damage = (int)(info.damage * 0.5f);
        for (int i = 0; i < info.additionalBurst; i++)
        {
            info.bulletType = BulletType.Small;
            info.knockBack = 0;
            SoundSystem.Play(SoundSystem.ACTION_SHOOT.GetRandom(), position, 0.3f);
            Utility.GetMono().Delay(0.1f * i + 0.1f, () => SpawnBullet(position, info));
        }
        return instance;
    }
    private static Bullet SpawnBullet(Vector2 position, AttackInfo info, Action<BulletHit> onHit = null, Action<BulletFinish> onFinish = null)
    {
        Bullet instance = pool.Get().GetComponent<Bullet>();
        instance.transform.position = position - (Vector2)instance.child.localPosition;
        instance.Initialize(info, onHit, onFinish);
        return instance;
    }

    public static int defaultDamage = 10;
    public static float defaultKnockBack = 1;
    public static float enemyBulletSpeed = 5.5f;
    public static float enemyBulletSpeedFast = 20;

    private SpriteRenderer sprite;
    private Transform child;
    public Material playerMat;
    public Material enemyMat;
    public Sprite normalSprite;
    public Sprite smallSprite;
    public Sprite trackingSprite;
    public Sprite fireSprite;
    public Sprite largeSprite;

    private bool isEnemy = false;
    private float bulletRadius = 0.35f;
    private float bulletSpeed = 10;
    private float maxDistance = 10;
    private float distanceTraveled = 0;
    private int baseDamage;
    protected AttackInfo attackInfo;
    private Action<BulletHit> onHit;
    private Action<BulletFinish> onFinish;
    private List<GameObject> ignore = new();

    private Vector2 direction;
    private LayerMask entityMask;
    private LayerMask collisionMask;
    private float randomPhase;

    private void Awake()
    {
        child = transform.GetChild(0);
        sprite = GetComponentInChildren<SpriteRenderer>();
        collisionMask = LayerMask.GetMask("WorldStatic");
    }

    private void Update()
    {
        if(distanceTraveled>maxDistance)
        {
            //SoundSystem.Play(SoundSystem.ACTION_HIT_SECONDARY.GetRandom(), transform.position, 0.3f);
            PlayEffect(new(0, -0.5f));
            onFinish?.Invoke(new() { direction = direction, position = child.transform.position });
            pool.Release(gameObject);
            return;
        }
        BulletHit hitContext;

        if(attackInfo.isHoming) direction = Homing(direction);
        if (attackInfo.doBend) direction = direction.Rotate(attackInfo.bendStrength*Time.deltaTime);
        Vector2 frameDir = direction;
        if (attackInfo.isStoned) frameDir = frameDir.Rotate(Mathf.Sin(Time.time * 15 *(1/attackInfo.stoneStrength) + randomPhase) * 40 *attackInfo.stoneStrength);

        Vector2 frameMovement = bulletSpeed * Time.deltaTime * frameDir;
        distanceTraveled += frameMovement.magnitude;
        hitContext = CheckHit(frameMovement);
        RotateToDirection(frameMovement);
        if (!hitContext.isHit)
        {
            //doesn't hit anything
            transform.Translate(frameMovement);
        }
        else if (hitContext.type == HitColliderType.Entity)
        {
            //hits entity
            //aging damage calc
            if(attackInfo.attacker == Entity.Player && Player.HasPerk(Perk.PERK_AGING))
            {
                attackInfo.damage = baseDamage + (int)Mathf.Lerp(0, (Player.GetPerk(Perk.PERK_AGING) as PerkAging).maxDamage, distanceTraveled / maxDistance);
            }
            if(hitContext.hp!=null) hitContext.hp.Damage(attackInfo);
            transform.Translate((hitContext.hitInfo.point - (Vector2)child.position) + frameDir*0.3f);
            ChainReaction(hitContext.hitInfo.collider); 

            SoundSystem.Play(SoundSystem.ACTION_HIT_SECONDARY.GetRandom(), transform.position, 0.2f);
            PlayEffect(new(0, -0.5f));

            if (!attackInfo.penetrate) 
            { 
                //explosion perk
                if(attackInfo.attacker == Entity.Player 
                    && Player.HasPerk(Perk.PERK_EXPLOSION)
                    && UnityEngine.Random.Range(0,1f) < ((PerkExplosion)Player.GetPerk(Perk.PERK_EXPLOSION)).explosionChance)
                {
                    Explosion.Explode(hitContext.hitInfo.point, 1.2f, attackInfo);
                }
                onHit?.Invoke(hitContext); 
                pool.Release(gameObject); 
            }
            else ignore.Add(hitContext.hitInfo.collider.gameObject);
        }
        else if (hitContext.type == HitColliderType.Wall)
        {
            //hits wall
            PlayEffect(new(0, UnityEngine.Random.Range(0, -0.2f)-child.localPosition.y));
            SoundSystem.Play(SoundSystem.ACTION_HIT_SECONDARY.GetRandom(), transform.position, 0.2f);
            if (attackInfo.bounceCount > 0)
            {
                ignore.Clear();
                attackInfo.bounceCount--;
                distanceTraveled = 0;
                direction = Vector2.Reflect(direction, hitContext.hitInfo.normal);
            }
            else {
                //explosion perk
                if (attackInfo.attacker == Entity.Player
                    && Player.HasPerk(Perk.PERK_EXPLOSION)
                    && UnityEngine.Random.Range(0, 1f) < ((PerkExplosion)Player.GetPerk(Perk.PERK_EXPLOSION)).explosionChance)
                {
                    Explosion.Explode(hitContext.hitInfo.point, 1.2f, attackInfo);
                }
                onHit?.Invoke(hitContext); 
                pool.Release(gameObject); 
            }
        }
    }

    public void Initialize(AttackInfo _info, Action<BulletHit> _onHit = null, Action<BulletFinish> _onFinish = null)
    {
        attackInfo = _info;
        bulletSpeed = _info.bulletSpeed;
        isEnemy = _info.attacker==Entity.Enemy;
        maxDistance = _info.bulletMaxDist;
        distanceTraveled = 0;
        randomPhase = UnityEngine.Random.Range(0, 100f);
        direction = attackInfo.direction;
        onHit = _onHit;
        onFinish = _onFinish;
        baseDamage = _info.damage;
        ignore.Clear();

        if (isEnemy)
        {
            sprite.material = enemyMat;
            entityMask = LayerMask.GetMask("Player","PlayerShield");
        }
        else
        {
            sprite.material = playerMat;
            entityMask = LayerMask.GetMask("Enemy");
        }
        if (attackInfo.bulletType == BulletType.Normal) { sprite.sprite = normalSprite; bulletRadius = 0.35f; }
        else if (attackInfo.bulletType == BulletType.Small) {sprite.sprite = smallSprite; bulletRadius = 0.24f; }
        else if(attackInfo.bulletType == BulletType.Tracking) { sprite.sprite = trackingSprite; }
        else if(attackInfo.bulletType == BulletType.Fire) { sprite.sprite = fireSprite; }
        else if(attackInfo.bulletType == BulletType.Large) { sprite.sprite = largeSprite; }
    }

    protected BulletHit CheckHit(Vector2 frameMovement)
    {
        BulletHit hitContext = new() { isHit = false, dist = 10000};
        BulletHit hitContextCol = new() { isHit = false, dist = 10000 };
        //hit
        RaycastHit2D[] hits = Physics2D.CircleCastAll(child.position, bulletRadius * 0.5f, frameMovement, frameMovement.magnitude + 0.05f, entityMask.value)
            .Where(x=>!ignore.Contains(x.collider.gameObject))
            .OrderBy(x => x.distance)
            .ToArray();
        int index = -1;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].distance > 0) index = i;
        }
        if (index >= 0)
        {
            RaycastHit2D entityHit = hits[index];
            hitContext.hitInfo = entityHit;
            hitContext.isHit = true;
            hitContext.dist = entityHit.distance;
            hitContext.type = HitColliderType.Entity;
            //hitContext.hitPoint = entityHit.point;
            //hitContext.collider = entityHit.collider;
            //hitContext.hitNormal = entityHit.normal;
            entityHit.collider.TryGetComponent(out hitContext.hp);
        }
        //collision
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, bulletRadius * 0.5f, frameMovement, frameMovement.magnitude + 0.05f, collisionMask.value);
        if (hit)
        {
            hitContextCol.hitInfo = hit;
            hitContextCol.isHit = true;
            hitContextCol.dist = hit.distance;
            hitContextCol.type = HitColliderType.Wall;
            //hitContextCol.hitPoint = hit.point;
            //hitContextCol.collider = hit.collider;
            //hitContextCol.hitNormal = hit.normal;
            hit.collider.TryGetComponent(out hitContextCol.hp);
        }
        return (hitContext.dist < hitContextCol.dist)?hitContext:hitContextCol;
    }

    protected void PlayEffect(Vector2 offset)
    {
        Effect.PlayColored(!isEnemy,"BulletHit", EffectInfo.Pos((Vector2)child.position + offset));
    }

    protected void RotateToDirection(Vector2 direction)
    {
        child.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction)-90);
    }

    protected void ChainReaction(Collider2D col)
    {
        if (attackInfo.chainCount <= 0) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(child.transform.position, 10, entityMask.value);

        float closestDist = 10000;
        Collider2D closest = null;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == col) continue;
            if ((hits[i].transform.position-child.transform.position).sqrMagnitude < closestDist)
            {
                closest = hits[i];
                closestDist = (hits[i].transform.position - child.transform.position).sqrMagnitude;
            }
        }
        if (closest == null) return;

        AttackInfo newInfo = attackInfo;
        newInfo.damage /=2;
        newInfo.additionalBurst = 0;
        newInfo.knockBack = 0;
        newInfo.delay = 0;
        newInfo.direction = (closest.transform.position - child.transform.position).normalized;
        newInfo.chainCount -= 1;
        newInfo.bulletType = BulletType.Small;
        Bullet instance = Fire(child.transform.position, newInfo);
        instance.ignore.Add(col.gameObject);
    }

    protected Vector2 Homing(Vector2 inDir)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 8, entityMask.value);
        if (hits == null || hits.Length == 0) return inDir;

        float closestAngle = -2;
        Collider2D closest = hits[0];
        for (int i = 0; i < hits.Length; i++)
        {
            if (Vector2.Dot(inDir, hits[i].transform.position-transform.position) > closestAngle)
            {
                closest = hits[i];
                closestAngle = Vector2.Dot(inDir, hits[i].transform.position - transform.position);
            }
        }
        return Vector2.MoveTowards(inDir, closest.transform.position - transform.position, Time.deltaTime * 1.5f * attackInfo.homingStrength).normalized;
    }
}
