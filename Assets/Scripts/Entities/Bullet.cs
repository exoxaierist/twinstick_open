using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static GameObjectPool pool = new("Bullet");
    public static void Fire(Vector2 position, AttackInfo info, GameObject ignore = null)
    {
        SpawnBullet(position, info, ignore:ignore);

        info.damage = (int)(info.damage * 0.5f);
        for (int i = 0; i < info.additionalBurst; i++)
        {
            info.bulletType = BulletType.Small;
            Utility.GetMono().Delay(0.1f * i + 0.1f, () => SpawnBullet(position, info));
        }
    }
    private static void SpawnBullet(Vector2 position, AttackInfo info, GameObject ignore = null)
    {
        Bullet instance = pool.Get().GetComponent<Bullet>();
        instance.transform.position = position - (Vector2)instance.child.localPosition;
        instance.Initialize(info);
        instance.ignore = ignore;
    }
    public const int DEFAULT_DAMAGE = 10;
    public const float DEFAULT_KNOCKBACK = 1;
    public const float ENEMY_BULLET_SPEED = 5.5f;
    public const float ENEMY_BULLET_SPEED_FAST = 20;

    public Material playerMat;
    public Material enemyMat;
    public Sprite normalSprite;
    public Sprite smallSprite;

    [HideInInspector] public bool isEnemy = false;
    [HideInInspector] public float bulletSize = 0.35f;
    [HideInInspector] public float bulletSpeed = 10;
    [HideInInspector] public float maxDistance = 10;
    private float distanceTraveled = 0;

    protected AttackInfo attackInfo;

    private Vector2 direction;
    private LayerMask entityMask;
    private LayerMask collisionMask;
    private Transform child;
    private SpriteRenderer sprite;
    private float randomPhase;
    public GameObject ignore;

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
            PlayEffect(new(0, -0.5f));
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
        if (!hitContext.hit)
        {
            //doesn't hit anything
            transform.Translate(frameMovement);
        }
        else if (hitContext.type == HitColliderType.Entity && hitContext.hp != null)
        {
            //hits entity
            hitContext.hp.Damage(attackInfo);
            ChainReaction(hitContext.collider);
            transform.Translate(hitContext.hitPoint - (Vector2)child.position);

            PlayEffect(new(0, -0.5f));

            pool.Release(gameObject);
        }
        else if (hitContext.type == HitColliderType.Wall)
        {
            //hits wall
            PlayEffect(new(0, Random.Range(0, -0.2f)-child.localPosition.y));
            if (attackInfo.bounceCount > 0)
            {
                attackInfo.bounceCount--;
                distanceTraveled = 0;
                direction = Vector2.Reflect(direction, hitContext.hitNormal);
            }
            else pool.Release(gameObject);
        }
    }

    public void Initialize(AttackInfo _info)
    {
        attackInfo = _info;
        bulletSpeed = _info.bulletSpeed;
        isEnemy = _info.attacker==Entity.Enemy;
        maxDistance = _info.bulletMaxDist;
        distanceTraveled = 0;
        ignore = null;
        randomPhase = Random.Range(0, 100f);
        direction = attackInfo.direction;
        if (isEnemy)
        {
            sprite.material = enemyMat;
            entityMask = LayerMask.GetMask("Player");
        }
        else
        {
            sprite.material = playerMat;
            entityMask = LayerMask.GetMask("Enemy");
        }
        if (attackInfo.bulletType == BulletType.Normal) { sprite.sprite = normalSprite; }
        else if (attackInfo.bulletType == BulletType.Small) 
        { 
            sprite.sprite = smallSprite;
        }
    }

    protected BulletHit CheckHit(Vector2 frameMovement)
    {
        BulletHit hitContext = new() { hit = false, dist = 10000};
        BulletHit hitContextCol = new() { hit = false, dist = 10000 };
        //hit
        RaycastHit2D[] hits = Physics2D.CircleCastAll(child.position, bulletSize * 0.5f, frameMovement, frameMovement.magnitude + 0.05f, entityMask.value);
        foreach (RaycastHit2D entityHit in hits)
        {
            if (entityHit.collider.gameObject == ignore) continue;
            hitContext.hit = true;
            hitContext.dist = entityHit.distance;
            hitContext.type = HitColliderType.Entity;
            hitContext.hitPoint = entityHit.point;
            hitContext.collider = entityHit.collider;
            hitContext.hitNormal = entityHit.normal;
            entityHit.collider.TryGetComponent(out hitContext.hp);
            break;
        }
        //collision
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, bulletSize * 0.5f, frameMovement, frameMovement.magnitude + 0.05f, collisionMask.value);
        if (hit)
        {
            hitContextCol.hit = true;
            hitContextCol.dist = hit.distance;
            hitContextCol.type = HitColliderType.Wall;
            hitContextCol.hitPoint = hit.point;
            hitContextCol.collider = hit.collider;
            hitContextCol.hitNormal = hit.normal;
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
        newInfo.bulletSpeed *= 0.5f;
        newInfo.damage = (int)(newInfo.damage * 0.5f);
        newInfo.additionalBurst = 0;
        newInfo.delay = 0;
        newInfo.direction = (closest.transform.position - child.transform.position).normalized;
        newInfo.chainCount -= 1;
        newInfo.bulletType = BulletType.Small;
        Fire(child.transform.position, newInfo, ignore: col.gameObject);
    }

    protected Vector2 Homing(Vector2 inDir)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 6, entityMask.value);
        if (hits.Length == 0) return inDir;

        float closestDist = 10000;
        Collider2D closest = null;
        for (int i = 0; i < hits.Length; i++)
        {
            if ((hits[i].transform.position - transform.position).sqrMagnitude < closestDist)
            {
                closest = hits[i];
                closestDist = (hits[i].transform.position - transform.position).sqrMagnitude;
            }
        }
        return Vector2.MoveTowards(inDir, closest.transform.position - transform.position, Time.deltaTime * 1.5f * attackInfo.homingStrength).normalized;
    }
}
