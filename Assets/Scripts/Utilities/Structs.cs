using UnityEngine;

//info for giving, receiving attacks
public struct AttackInfo
{
    public Entity attacker;
    public string attackerName;
    public Vector2 direction;
    public AttackType attackType;
    public AttackEffect attackEffect;

    public int damage;
    public float knockBack;
    public float bulletSpeed;
    public float bulletMaxDist;

    public bool isCrit;
    public bool isHeal;

    //misc
    public float delay;
    public int additionalBurst;
    public int chainCount;
    public BulletType bulletType;
    public int bulletAdditionalCost;

    //behaviours
    public bool isChainReaction;
    public bool isHoming;
    public float homingStrength;
    public bool isStoned;
    public float stoneStrength;
    public bool doBend;
    public float bendStrength; //positive to right negative to left
    public int bounceCount;
    public bool penetrate;

    public AttackInfo Damage(int amount) { damage = amount; return this; }
    public AttackInfo Distance(float dist) { bulletMaxDist = dist; return this; }
    public AttackInfo Direction(Vector2 dir) { direction = dir; return this; }
}

//passed for when checking if bullet hit something
public struct BulletHit
{
    public RaycastHit2D hitInfo;
    public HitColliderType type;
    public bool isHit;
    public float dist;

    public Hp hp;
}

public struct BulletFinish
{
    public Vector2 position;
    public Vector2 direction;
}

//passed to weapon class when player tries to attack
public struct AttackInput
{
    public bool isStarted;
    public bool isPressing;
    public bool isReleased;
    public Vector2 direction;
}

public struct AttackParams
{
    public static AttackParams Get(float speed = 10, float maxDistance = 10, bool enemy = false)
    {
        return new()
        {
            speed = speed,
            maxDistance = maxDistance
        };
    }
    public bool isEnemy;

    public Vector2 direction;
    public float speed;
    public float size;
    public float maxDistance;
}

[System.Serializable]
public struct ItemDisplayInfo
{
    public string name;
    public string ID;
    [TextArea] public string description;
    public int price;
}

[System.Serializable]
public struct WeaponInfo
{
    public Sprite sprite;
    public GameObject prefab;

    public string ID;
    public string name;
    [TextArea] public string description;

    public bool unlocked;
}
