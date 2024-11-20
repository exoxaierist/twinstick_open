using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : Room
{
    public Transform playerSpawn;
    public Direction direction;

    private void OnDrawGizmos()
    {
        if (playerSpawn == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(playerSpawn.position, 0.5f);
    }

    private void OnEnable()
    {
        pathBlockTilemap = roomAreaTilemap;
    }

    public void Lock() => GetComponentInChildren<PortalDoor>().Lock();
    public void Open() => GetComponentInChildren<PortalDoor>().Open();
}
