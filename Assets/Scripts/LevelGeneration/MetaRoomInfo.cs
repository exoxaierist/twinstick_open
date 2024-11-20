using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaRoomInfo
{
    public MetaRoom metaRoom;
    public MinimapBlock minimapBlock;

    public int roomNumber;
    public int x;
    public int y;
    public bool isCleared = true;

    public bool upConnected;
    public bool downConnected;
    public bool rightConnected;
    public bool leftConnected;

    public RoomType type;
}
