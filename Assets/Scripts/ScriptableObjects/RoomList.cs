using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomList",menuName ="Room List")]
public class RoomList : ScriptableObject
{
    [Header("Special")]
    public List<Room> spawnRooms;
    public List<Room> shopRooms;
    public List<Room> mainRooms;
    public List<Room> smallRooms;
    private HashSet<Room> appearedMain = new();

    [Header("Portals")]
    public Room upPortal;
    public Room downPortal;
    public Room rightPortal;
    public Room leftPortal;

    [Header("Rooms")]
    public List<Room> rooms = new();
    private List<int> testedIndex = new();
    private int undoIndex = 0;

    public Room GetSpawnRoom() => spawnRooms[Random.Range(0, spawnRooms.Count)];

    public Room GetShopRoom() => shopRooms[Random.Range(0, shopRooms.Count)];

    public Room GetFinishRoom()
    {
        return null;
    }

    public Room GetRandomRoom(RoomSize type)
    {
        if(type == RoomSize.Main)
        {
            Room result = mainRooms.Except(appearedMain).ToArray().GetRandom();
            appearedMain.Add(result);
            if (appearedMain.Count == mainRooms.Count) appearedMain.Clear();
            return result;
        }
        else
        {
            return smallRooms[Random.Range(0, smallRooms.Count)];
        }


        /*for (int i = 0; i < 100; i++)
        {
            Room room = GetRandomRoom();
            if (room.type == type) return room;
        }
        return GetRandomRoom();*/
    }

    public Room GetRandomRoom()
    {
        return rooms[GetRandomIndex()];
    }

    public void UndoRandomPick()
    {
        testedIndex.Add(undoIndex);
    }

    private int GetRandomIndex()
    {
        if (testedIndex.Count == 0 || testedIndex.Count > rooms.Count) ResetRandomIndex();
        int randomIndex = Random.Range(0, testedIndex.Count);
        undoIndex = testedIndex[randomIndex];
        testedIndex.RemoveAt(randomIndex);
        return undoIndex;
    }

    private void ResetRandomIndex()
    {
        testedIndex.Clear();
        for (int i = 0; i < rooms.Count; i++)
        {
            testedIndex.Add(i);
        }
    }

    [ContextMenu("reset stuff")]
    public void ResetStuff()
    {
        testedIndex.Clear();
    }
}
