using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public static bool inLevel = false;
    public const float roomGap = 100;

    //stage change
    public int stage2Start = 15;
    public int stage3Start = 25;
    public RoomList stage1List;
    public Texture stage1Texture;
    public Color stage1Color;
    public RoomList stage2List;
    public Texture stage2Texture;
    public Color stage2Color;
    public RoomList stage3List;
    public Texture stage3Texture;
    public Color stage3Color;
    public Material tilemapMaterial;

    public Action<MetaRoomInfo> onRoomChange;

    //room infos
    private List<MetaRoomInfo> levelLayout = new();
    public static MetaRoom currentRoom;
    public static int currentRoomNumber = 0;
    [HideInInspector] public Vector2Int currentPos = new();
    private MetaRoom startRoom;
    private MetaRoomInfo lastGen;
    private int lastGenX = 0;
    private int lastGenY = 0;
    private int generatedRoomCount = 0;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init()
    {
        currentRoomNumber = 0;
        inLevel = false;
        SceneManager.sceneLoaded += Init;
    }
    public static void Init(Scene _, LoadSceneMode __) => Init();

    private void Awake()
    {
        main = this;
        currentRoom = null;
    }

    public void StartLevel(Action onGenerateFinish)
    {
        InitializeLayout(); 
        SetPlayerStart();

        tilemapMaterial.SetTexture("_Tex", stage1Texture);
        Camera.main.backgroundColor = stage1Color;
        this.DelayFrame(() => onGenerateFinish?.Invoke());

        /* alt layout, unused
        CreateLayout();
        this.Delay(0.1f, () => {
            GenerateMetaRooms();
            CreateMinimap();
            SetPlayerStart();
            onGenerateFinish?.Invoke();
        });*/
    }

    public void ChangeRoom(MetaRoom from, Direction direction)
    {
        //get MetaRoom to
        int toIndex = 0;
        for (int i = 0; i < levelLayout.Count; i++)
        {
            MetaRoomInfo info = levelLayout[i];
            if (info.metaRoom == from)
            {
                //fromIndex = i;
                if (direction == Direction.Up) toIndex = GetLayoutIndexAtPos(info.x, info.y + 1);
                else if (direction == Direction.Down) toIndex = GetLayoutIndexAtPos(info.x, info.y - 1);
                else if (direction == Direction.Right) toIndex = GetLayoutIndexAtPos(info.x + 1, info.y);
                else if (direction == Direction.Left) toIndex = GetLayoutIndexAtPos(info.x - 1, info.y);
                break;
            }
        }
        //change room if either one is shop
        if(currentRoomNumber <= 0 || levelLayout[toIndex].type == RoomType.Shop || from.info.type == RoomType.Shop)
        {
            //change room to shop or etc
            ChangeRoomImmediate(from, levelLayout[toIndex], direction);
        }
        else
        {
            //room difficulty
            if(currentRoomNumber <= 10)
            {
                PlayerStats.waveChance += 0.06f;
                PlayerStats.enemyHpMul += 0.075f;
                PlayerStats.waveEnemyCount = 10;
            }
            else if(currentRoomNumber <= 20)
            {
                PlayerStats.waveChance += 0.06f;
                PlayerStats.enemyHpMul += 0.1f;
                PlayerStats.waveEnemyCount = 13;
            }
            else if (currentRoomNumber <= 30)
            {
                PlayerStats.waveChance += 0.06f;
                PlayerStats.enemyHpMul += 0.125f;
                PlayerStats.waveEnemyCount = 15;
            }

            //pick perk and change room
            UIManager.main.HideHud();
            PerkPicker.Pick(() =>
            {
                ChangeRoomImmediate(from, levelLayout[toIndex], direction);
                UIManager.main.ShowHud();
            });
        } 
    }

    public void ChangeRoomImmediate(MetaRoom from, MetaRoomInfo toInfo, Direction direction)
    {
        MetaRoom to = toInfo.metaRoom;
        currentRoom = to;
        currentPos = new(toInfo.x, toInfo.y);
        currentRoomNumber = toInfo.roomNumber;

        from.gameObject.SetActive(false);
        to.gameObject.SetActive(true);

        //move player
        Vector2 playerPos = new();
        if (direction == Direction.Up) playerPos = to.portalDown.playerSpawn.position;
        else if (direction == Direction.Down) playerPos = to.portalUp.playerSpawn.position;
        else if (direction == Direction.Right) playerPos = to.portalLeft.playerSpawn.position;
        else if (direction == Direction.Left) playerPos = to.portalRight.playerSpawn.position;
        Player.main.MoveRoom(direction, playerPos);

        to.OnRoomEnter();
        onRoomChange?.Invoke(toInfo);

        //set stage visuals
        if(currentRoomNumber == stage3Start)
        {
            tilemapMaterial.SetTexture("_Tex", stage3Texture);
            Camera.main.backgroundColor = stage3Color;
        }
        else if(currentRoomNumber == stage2Start)
        {
            tilemapMaterial.SetTexture("_Tex", stage2Texture);
            Camera.main.backgroundColor = stage2Color;
        }

        //create next room
        if (currentRoomNumber >= generatedRoomCount - 1) CreateNextRoom();
        Utility.GetMono().DelayFrame(() => { 
            UIManager.main.FadeOut(0.4f);
        });
    }

    private void InitializeLayout()
    {
        CreateStartRoom();
        for (int i = 0; i < 3; i++)
        {
            CreateNextRoom();
        }
    }

    private void CreateNextRoom()
    {
        MetaRoomInfo roomInfo = AddMetaRoomAtDirection(Direction.Up, RoomType.Combat);
        int chamberNumber = generatedRoomCount;
        //boss room
        if(chamberNumber > 0 && chamberNumber % 10 == 0)
        //if(chamberNumber == 1)
        {
            roomInfo.type = RoomType.Boss;
            roomInfo.bossId = Boss.GetRandomId();
            //roomInfo.bossId = "BOSS_AMALGAMATION";
        }
        //shop room
        else if (chamberNumber>1 && (chamberNumber+1) % 3 == 0)
        {
            //add shop
            MetaRoomInfo shopRoomInfo = AddMetaRoomAtDirection(Direction.Right, RoomType.Shop);
            shopRoomInfo.metaRoom.BuildRoom(shopRoomInfo);
        }
        //build previously generated room
        lastGen.metaRoom.BuildRoom(lastGen);
        lastGen = roomInfo;
        lastGenX = roomInfo.x;
        lastGenY = roomInfo.y;
    }

    //set player initial spawn position
    private void SetPlayerStart()
    {
        inLevel = true;
        startRoom.GetComponentInChildren<PlayerStart>().SetStart();
        startRoom.gameObject.SetActive(true);
        currentRoom = startRoom;
        startRoom.roomEnterCount = 1;
        onRoomChange?.Invoke(levelLayout[0]);
    }

    //create spawn room and reset variables
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
        startRoom.roomList = stage1List;
        lastGen = levelLayout[0];
        lastGenX = 0;
        lastGenY = 0;
        currentRoomNumber = 0;
    }

    private MetaRoomInfo AddMetaRoomAtDirection(Direction direction, RoomType type)
    {
        MetaRoomInfo instance = new()
        {
            metaRoom = InstantiateMetaRoom(),
            roomNumber = lastGen.roomNumber + (type == RoomType.Combat ? 1 : 0),
            x = lastGenX + ((direction == Direction.Right) ? 1 : (direction == Direction.Left) ? -1 : 0),
            y = lastGenY + ((direction == Direction.Up) ? 1 : (direction == Direction.Down) ? -1 : 0),
            isCleared = false,
            upConnected = direction == Direction.Down,
            downConnected = direction == Direction.Up,
            rightConnected = direction == Direction.Left,
            leftConnected = direction == Direction.Right,
            type = type
        };
        //assign RoomList to use for generation
        instance.metaRoom.roomList = stage1List;
        if (type == RoomType.Combat)
        {
            if (instance.roomNumber >= stage3Start) instance.metaRoom.roomList = stage3List;
            else if (instance.roomNumber >= stage2Start) instance.metaRoom.roomList = stage2List;
        }
        instance.metaRoom.gameObject.name = "NotGenerated " + instance.roomNumber;
        levelLayout.Add(instance);

        lastGen.upConnected = (lastGen.upConnected || direction == Direction.Up);
        lastGen.downConnected = (lastGen.downConnected || direction == Direction.Down);
        lastGen.rightConnected = (lastGen.rightConnected || direction == Direction.Right);
        lastGen.leftConnected = (lastGen.leftConnected || direction == Direction.Left);

        if (type == RoomType.Combat) generatedRoomCount += 1;

        //remove prev room
        if(generatedRoomCount > 3)
        {
            Destroy(levelLayout[generatedRoomCount-4].metaRoom.gameObject);
        }

        return instance;
    }

    private MetaRoom InstantiateMetaRoom()
    {
        MetaRoom instance = Instantiate(Prefab.Get("MetaRoom")).GetComponent<MetaRoom>();
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform);
        return instance;
    }

    private int GetLayoutIndexAtPos(int x, int y)
    {
        for (int i = 0; i < levelLayout.Count; i++)
        {
            if (levelLayout[i].x == x && levelLayout[i].y == y) return i;
        }
        return -1;
    }

    /*public void EndLevel()
    {
        DestroyLevel();
        level++;
    }*/

    /*private void DestroyLevel()
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
    }*/

    /*private void GenerateMetaRooms()
    {
        foreach (MetaRoomInfo info in levelLayout)
        {
            info.metaRoom.BuildRoom(info, roomToGenerate);
        }
    }*/

    /*public void CheckLevelClear()
    {
        if (LevelFinish.isEnabled) return;
        foreach (MetaRoomInfo info in levelLayout)
        {
            if (!info.isCleared) return;
        }
        // on clear
        LevelFinish.EnableFinish();
    }*/

    /*private void CreateLayout()
    {
        CreateStartRoom();
        for (int i = 0; i < roomCount; i++)
        {
            *//*if(i==layoutCount - 2)
            {
                AddMetaRoomToLayout(RoomType.Shop);
                continue;
            }
            if (i == layoutCount - 1)
            {
                AddMetaRoomToLayout(RoomType.Finish);
                continue;
            }*//*
            AddMetaRoom();
        }
        SetConnectionsAll();
    }*/

    /*private void CreateMinimap()
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
        foreach (MetaRoomInfo info in levelLayout)
        {
            MinimapBlock minimapBlock = Instantiate(Prefab.Get("MinimapBlock")).GetComponent<MinimapBlock>();
            minimapBlock.transform.SetParent(UIManager.main.minimapParent);
            minimapBlock.transform.localPosition = new((info.x - center.x) * UIManager.main.pixelsPerUnit, (info.y - center.y) * UIManager.main.pixelsPerUnit);
            minimapBlock.transform.localScale = Vector3.one;
            onRoomChange += minimapBlock.OnRoomChange;
            info.minimapBlock = minimapBlock;
            minimapBlock.SetVisual(info);
        }
        levelLayout[0].minimapBlock.IsCurrentRoom(true);
    }*/

    /*private void SetConnectionsAll()
    {
        //connect all
        foreach(MetaRoomInfo info in levelLayout)
        {
            SetConnections(info);
        }
    }*/

    /*private void SetConnections(MetaRoomInfo roomInfo)
    {
        if (!roomInfo.rightConnected && HasRoomAt(roomInfo.x + 1, roomInfo.y))
        {
            roomInfo.rightConnected = UnityEngine.Random.Range(-2f, 1f) > 0;
            int index = GetLayoutIndexAtPos(roomInfo.x + 1, roomInfo.y);
            levelLayout[index].leftConnected = roomInfo.rightConnected;
        }
        if (!roomInfo.downConnected && HasRoomAt(roomInfo.x, roomInfo.y - 1))
        {
            roomInfo.downConnected = UnityEngine.Random.Range(-2f, 1f) > 0;
            int index = GetLayoutIndexAtPos(roomInfo.x, roomInfo.y - 1);
            levelLayout[index].upConnected = roomInfo.downConnected;
        }
    }*/

    /*private void AddMetaRoom(RoomType type = RoomType.Combat)
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
            int index = GetLayoutIndexAtPos((int)newPos.x, (int)newPos.y + 1);
            levelLayout[index].downConnected = true;
        }
        else if (connectDown)
        {
            int index = GetLayoutIndexAtPos((int)newPos.x, (int)newPos.y - 1);
            levelLayout[index].upConnected = true;
        }
        else if (connectRight)
        {
            int index = GetLayoutIndexAtPos((int)newPos.x + 1, (int)newPos.y);
            levelLayout[index].leftConnected = true;
        }
        else if (connectLeft)
        {
            int index = GetLayoutIndexAtPos((int)newPos.x - 1, (int)newPos.y);
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
    }*/

    /*private bool HasRoomAt(int x, int y)
    {
        foreach (MetaRoomInfo info in levelLayout)
        {
            if (info.x == x && info.y == y)
            {
                return true;
            }
        }
        return false;
    }*/
}
