public enum Direction
{
    Up,
    Right,
    Down,
    Left
}

public enum DoorType
{
    Narrow,
    Wide,
    SuperWide,
}

public enum RoomSize
{
    Main,
    Small,
}

public enum RoomType
{
    Combat,
    Shop,
    Spawn,
    Test,
    Finish,
    Boss,
}

public enum HitColliderType
{
    None,
    Wall,
    Entity
}

public enum Entity
{
    Player,
    Enemy,
    Gimic,
}

public enum MovementBehaviour
{
    None,
    Wander,
    FollowPlayer,
}

public enum SpriteDirMode
{
    FaceDirection,
    FacePlayer,
}

public enum BulletType
{
    Normal,
    Small,
    Large,
    Tracking,
    Fire,
}

public enum AttackType
{
    BulletHit,
    Fire,
    Contact,
}

public enum AttackEffect
{
    None,
    Fire,
}
