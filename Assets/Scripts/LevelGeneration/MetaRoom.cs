using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MetaRoom : MonoBehaviour
{
    //RoomList to use
    public RoomList roomList;

    //tilemap ref
    public Tilemap ceilingTilemap;
    public Tilemap pathBlockTilemap;
    public Tilemap areaMaskTilemap;
    public Tilemap sideWallTilemap;

    //tiles
    public TileBase maskTile;
    public TileBase sideWallTile;
    public TileBase pitTile;

    private List<Room> placedRooms = new();
    private List<ConnectionSocket> placedSockets = new();
    private Room startingRoom;

    public List<Vector2> walkablePos = new();

    public RoomPortal portalUp;
    public RoomPortal portalDown;
    public RoomPortal portalRight;
    public RoomPortal portalLeft;

    public MetaRoomInfo info;

    private int _enemyCount;
    public int enemyCount
    {
        get { return _enemyCount; }
        set {
            _enemyCount = value;
            CheckRoomClear();
        }
    }
    public int waveCount;
    public int currentWaveCount = 0;
    public int roomEnterCount = 0;
    public int coinDropAmount = 0;
    public bool isFriendly;
    public bool isCleared;
    public Action onWaveStart;
    public Action onRoomClear;
    private Coroutine waveCoroutine;
    
    //debug
    private void OnDrawGizmosSelected()
    {
        return;
        foreach (Vector2 pos in walkablePos)
        {
            Gizmos.DrawSphere(pos, 0.4f);
        }
    }

    public void BuildRoom(MetaRoomInfo _info, Room optionalTestRoom = null)
    {
        info = _info;
        //generate room according to type
        switch (info.type)
        {
            case RoomType.Combat:
                isFriendly = false;
                AddFirstRoom(roomList.GetRandomRoom(RoomSize.Main));
                //for (int i = 0; i < 6; i++) AddRoom(RoomSize.Main, 3);
                for (int i = 0; i < 3; i++) AddRoom(RoomSize.Small, 2);
                gameObject.name = "Room " + info.roomNumber;
                break;
            case RoomType.Shop:
                isFriendly = true;
                info.isCleared = true;
                AddFirstRoom(roomList.GetShopRoom());
                gameObject.name = "Shop" + info.roomNumber;
                break;
            case RoomType.Spawn:
                isFriendly = true;
                info.isCleared = true;
                AddFirstRoom(roomList.GetSpawnRoom());
                gameObject.name = "Spawn";
                break;
            case RoomType.Boss:
                isFriendly = false;
                info.isCleared = false;
                AddFirstRoom(BossInfo.GetRoom(info.bossId));
                gameObject.name = "Boss " + info.bossId;
                break;
            /*case RoomType.Finish:
                isFriendly = true;
                info.isCleared = true;
                AddFirstRoom(roomList.GetFinishRoom());
                break;
            case RoomType.Test:
                isFriendly = true;
                info.isCleared = true;
                AddFirstRoom(optionalTestRoom);
                break;*/
        }

        //connect portal
        if (info.upConnected) CreatePortal(Direction.Up, (a, b) => a.y < b.y);
        if (info.downConnected) CreatePortal(Direction.Down, (a, b) => a.y > b.y);
        if (info.rightConnected) CreatePortal(Direction.Right, (a, b) => a.x < b.x);
        if (info.leftConnected) CreatePortal(Direction.Left, (a, b) => a.x > b.x);

        CreateSideWall(ceilingTilemap.cellBounds,Vector2.zero);

        transform.position = new(info.x * LevelManager.roomGap, info.y * LevelManager.roomGap, 0);
        GetWalkablePos();

        //set wave count
        currentWaveCount =
            info.roomNumber <= 3 ? 1 :
            info.roomNumber <= 5 ? 2 :
            info.roomNumber <= 10 ? 3 :
            info.roomNumber <= 15 ? 4 :
            info.roomNumber <= 20 ? 5 :
            info.roomNumber <= 25 ? 5 :
            5;
        int maxWave =
            info.roomNumber <= 3 ? 0 :
            info.roomNumber <= 10 ? 1 :
            info.roomNumber <= 20 ? 2 :
            info.roomNumber <= 30 ? 3 :
            4;
        for (int i = 0; i < maxWave; i++)
        {
            if (UnityEngine.Random.Range(0, 1f) > 0.5f) break;
            currentWaveCount++;
        }
        waveCount = currentWaveCount;
    }

    public void OnRoomEnter()
    {
        roomEnterCount++;
        _enemyCount = 0;

        //set pathfinder tilemap
        PathFinder.ceilingTilemap = ceilingTilemap;
        PathFinder.pathBlockTilemap = pathBlockTilemap;

        if (roomEnterCount <= 1) 
        {
            if(info.type == RoomType.Boss)
            {
                //boss spawn
                BossSpawner spawner = GetComponentInChildren<BossSpawner>();
                Enemy.SpawnImmediate(spawner.transform.position, info.bossId);
                this.Delay(1f, ClosePortals);
            }
            else
            {
                //normal enemy spawn
                Deco.SpawnVase();
                if (!isFriendly)
                {
                    this.Delay(0.6f, ClosePortals);
                    this.Delay(2, () =>
                    {
                        SpawnWave();
                    });
                }
            }
        }
    }

    public void CheckRoomClear()
    {
        if(_enemyCount <= 0)
        {
            if (currentWaveCount <= 0 || info.type == RoomType.Boss)
            {
                isCleared = true;
                onRoomClear?.Invoke();
                OpenPortals();
            }
            else { SpawnWave(); }
        }
    }

    private void SpawnWave()
    {
        //gets position that don't overlap
        HashSet<Vector2> spawnPos = new();
        for (int i = 0; i < PlayerStats.waveEnemyCount; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                Vector2 pos = GetRandomWalkablePos();
                if (!spawnPos.Contains(pos))
                {
                    spawnPos.Add(pos);
                    break;
                }
            }
        }
        currentWaveCount -= 1;
        EnemySpawnSet.SetRecipe();
        foreach (Vector2 pos in spawnPos)
        {
            Enemy.Spawn(pos, EnemySpawnSet.GetID());
        }
        onWaveStart?.Invoke();
        if (waveCoroutine != null) { StopCoroutine(waveCoroutine); waveCoroutine = null; }
        waveCoroutine = StartCoroutine(WaveCounter());
    }
    private IEnumerator WaveCounter()
    {
        yield return new WaitForSeconds(PlayerStats.waveInterval);
        if (currentWaveCount > 0) SpawnWave();
    }


    public void ClosePortals()
    {
        if (portalUp != null) portalUp.Lock();
        if (portalDown != null) portalDown.Lock();
        if (portalRight != null) portalRight.Lock();
        if (portalLeft != null) portalLeft.Lock();
    }
    public void OpenPortals()
    {
        if(portalUp!=null) portalUp.Open();
        //if (portalDown != null) portalDown.Open();
        if (portalRight != null) portalRight.Open();
        if (portalLeft != null) portalLeft.Open();
    }

    //finds all walkable pos (area's that can be accessed)
    private void GetWalkablePos()
    {
        bool activeState = gameObject.activeInHierarchy;
        gameObject.SetActive(true);
        Physics2D.SyncTransforms();
        for (int x = -50; x < 50; x++)
        {
            for (int y = -50; y < 50; y++)
            { 
                Vector3Int intPos = new(x, y, 0);
                if (ceilingTilemap.HasTile(intPos) || pathBlockTilemap.HasTile(intPos) || ceilingTilemap.HasTile(intPos + Vector3Int.down)
                    || Physics2D.OverlapPoint((Vector2)transform.position + new Vector2(x+0.5f,y+0.5f),LayerMask.GetMask("PawnBlock"))!=null) continue;
                walkablePos.Add(transform.position + (Vector3)intPos + new Vector3(0.5f,0.5f));
            }
        }
        gameObject.SetActive(activeState);
    }
    public Vector2 GetRandomWalkablePos() => walkablePos.GetRandom();


    #region Place Room

    //places first room in the meta room
    private void AddFirstRoom(Room roomPrefab)
    {
        Room room = InstantiateRoom(roomPrefab);
        startingRoom = room;
        TryPlaceRoom(room, new(0, 0), new(0, 0));
    }
    //adds room to meta room
    private void AddRoom(RoomSize roomType, int doorMinWidth = 2)
    {
        for (int i = 0; i < 40; i++)
        {
            Room room = InstantiateRoom(roomList.GetRandomRoom(roomType));
            ConnectionSocket doorA = startingRoom.GetRandomSocket();
            //ConnectionSocket doorA = placedSockets.GetRandom();
            ConnectionSocket doorB = room.GetRandomSocket(doorA.direction.Reverse());
            int doorLength = UnityEngine.Random.Range(doorMinWidth, Mathf.Min(doorA.length, doorB.length) + 1);
            if(doorLength >= doorMinWidth)
            {
                int offsetA = UnityEngine.Random.Range(0, doorA.length - doorLength + 1);
                int offsetB = UnityEngine.Random.Range(0, doorB.length - doorLength + 1);

                if (TryPlaceRoom(room, doorA, doorB, doorLength, offsetA, offsetB)) return;
            }

            Destroy(room.gameObject);
        }
    }
    //places room based on connection sockets(door pieces)
    private bool TryPlaceRoom(Room roomToPlace, ConnectionSocket doorA, ConnectionSocket doorB, int length, int offsetA, int offsetB)
    {
        Vector2 dir = doorA.direction.IsHorizontal() ? Vector2.down : Vector2.right;
        if (!TryPlaceRoom(roomToPlace, 
            (Vector2)doorA.transform.position + (dir * offsetA) + ((doorA.direction == Direction.Up) ? Vector2.up : Vector2.zero), 
            doorB.offset + (dir * offsetB) + ((doorB.direction == Direction.Up) ? Vector2.up : Vector2.zero))) return false;
        ConnectMainSockets(doorA, offsetA, length);
        return true;
    }
    //places room raw with just the position of the new room
    private bool TryPlaceRoom(Room roomToPlace, Vector2 startingPos, Vector2 doorOffset)
    {
        if (!CheckAreaClear(roomToPlace.floorTilemap, startingPos, doorOffset)) return false;
        RemoveCeiling(roomToPlace.floorTilemap, startingPos, doorOffset);
        if (roomToPlace.pathBlockTilemap != null) AddPathBlockMask(roomToPlace.pathBlockTilemap, startingPos, doorOffset);
        
        roomToPlace.transform.position = startingPos - doorOffset;
        placedRooms.Add(roomToPlace);
        placedSockets.AddRange(roomToPlace.sockets);

        return true;
    }
    //instantiate the new room prefab
    private Room InstantiateRoom(Room roomPrefab)
    {
        Room instance = Instantiate(roomPrefab);
        instance.transform.SetParent(transform);
        instance.ResetData();
        return instance;
    }


    //creates portal to another meta room
    private void CreatePortal(Direction dir, Func<Vector2,Vector2,bool> sortPredicate)
    {
        //instantiate and assign portal
        Room portal = InstantiateRoom(dir == Direction.Up ? roomList.upPortal : dir == Direction.Down ? roomList.downPortal : dir == Direction.Right ? roomList.rightPortal : roomList.leftPortal);
        if (dir == Direction.Up) portalUp = (RoomPortal)portal;
        else if (dir == Direction.Down) portalDown = (RoomPortal)portal;
        else if (dir == Direction.Right) portalRight = (RoomPortal)portal;
        else if (dir == Direction.Left) portalLeft = (RoomPortal)portal;

        //get and sort all sockets matching direction
        List<ConnectionSocket> matchingSockets = new();
        foreach (Room room in placedRooms)
            matchingSockets.AddRange(room.sockets.FindAll(x => x.direction == dir));

        for (int i = 0; i < matchingSockets.Count - 1; i++)
        {
            for (int j = 0; j < matchingSockets.Count - 1 - i; j++)
            {
                if (sortPredicate.Invoke(matchingSockets[j].transform.position, matchingSockets[j + 1].transform.position))
                {
                    (matchingSockets[j + 1], matchingSockets[j]) = (matchingSockets[j], matchingSockets[j + 1]);
                }
            }
        }

        //try placing portal to each socket
        foreach (ConnectionSocket item in matchingSockets)
        {
            for (int j = 0; j < item.length - 1; j++)
            {
                if (TryPlaceRoom(portal, item, portal.sockets[0], 2, j, 0)) return;
            }
        }
        Destroy(portal.gameObject);
    }
    //removes the ceiling tiles and creates side walls for newly placed connections
    private void ConnectMainSockets(ConnectionSocket socket, int offset, int length)
    {
        socket.appliedLength = length;
        socket.appliedOffset = offset;
        Vector3Int pos = Vector3Int.FloorToInt(socket.transform.position);
        if(socket.direction != Direction.Up) pos.y -= 1;
        Vector3Int dir = socket.direction.IsHorizontal() ? Vector3Int.down : Vector3Int.right;
        Vector3Int sidewallOffset = (dir.y == 0) ? Vector3Int.down : Vector3Int.zero;
        Vector3Int ceilingOffset = socket.direction.IsHorizontal() ? Vector3Int.up : Vector3Int.zero;

        for (int i = 0; i < length+(socket.direction.IsHorizontal()?1:0); i++) 
        {
            ceilingTilemap.SetTile(pos + dir * (i + offset) + ceilingOffset, null);
        }
    }

    #region Room Mask

    //adds to the mask that is used to check if a new room can fit in the new position
    private void AddToMask(Vector3Int pos)
    {
        pos.y += 1;
        areaMaskTilemap.SetTile(pos, maskTile);
        //extend out 1 block
        areaMaskTilemap.SetTile(pos + new Vector3Int(-1, 1, 0), maskTile);
        areaMaskTilemap.SetTile(pos + new Vector3Int(0, 1, 0), maskTile);
        areaMaskTilemap.SetTile(pos + new Vector3Int(1, 1, 0), maskTile);
        areaMaskTilemap.SetTile(pos + new Vector3Int(-1, 0, 0), maskTile);
        areaMaskTilemap.SetTile(pos + new Vector3Int(1, 0, 0), maskTile);
        areaMaskTilemap.SetTile(pos + new Vector3Int(-1, -1, 0), maskTile);
        areaMaskTilemap.SetTile(pos + new Vector3Int(0, -1, 0), maskTile);
        areaMaskTilemap.SetTile(pos + new Vector3Int(1, -1, 0), maskTile);
    }

    //uses the mask above to see if the new room can fit
    private bool CheckAreaClear(Tilemap area, Vector2 startingPos, Vector2 doorOffset)
    {
        BoundsInt bounds = area.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (area.HasTile(pos))
                {
                    pos = pos + Vector3Int.FloorToInt(startingPos - doorOffset);
                    if (areaMaskTilemap.HasTile(pos) || areaMaskTilemap.HasTile(pos+Vector3Int.up)) return false;
                }
            }
        }
        return true;
    }
    private bool CheckAreaClear(BoundsInt bounds)
    {
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (areaMaskTilemap.HasTile(pos)) return false;
            }
        }
        return true;
    }

    #endregion

    #region Edit Tilemap
    
    //remove the new room's floor tile positions from the ceiling tilemap
    private void RemoveCeiling(Tilemap area, Vector2 startingPos, Vector2 doorOffset)
    {
        BoundsInt bounds = area.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (area.HasTile(pos))
                {
                    pos = pos + Vector3Int.FloorToInt(startingPos - doorOffset);
                    AddToMask(pos);
                    ceilingTilemap.SetTile(pos, null);
                    pos.y -= 1;
                    AddToMask(pos);
                    ceilingTilemap.SetTile(pos, null);
                }
            }
        }
    }
    private void RemoveCeiling(BoundsInt bounds)
    {
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                AddToMask(pos);
                ceilingTilemap.SetTile(pos, null);
                pos.y -= 1;
                AddToMask(pos);
                ceilingTilemap.SetTile(pos, null);
            }
        }
    }

    //path block mask for blocking PathFinder route finds
    private void AddPathBlockMask(Tilemap area, Vector2 startingPos, Vector2 doorOffset)
    {
        BoundsInt bounds = area.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (area.HasTile(pos))
                {
                    pos = pos + Vector3Int.FloorToInt(startingPos - doorOffset);
                    pos.y -= 1;
                    AddToMask(pos);
                    pathBlockTilemap.SetTile(pos, maskTile);
                    pathBlockTilemap.SetTile(pos+Vector3Int.up, maskTile);
                    pathBlockTilemap.SetTile(pos+Vector3Int.up*2, maskTile);
                }
            }
        }
    }

    private void CreateSideWall(BoundsInt bounds, Vector2 pivot)
    {
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new(x, y, 0);
                if (!ceilingTilemap.HasTile(pos) && ceilingTilemap.HasTile(pos + new Vector3Int(0, 1)))
                {
                    sideWallTilemap.SetTile(pos + new Vector3Int(0, 1), sideWallTile);
                    if (ceilingTilemap.HasTile(pos + new Vector3Int(1, 1))) sideWallTilemap.SetTile(pos + new Vector3Int(1, 1), sideWallTile);
                    if (ceilingTilemap.HasTile(pos + new Vector3Int(-1, 1))) sideWallTilemap.SetTile(pos + new Vector3Int(-1, 1), sideWallTile);
                }
            }
        }
    }

    #endregion

    #endregion
}
