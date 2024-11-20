using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    //parameters
    public int roomCountMin = 5;
    public int roomCountMax = 10;
    private int roomCount = 10;
    public const float roomGap = 100;
    public Room roomToGenerate; //for test

    private List<MetaRoomInfo> levelLayout = new();
    public Action<MetaRoomInfo> onRoomChange;

    //stats
    public static MetaRoom currentRoom;
    [HideInInspector] public Vector2Int currentPos = new();
    private MetaRoom startRoom;
    private MetaRoomInfo lastGen;
    private int lastGenX = 0;
    private int lastGenY = 0;
    private int generatedRoomCount = 0;

    public static int roomNumber = 0;
    public static int level = 0;
    public static bool inLevel = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init()
    {
        roomNumber = 0;
        level = 0;
        inLevel = false;
        SceneManager.sceneLoaded += Init;
    }
    public static void Init(Scene _, LoadSceneMode __) => Init();

    private void Awake()
    {
        main = this;
        currentRoom = null;
        level = 0;
    }

    public void EndLevel()
    {
        DestroyLevel();
        level++;
    }

    private void DestroyLevel()
    {
        currentPos = new();
        onRoomChange = null;
        startRoom = null;
        inLevel = false;
        levelLayout.Clear();
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++) Destroy(transform.GetChild(i).gameObject);
        childCount = UIManager.main.minimapParent.childCount;
        for (int i = 0; i < childCount; i++) Destroy(UIManager.main.minimapParent.GetChild(i).gameObject);
        PlayerStart.DespawnPlayer();
        BloodParticleHandler.main.Clear();
    }

    public void StartLevel(Action onGenerateFinish)
    {
        roomCount = UnityEngine.Random.Range(roomCountMin, roomCountMax);
        if (roomToGenerate != null) roomCount = 1;
        level++;
        CreateAltLayout();
        SetPlayerStart();
        this.DelayFrame(() => onGenerateFinish?.Invoke());

        /*CreateLayout();
        this.Delay(0.1f, () => {
            GenerateMetaRooms();
            CreateMinimap();
            SetPlayerStart();
            onGenerateFinish?.Invoke();
        });*/
    }

    public void CheckLevelClear()
    {
        if (LevelFinish.isEnabled) return;
        foreach (MetaRoomInfo info in levelLayout)
        {
            if (!info.isCleared) return;
        }
        // on clear
        LevelFinish.EnableFinish();
    }
    
    public void ChangeRoom(MetaRoom from, Direction direction)
    {
        //int fromIndex = 0;
        int toIndex = 0;
        for (int i = 0; i < levelLayout.Count; i++)
        {
            MetaRoomInfo info = levelLayout[i];
            if (info.metaRoom == from)
            {
                //fromIndex = i;
                if (direction == Direction.Up) toIndex = GetIndexAtPos(info.x, info.y + 1);
                else if (direction == Direction.Down) toIndex = GetIndexAtPos(info.x, info.y - 1);
                else if (direction == Direction.Right) toIndex = GetIndexAtPos(info.x + 1, info.y);
                else if (direction == Direction.Left) toIndex = GetIndexAtPos(info.x - 1, info.y);
                break;
            }
        }
        if(roomNumber > 0 && (levelLayout[toIndex].type == RoomType.Combat && from.info.type == RoomType.Combat))
        {
            //on room progress
            PlayerStats.waveChance += 0.06f;
            PlayerStats.enemyHpMul += 0.05f;
            UIManager.main.HideHud();
            PerkPicker.Pick(() =>
            {
                ChangeRoom(from, levelLayout[toIndex], direction);
                UIManager.main.ShowHud();
            });
        }
        else
        {
            //change room to shop or etc
            ChangeRoom(from, levelLayout[toIndex], direction);
        }
    }

    public void ChangeRoom(MetaRoom from, MetaRoomInfo toInfo, Direction direction)
    {
        MetaRoom to = toInfo.metaRoom;

        from.gameObject.SetActive(false);
        to.gameObject.SetActive(true);

        currentPos = new(toInfo.x, toInfo.y);
        //levelLayout[fromIndex].minimapBlock.IsCurrentRoom(false);
        //levelLayout[toIndex].minimapBlock.IsCurrentRoom(true);

        Vector2 playerPos = new();
        if (direction == Direction.Up) playerPos = to.portalDown.playerSpawn.position;
        else if (direction == Direction.Down) playerPos = to.portalUp.playerSpawn.position;
        else if (direction == Direction.Right) playerPos = to.portalLeft.playerSpawn.position;
        else if (direction == Direction.Left) playerPos = to.portalRight.playerSpawn.position;

        CheckLevelClear();
        Player.main.MoveRoom(direction, playerPos);

        currentRoom = to;
        to.OnRoomEnter(direction);
        roomNumber = toInfo.roomNumber;
        onRoomChange?.Invoke(toInfo);

        if (roomNumber >= generatedRoomCount - 1) CreateNextRoom();
        Utility.GetMono().DelayFrame(() => { 
            UIManager.main.FadeOut(0.4f);
        });
    }

    private void CreateAltLayout()
    {
        CreateStartRoom();
        for (int i = 0; i < 3; i++)
        {
            CreateNextRoom();
        }
    }

    private void CreateLayout()
    {
        CreateStartRoom();
        for (int i = 0; i < roomCount; i++)
        {
            /*if(i==layoutCount - 2)
            {
                AddMetaRoomToLayout(RoomType.Shop);
                continue;
            }
            if (i == layoutCount - 1)
            {
                AddMetaRoomToLayout(RoomType.Finish);
                continue;
            }*/
            AddMetaRoom();
        }
        SetConnectionsAll();
    }

    private void CreateNextRoom()
    {
        MetaRoomInfo roomInfo = AddMetaRoomAtDirection(Direction.Up, RoomType.Combat);

        if (generatedRoomCount > 1 && (generatedRoomCount-1) % 3 == 0)
        {
            MetaRoomInfo shopRoomInfo = AddMetaRoomAtDirection(Direction.Right, RoomType.Shop);
            shopRoomInfo.metaRoom.AutoBuildRoom(shopRoomInfo);
        }

        lastGen.metaRoom.AutoBuildRoom(lastGen);
        lastGen = roomInfo;
        lastGenX = roomInfo.x;
        lastGenY = roomInfo.y;
    }

    private void CreateMinimap()
    {
        //get max min
        Vector2 min = Vector2.zero, max = Vector2.zero;
        foreach (MetaRoomInfo info in levelLayout)
        {
            if (info.x < min.x) min.x = info.x;
            if (info.y < min.y) min.y = info.y;
            if (info.x > max.x) max.x = info.x;
            if (info.y > max.y) max.y = info.y;
        }
        Vector2 center = (min + max) * 0.5f;
        foreach(MetaRoomInfo info in levelLayout)
        {
            MinimapBlock minimapBlock = Instantiate(Prefab.Get("MinimapBlock")).GetComponent<MinimapBlock>();
            minimapBlock.transform.SetParent(UIManager.main.minimapParent);
            minimapBlock.transform.localPosition = new((info.x-center.x) * UIManager.main.pixelsPerUnit, (info.y-center.y) * UIManager.main.pixelsPerUnit);
            minimapBlock.transform.localScale = Vector3.one;
            onRoomChange += minimapBlock.OnRoomChange;
            info.minimapBlock = minimapBlock;
            minimapBlock.SetVisual(info);
        }
        levelLayout[0].minimapBlock.IsCurrentRoom(true);
    }

    private void GenerateMetaRooms()
    {
        foreach (MetaRoomInfo info in levelLayout)
        {
            info.metaRoom.AutoBuildRoom(info, roomToGenerate);
        }
    }

    private void SetPlayerStart()
    {
        inLevel = true;
        startRoom.GetComponentInChildren<PlayerStart>().SetStart();
        startRoom.gameObject.SetActive(true);
        currentRoom = startRoom;
        startRoom.roomEnterCount = 1;
        onRoomChange?.Invoke(levelLayout[0]);
    }

    private void SetConnectionsAll()
    {
        //connect all
        foreach(MetaRoomInfo info in levelLayout)
        {
            SetConnections(info);
        }
    }

    private void SetConnections(MetaRoomInfo roomInfo)
    {
        if (!roomInfo.rightConnected && HasRoomAt(roomInfo.x + 1, roomInfo.y))
        {
            roomInfo.rightConnected = UnityEngine.Random.Range(-2f, 1f) > 0;
            int index = GetIndexAtPos(roomInfo.x + 1, roomInfo.y);
            levelLayout[index].leftConnected = roomInfo.rightConnected;
        }
        if (!roomInfo.downConnected && HasRoomAt(roomInfo.x, roomInfo.y - 1))
        {
            roomInfo.downConnected = UnityEngine.Random.Range(-2f, 1f) > 0;
            int index = GetIndexAtPos(roomInfo.x, roomInfo.y - 1);
            levelLayout[index].upConnected = roomInfo.downConnected;
        }
    }

    private void CreateStartRoom()
    {
        startRoom = InstantiateMetaRoom();
        levelLayout.Add(new()
        {
            metaRoom = startRoom,
            x = 0,
            y = 0,
            isCleared = true,
            type = RoomType.Spawn
        });
        lastGen = levelLayout[0];
        lastGenX = 0;
        lastGenY = 0;
        roomNumber = 0;
    }

    private void AddMetaRoom(RoomType type = RoomType.Combat)
    {
        List<Vector2> layoutPos = new();
        foreach (MetaRoomInfo info in levelLayout) layoutPos.Add(new(info.x, info.y));

        List<Vector2> outlinePos = new();
        foreach (Vector2 pos in layoutPos)
        {
            if (!HasRoomAt((int)pos.x + 1, (int)pos.y)) outlinePos.Add(pos + Vector2.right);
            if (!HasRoomAt((int)pos.x - 1, (int)pos.y)) outlinePos.Add(pos + Vector2.left);
            if (!HasRoomAt((int)pos.x, (int)pos.y + 1)) outlinePos.Add(pos + Vector2.up);
            if (!HasRoomAt((int)pos.x, (int)pos.y - 1)) outlinePos.Add(pos + Vector2.down);
        }

        Vector2 newPos = outlinePos[UnityEngine.Random.Range(0, outlinePos.Count)];
        bool connectUp = HasRoomAt((int)newPos.x, (int)newPos.y + 1);
        bool connectDown = !connectUp && HasRoomAt((int)newPos.x, (int)newPos.y - 1);
        bool connectRight = !connectUp && !connectDown && HasRoomAt((int)newPos.x + 1, (int)newPos.y);
        bool connectLeft = !connectUp && !connectDown && !connectRight && HasRoomAt((int)newPos.x - 1, (int)newPos.y);

        if (connectUp)
        {
            int index = GetIndexAtPos((int)newPos.x, (int)newPos.y + 1);
            levelLayout[index].downConnected = true;
        }
        else if (connectDown)
        {
            int index = GetIndexAtPos((int)newPos.x, (int)newPos.y - 1);
            levelLayout[index].upConnected = true;
        }
        else if (connectRight)
        {
            int index = GetIndexAtPos((int)newPos.x + 1, (int)newPos.y);
            levelLayout[index].leftConnected = true;
        }
        else if (connectLeft)
        {
            int index = GetIndexAtPos((int)newPos.x - 1, (int)newPos.y);
            levelLayout[index].rightConnected = true;
        }

        if (roomToGenerate != null) type = RoomType.Test;

        levelLayout.Add(new()
        {
            metaRoom = InstantiateMetaRoom(),
            x = (int)newPos.x,
            y = (int)newPos.y,
            isCleared = false,
            upConnected = connectUp,
            downConnected = connectDown,
            rightConnected = connectRight,
            leftConnected = connectLeft,
            type = type
        });
        lastGenX = (int)newPos.x;
        lastGenY = (int)newPos.y;
    }

    private MetaRoomInfo AddMetaRoomAtDirection(Direction direction, RoomType type)
    {
        MetaRoomInfo instance = new()
        {
            metaRoom = InstantiateMetaRoom(),
            roomNumber = type == RoomType.Combat ? lastGen.roomNumber + 1 : -1,
            x = lastGenX + ((direction == Direction.Right) ? 1 : (direction == Direction.Left) ? -1 : 0),
            y = lastGenY + ((direction == Direction.Up) ? 1 : (direction == Direction.Down) ? -1 : 0),
            isCleared = false,
            upConnected = direction == Direction.Down,
            downConnected = direction == Direction.Up,
            rightConnected = direction == Direction.Left,
            leftConnected = direction == Direction.Right,
            type = type
        };
        instance.metaRoom.gameObject.name = "NotGenerated";
        levelLayout.Add(instance);

        lastGen.upConnected = (lastGen.upConnected || direction == Direction.Up);
        lastGen.downConnected = (lastGen.downConnected || direction == Direction.Down);
        lastGen.rightConnected = (lastGen.rightConnected || direction == Direction.Right);
        lastGen.leftConnected = (lastGen.leftConnected || direction == Direction.Left);

        if (type == RoomType.Combat) generatedRoomCount += 1;

        return instance;
    }

    private MetaRoom InstantiateMetaRoom()
    {
        MetaRoom instance = Instantiate(Prefab.Get("MetaRoom")).GetComponent<MetaRoom>();
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform);
        return instance;
    }

    private bool HasRoomAt(int x, int y)
    {
        foreach (MetaRoomInfo info in levelLayout)
        {
            if (info.x == x && info.y == y)
            {
                return true;
            }
        }
        return false;
    }

    private int GetIndexAtPos(int x, int y)
    {
        for (int i = 0; i < levelLayout.Count; i++)
        {
            if (levelLayout[i].x == x && levelLayout[i].y == y) return i;
        }
        return -1;
    }
}
