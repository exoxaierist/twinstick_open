using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    public RoomSize type;
    public List<Door> doors = new();
    public List<ConnectionSocket> sockets = new();
    public List<EnemySpawner> spawners = new();
    public Tilemap roomAreaTilemap;
    public Tilemap pathBlockTilemap;

    [ContextMenu("Set Required Data")]
    public void ResetData()
    {
        doors.Clear();
        Door[] _ = GetComponentsInChildren<Door>();
        foreach (Door door in _)
        {
            door.canUse = true;
            door.offset = door.transform.position - transform.position;
            doors.Add(door);
        }
        spawners.Clear();
        spawners.AddRange(GetComponentsInChildren<EnemySpawner>());

        sockets.Clear();
        ConnectionSocket[] __ = GetComponentsInChildren<ConnectionSocket>();
        foreach (ConnectionSocket socket in __)
        {
            socket.offset = socket.transform.position - transform.position;
            sockets.Add(socket);
        }
    }

    public Door GetRandomDoor(Direction direction) => GetRandomDoor(direction, true);
    public Door GetRandomDoor(Direction direction, bool getAvailable)
    {
        List<Door> doorPickList = new();

        foreach (Door door in doors)
        {
            if (door.direction != direction) continue;
            if (getAvailable && !door.canUse) continue;
            doorPickList.Add(door);
        }
        if (doorPickList.Count == 0) return null;
        return doorPickList[Random.Range(0, doorPickList.Count)];
    }
    public Door GetRandomDoor(Direction direction, DoorType type)
    {
        List<Door> doorPickList = new();

        foreach (Door door in doors)
        {
            if (door.direction != direction || door.type != type) continue;
            doorPickList.Add(door);
        }
        if (doorPickList.Count == 0) return null;
        return doorPickList[Random.Range(0, doorPickList.Count)];
    }

    public List<Door> GetDoors(Direction direction, DoorType type)
    {
        List<Door> result = new();
        foreach (Door door in doors)
        {
            if (door.direction == direction && door.type == type) result.Add(door);
        }
        return result;
    }

    public bool HasDoor(Direction direction, DoorType type)
    {
        foreach (Door door in doors)
        {
            if (door.direction == direction && door.type == type) return true;
        }
        return false;
    }

    public ConnectionSocket GetRandomSocket()
    {
        return sockets[Random.Range(0, sockets.Count)];
    }
    public ConnectionSocket GetRandomSocket(Direction dir)
    {
        return sockets.Find(x => x.direction == dir);
    }
}
