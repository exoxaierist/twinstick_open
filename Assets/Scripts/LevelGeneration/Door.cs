using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool canUse = true;
    public Direction direction;
    public DoorType type;
    public Vector2 offset;

    private void OnDrawGizmos()
    {
        Vector2 dir = Vector2.zero;
        Vector2 center = Vector2.zero;
        Vector2 boxSize = Vector2.zero;
        float boxWidth = 0.3f;
        switch (type)
        {
            case DoorType.Narrow:
                Gizmos.color = new(0, 1, 0, 0.3f);
                boxWidth = 0.6f;
                if (direction == Direction.Up || direction == Direction.Down) boxSize = new(2, boxWidth);
                else boxSize = new(boxWidth, 3);
                break;
            case DoorType.Wide:
                Gizmos.color = new(1, 0.2f, 0, 0.5f);
                boxWidth = 0.3f;
                if (direction == Direction.Up || direction == Direction.Down) boxSize = new(4,boxWidth);
                else boxSize = new(boxWidth, 5);
                break;
            case DoorType.SuperWide:
                Gizmos.color = new(0, 0.2f, 1, 0.6f);
                boxWidth = 0.2f;
                if (direction == Direction.Up || direction == Direction.Down) boxSize = new(6, boxWidth);
                else boxSize = new(boxWidth, 7);
                break;
        }
        switch (direction)
        {
            case Direction.Up:
                center.x = boxSize.x * 0.5f - 0.5f;
                center.y = -0.5f + boxSize.y * 0.5f;
                dir = Vector2.up;
                break;
            case Direction.Right:
                center.x = -0.5f + boxSize.x * 0.5f;
                center.y = -boxSize.y * 0.5f + 0.5f;
                dir = Vector2.right;
                break;
            case Direction.Down:
                center.x = boxSize.x * 0.5f - 0.5f;
                center.y = 0.5f - boxSize.y * 0.5f;
                dir = Vector2.down;
                break;
            case Direction.Left:
                center.x = 0.5f - boxSize.x * 0.5f;
                center.y = -boxSize.y * 0.5f + 0.5f;
                dir = Vector2.left;
                break;
            default:
                break;
        }
        center += (Vector2)transform.position;
        Gizmos.DrawCube(center, boxSize);
        Gizmos.DrawLine(center + dir * boxWidth*0.5f, center + dir);
        Gizmos.DrawLine(center + dir, center + (dir*2 + new Vector2(dir.y,-dir.x)) * 0.3f);
        Gizmos.DrawLine(center + dir, center + (dir*2 + new Vector2(-dir.y,dir.x)) * 0.3f);
    }
}
