using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    public List<ConnectionSocket> sockets = new();
    public Tilemap floorTilemap;
    public Tilemap pathBlockTilemap;

    [ContextMenu("Set Data")]
    public void ResetData()
    {
        //find tilemaps
        floorTilemap = GetComponentsInChildren<Tilemap>().FirstOrDefault(x => x.gameObject.name == "Floor");
        pathBlockTilemap = GetComponentsInChildren<Tilemap>().FirstOrDefault(x => x.gameObject.name == "pit");

        //find sockets
        sockets.Clear();
        ConnectionSocket[] __ = GetComponentsInChildren<ConnectionSocket>();
        foreach (ConnectionSocket socket in __)
        {
            socket.offset = socket.transform.position - transform.position;
            sockets.Add(socket);
        }
    }

    public ConnectionSocket GetRandomSocket() => sockets.GetRandom();
    public ConnectionSocket GetRandomSocket(Direction dir) => sockets.Where(x=>x.direction == dir).ToArray().GetRandom();
}
