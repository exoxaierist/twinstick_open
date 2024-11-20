using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MetaRoom : MonoBehaviour
{
    [Header("Input")]
    public RoomList roomList;

    [Header("Composite")]
    public Tilemap ceilingTilemap;
    public Tilemap pathBlockTilemap;
    public Tilemap areaMaskTilemap;
    public Tilemap sideWallTilemap;

    public TileBase maskTile;
    public TileBase sideWallTile;

    private List<Room> placedRooms = new();
    private List<Door> placedDoors = new();
    private List<EnemySpawner> placedSpawners = new();
    private List<Enemy> placedEnemies = new();
    private Room startingRoom;

    public List<Vector2> walkablePos = new();

    private Vector2 meanRoomCenter;

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
    public bool isFriendly;
    public bool isCleared;
    public Action onWaveStart;
    public Action onRoomClear;
    private Coroutine waveCoroutine;

    public void AutoBuildRoom(MetaRoomInfo _info, Room optionalTestRoom = null)
    {
        info = _info;
        switch (info.type)
        {
            case RoomType.Combat:
                isFriendly = false;
                PlaceRoomBegin(roomList.GetRandomRoom(RoomSize.Main));
                //AddRoom(RoomSize.Main, 4);
                for (int i = 0; i < 3; i++) AddRoom(RoomSize.Small, 3);
                gameObject.name = "Room " + info.roomNumber;
                break;
            case RoomType.Shop:
                isFriendly = true;
                info.isCleared = true;
                PlaceRoomBegin(roomList.GetShopRoom());
                gameObject.name = "Shop";
                break;
            case RoomType.Spawn:
                isFriendly = true;
                info.isCleared = true;
                PlaceRoomBegin(roomList.GetSpawnRoom());
                gameObject.name = "Spawn";
                break;
            case RoomType.Finish:
                isFriendly = true;
                info.isCleared = true;
                PlaceRoomBegin(roomList.GetFinishRoom());
                break;
            case RoomType.Test:
                isFriendly = true;
                info.isCleared = true;
                PlaceRoomBegin(optionalTestRoom);
                break;
        }

        if (info.upConnected) CreatePortal(Direction.Up, (a, b) => a.y < b.y);
        if (info.downConnected) CreatePortal(Direction.Down, (a, b) => a.y > b.y);
        if (info.rightConnected) CreatePortal(Direction.Right, (a, b) => a.x < b.x);
        if (info.leftConnected) CreatePortal(Direction.Left, (a, b) => a.x > b.x);

        transform.position = new(info.x * LevelManager.roomGap, info.y * LevelManager.roomGap, 0);
        
        currentWaveCount = 1;
        for (int i = 0; i < 6; i++)
        {
            if (UnityEngine.Random.Range(0, 1f) > (PlayerStats.waveChance-0.1f*i)) break;
            currentWaveCount++;
        }
        waveCount = currentWaveCount;
        GetWalkablePos();
    }

    public void TestBuildRoom(Room room, MetaRoomInfo _info)
    {
        info = _info;
        if (info.type == RoomType.Spawn) { isFriendly = true; PlaceRoomBegin(roomList.GetSpawnRoom()); }
        else
        {
            TryPlaceRoom(InstantiateRoom(room), new(0, 0), (Vector2)new(0, 0));
        }

        if (info.upConnected) CreatePortal(Direction.Up, (a, b) => a.y < b.y);
        if (info.downConnected) CreatePortal(Direction.Down, (a, b) => a.y > b.y);
        if (info.rightConnected) CreatePortal(Direction.Right, (a, b) => a.x < b.x);
        if (info.leftConnected) CreatePortal(Direction.Left, (a, b) => a.x > b.x);
    }

    public void OnRoomEnter(Direction direction)
    {
        roomEnterCount++;
        _enemyCount = 0;
        PathFinder.ceilingTilemap = ceilingTilemap;
        PathFinder.pathBlockTilemap = pathBlockTilemap;
        if (roomEnterCount <= 1) 
        {
            Deco.SpawnVase();
            if (!isFriendly)
            {
                this.Delay(0.6f, ClosePortals);
                this.Delay(2, () =>
                {
                    SpawnEnemies(10);
                });
            }
            return; 
        }
    }

    private void SpawnEnemies(int count)
    {
        //gets position that don't overlap
        List<Vector2> spawnPos = new();
        for (int i = 0; i < count; i++)
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
        for (int i = 0; i < spawnPos.Count; i++)
        {
            Enemy.Spawn(spawnPos[i], EnemySpawnSet.GetID());
        }
        onWaveStart?.Invoke();
        if (waveCoroutine != null) { StopCoroutine(waveCoroutine); waveCoroutine = null; }
        waveCoroutine = StartCoroutine(Wave());
    }

    private IEnumerator Wave()
    {
        yield return new WaitForSeconds(10);
        if (currentWaveCount > 0) SpawnEnemies(10);
    }

    public void CheckRoomClear()
    {
        if(_enemyCount <= 0)
        {
            if (currentWaveCount <= 0)
            {
                isCleared = true;
                onRoomClear?.Invoke();
                OpenPortals();
            }
            else { SpawnEnemies(10); }
        }
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

    public Vector2 GetRandomWalkablePos()
    {
        return walkablePos[UnityEngine.Random.Range(0, walkablePos.Count)];
    }

    private void GetWalkablePos()
    {
        for (int x = -50; x < 50; x++)
        {
            for (int y = -50; y < 50; y++)
            {
                Vector3Int intPos = new(x, y, 0);
                if (ceilingTilemap.HasTile(intPos) || pathBlockTilemap.HasTile(intPos) || ceilingTilemap.HasTile(intPos + Vector3Int.down)) continue;
                walkablePos.Add(transform.position + (Vector3)intPos + new Vector3(0.5f,0.5f));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Vector2 pos in walkablePos)
        {
            Gizmos.DrawSphere(pos, 0.4f);
        }
    }

    private void AddRoom(RoomSize roomType, int doorMinWidth = 2)
    {
        for (int i = 0; i < 20; i++)
        {
            Room room = InstantiateRoom(roomList.GetRandomRoom(roomType));

            ConnectionSocket doorA = startingRoom.GetRandomSocket();
            ConnectionSocket doorB = room.GetRandomSocket(doorA.direction.Reverse());
            int doorLength = UnityEngine.Random.Range(doorMinWidth, Mathf.Min(doorA.length, doorB.length) + 1);
            if(doorLength >= doorMinWidth)
            {
                int offsetA = UnityEngine.Random.Range(0, doorA.length - doorLength + 1);
                int offsetB = UnityEngine.Random.Range(0, doorB.length - doorLength + 1);

                if (TryPlaceRoom(room, doorA, doorB, doorLength, offsetA, offsetB)) return;
            }

            Destroy(room.gameObject);
            roomList.UndoRandomPick();
        }
    }
    
    private void PlaceRoomBegin(Room roomPrefab)
    {
        Room room = InstantiateRoom(roomPrefab);
        startingRoom = room;
        TryPlaceRoom(room, new(0, 0), (Vector2)new(0, 0));
    }

    private bool TryPlaceRoom(Room roomToPlace, ConnectionSocket doorA, ConnectionSocket doorB, int length, int offsetA, int offsetB)
    {
        Vector2 dir = doorA.direction.IsHorizontal() ? Vector2.down : Vector2.right;
        if (!TryPlaceRoom(roomToPlace, 
            (Vector2)doorA.transform.position + (dir * offsetA) + ((doorA.direction == Direction.Up) ? Vector2.up : Vector2.zero), 
            doorB.offset + (dir * offsetB) + ((doorB.direction == Direction.Up) ? Vector2.up : Vector2.zero))) return false;
        ConnectDoor(doorA, offsetA, length);
        return true;
    }
    private bool TryPlaceRoom(Room roomToPlace, Vector2 startingPos, Vector2 doorOffset)
    {
        if (!CheckAreaClear(roomToPlace.roomAreaTilemap, startingPos, doorOffset)) return false;
        RemoveCeiling(roomToPlace.roomAreaTilemap, startingPos, doorOffset);
        if (roomToPlace.pathBlockTilemap != null) AddPathBlockMask(roomToPlace.pathBlockTilemap, startingPos, doorOffset);
        
        roomToPlace.transform.position = startingPos - doorOffset;
        placedRooms.Add(roomToPlace);
        placedDoors.AddRange(roomToPlace.doors);
        placedSpawners.AddRange(roomToPlace.spawners);

        meanRoomCenter = (meanRoomCenter * (placedRooms.Count - 1) + (Vector2)roomToPlace.transform.position) / placedRooms.Count;
        return true;
    }

    private Room InstantiateRoom(Room roomPrefab)
    {
        Room instance = Instantiate(roomPrefab);
        instance.transform.SetParent(transform);
        instance.ResetData();
        return instance;
    }

    private void CreatePortal(Direction dir, Func<Vector2,Vector2,bool> swapPredicate)
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
                if (swapPredicate.Invoke(matchingSockets[j].transform.position, matchingSockets[j + 1].transform.position))
                {
                    (matchingSockets[j + 1], matchingSockets[j]) = (matchingSockets[j], matchingSockets[j + 1]);
                }
            }
        }

        //try placing portal to each socket
        foreach (ConnectionSocket item in matchingSockets)
        {
            for (int j = 0; j < item.length - (dir.IsVertical() ? 2 : 3) + 1; j++)
            {
                if (TryPlaceRoom(portal, item, portal.sockets[0], dir.IsVertical() ? 2 : 3, j, 0)) return;
            }
        }
        Destroy(portal.gameObject);
    }

    private void ConnectDoor(ConnectionSocket socket, int offset, int length)
    {
        Vector3Int pos = Vector3Int.FloorToInt(socket.transform.position);
        if(socket.direction != Direction.Up) pos.y -= 1;
        Vector3Int dir = socket.direction.IsHorizontal() ? Vector3Int.down : Vector3Int.right;
        Vector3Int sidewallOffset = (dir.y == 0) ? Vector3Int.down : Vector3Int.zero;

        for (int i = 0; i < length; i++)
        {
            ceilingTilemap.SetTile(pos + dir * (i + offset), null);
            CreateSideWall(pos + sidewallOffset + dir * (i + offset));
        }
    }

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
        CreateSideWall(bounds, startingPos - doorOffset);
    }

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
        CreateSideWall(bounds, startingPos - doorOffset);
    }

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

    private void CreateSideWall(BoundsInt bounds, Vector2 pivot)
    {
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                CreateSideWall(new Vector2(x, y) + pivot);
            }
        }
    }
    private void CreateSideWall(Vector2 position) => CreateSideWall(Vector3Int.FloorToInt(position));
    private void CreateSideWall(Vector3Int pos)
    {
        if(!ceilingTilemap.HasTile(pos) && ceilingTilemap.HasTile(pos + new Vector3Int(0, 1)))
        {
            sideWallTilemap.SetTile(pos + new Vector3Int(0, 1), sideWallTile);
            if (ceilingTilemap.HasTile(pos + new Vector3Int(1, 1))) sideWallTilemap.SetTile(pos + new Vector3Int(1, 1), sideWallTile);
            if (ceilingTilemap.HasTile(pos + new Vector3Int(-1, 1))) sideWallTilemap.SetTile(pos + new Vector3Int(-1, 1), sideWallTile);
            return;
        }
        if(!ceilingTilemap.HasTile(pos + new Vector3Int(0, 1)))
        {
            sideWallTilemap.SetTile(pos + new Vector3Int(0, 1), null);
        }
    }
}
