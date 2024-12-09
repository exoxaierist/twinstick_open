using UnityEngine;
using UnityEngine.Tilemaps;

public class RuleTileExtended : RuleTile
{
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        bool temp = base.StartUp(position, tilemap, go);
        if (go != null) go.transform.position = position + new Vector3(0.5f,0,0);
        return temp;
    }
}
