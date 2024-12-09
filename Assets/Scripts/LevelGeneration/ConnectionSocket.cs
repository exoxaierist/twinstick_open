using UnityEditor;
using UnityEngine;

public class ConnectionSocket : MonoBehaviour
{
    public Room owningRoom;
    public bool canUse = true;
    public Direction direction;
    public Vector2 offset;
    public int length = 1;

    public int appliedOffset;
    public int appliedLength;

    private void OnDrawGizmos()
    {
        Vector2 dir = Vector2.zero;
        Vector2 center = Vector2.zero;
        Vector2 boxSize;
        float boxWidth;

        Gizmos.color = new(0, 1, 0, 0.3f);
        boxWidth = 0.5f;
        if (direction.IsVertical()) boxSize = new(length, boxWidth);
        else boxSize = new(boxWidth, length);

        //draw arrow
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
        Gizmos.DrawLine(center + dir * boxWidth * 0.5f, center + dir);
        Gizmos.DrawLine(center + dir, center + (dir * 2 + new Vector2(dir.y, -dir.x)) * 0.3f);
        Gizmos.DrawLine(center + dir, center + (dir * 2 + new Vector2(-dir.y, dir.x)) * 0.3f);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ConnectionSocket))]
public class ConnectionSocketEditor : Editor
{
    private void OnSceneGUI()
    {
        ConnectionSocket socket = (ConnectionSocket)target;
        Transform socketTransform = socket.transform;

        // Get the direction vector based on the socket's direction
        Vector3 dir = GetAreaDirection(socket.direction);

        // Calculate the handle position
        Vector3 handlePosition = socketTransform.position + dir * (socket.length - 0.5f) - socket.direction.ToVector() * 0.5f;

        // Draw the handle and constrain its movement
        EditorGUI.BeginChangeCheck();
        Vector3 newHandlePosition = Handles.Slider(handlePosition, dir, 0.2f, Handles.SphereHandleCap, 0.5f);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(socket, "Change Socket Length");

            // Calculate the new length based on the handle's position
            float newLength = Vector3.Dot(newHandlePosition - socketTransform.position, dir);
            socket.length = Mathf.Max(0, Mathf.RoundToInt(newLength)) + 1;
        }
    }

    private Vector3 GetAreaDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.right;
            case Direction.Down:
                return Vector3.right;
            case Direction.Left:
                return Vector3.down;
            case Direction.Right:
                return Vector3.down;
            default:
                return Vector3.zero;
        }
    }
}
#endif