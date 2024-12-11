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

    public Room GetSpawnRoom() => spawnRooms.GetRandom();
    public Room GetShopRoom() => shopRooms.GetRandom();
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
    }
}
