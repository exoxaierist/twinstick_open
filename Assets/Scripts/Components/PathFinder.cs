using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    public static Tilemap ceilingTilemap;
    public static Tilemap pathBlockTilemap;
    [HideInInspector] public List<Vector2> path = new();
    [HideInInspector] public bool optimizePath = true;

    private List<Node> openList = new();
    private HashSet<Node> closedList = new();
    private Dictionary<Vector3Int, Node> allNodes = new();
    private Vector2 targetPos; 

    [HideInInspector] public Vector3Int[] neighborOffsets = new Vector3Int[]
        {
            new (0, 1, 0),
            new (1, 0, 0),
            new (0, -1, 0),
            new (-1, 0, 0)
        };

    public void FindPath(Vector2 target)
    {
        if (!IsWalkable(ceilingTilemap.WorldToCell(transform.position)) || !IsWalkable(ceilingTilemap.WorldToCell(target))) { return; }

        if (optimizePath)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.2f, (Vector3)target - transform.position, Vector2.Distance(transform.position, target), LayerMask.GetMask("WorldStatic","PawnBlock"));
            if (!hit)
            {
                path.Clear();
                path.Add(transform.position);
                path.Add(target);
                DrawPath();
                return;
            }
        }

        Node startNode = Node.Get(ceilingTilemap.WorldToCell(transform.position));
        Node endNode = Node.Get(ceilingTilemap.WorldToCell(target));
        targetPos = target;

        openList.Clear();
        closedList.Clear();
        allNodes.Clear();

        openList.Add(startNode);
        allNodes[startNode.position] = startNode;


        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode();
            Debug.DrawRay(currentNode.position, Vector2.up * 0.1f, Color.red, 0.1f);
            if (currentNode.position == endNode.position)
            {
                RetracePath(startNode, currentNode);
                return;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Vector3Int neighborPos in GetNeighbors(currentNode.position))
            {
                if (closedList.Contains(GetNode(neighborPos))) continue;

                Node neighborNode = GetNode(neighborPos);
                float newGCost = currentNode.gCost + GetDistance(currentNode, neighborNode);
                if (newGCost < neighborNode.gCost || !openList.Contains(neighborNode))
                {
                    neighborNode.gCost = newGCost;
                    neighborNode.hCost = GetDistance(neighborNode, endNode);
                    neighborNode.parent = currentNode;

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }
        ReturnAllNodes();
    }

    public Vector2 GetReachablePosition()
    {
        if (path.Count == 0) 
        {
            return transform.position;
        }

        if (!optimizePath) return path[0];
        Vector2 reachablePos = path[0];
        Vector2 from = transform.position;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 to = path[i];
            RaycastHit2D hit = Physics2D.CircleCast(from, 0.2f, to - from, Vector2.Distance(from, to), LayerMask.GetMask("WorldStatic","PawnBlock"));
            if (!hit) reachablePos = to;
            else break;
        }
        return reachablePos;
    }

    public Vector2 GetDirection()
    {
        return GetReachablePosition() - (Vector2)transform.position;
    }

    private Node GetLowestFCostNode()
    {
        Node lowestFCostNode = openList[0];
        foreach (Node node in openList)
        {
            if (node.FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = node;
            }
        }
        return lowestFCostNode;
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> newPath = new();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
/*
            Vector2 from = (Vector2)ceilingTilemap.CellToWorld(currentNode.position) + new Vector2(0.5f, 0.5f) + currentNode.offset;
            Vector2 to = (Vector2)ceilingTilemap.CellToWorld(currentNode.parent.position) + new Vector2(0.5f, 0.5f) + currentNode.offset;
            RaycastHit2D hit = Physics2D.CircleCast(from, 0.1f, to - from, Vector2.Distance(from, to), LayerMask.GetMask("WorldStatic", "PawnBlock"));
            if(!hit) newPath.Add((Vector2)ceilingTilemap.CellToWorld(currentNode.position) + new Vector2(0.5f,0.5f) + currentNode.offset);
            */
            newPath.Add((Vector2)ceilingTilemap.CellToWorld(currentNode.position) + new Vector2(0.5f,0.5f) + currentNode.offset);
            currentNode = currentNode.parent;
        }
        newPath.Reverse();

        path = newPath;
        DrawPath();
    }

    private void DrawPath()
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.cyan, 0.5f);
        }
    }

    private List<Vector3Int> GetNeighbors(Vector3Int nodePosition)
    {
        List<Vector3Int> neighbors = new();

        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborPos = nodePosition + offset;
            if(IsWalkable(neighborPos)) neighbors.Add(neighborPos);
        }

        return neighbors;
    }

    private Node GetNode(Vector3Int position)
    {
        if (allNodes.ContainsKey(position))
        {
            return allNodes[position];
        }
        else
        {
            Node newNode = Node.Get(position);
            allNodes[position] = newNode;
            if (ceilingTilemap.HasTile(position + Vector3Int.down))
            {
                newNode.offset = new(0, 0.5f);
            }
            return newNode;
        }
    }

    private bool IsWalkable(Vector3Int position)
    {
        return !ceilingTilemap.HasTile(position) && !pathBlockTilemap.HasTile(position)
            && Physics2D.OverlapPoint(ceilingTilemap.CellToWorld(position) + new Vector3(0.5f,0.6f), LayerMask.GetMask("PawnBlock","WorldStatic")) == null;
    }

    private float GetDistance(Node a, Node b)
    {
        return Vector3Int.Distance(a.position, b.position);
    }

    private void ReturnAllNodes()
    {
        foreach (Node node in allNodes.Values) node.Return();
    }

    public class Node
    {
        public Vector2 offset;
        public Vector3Int position;
        public float gCost;
        public float hCost;
        public Node parent;

        public Node(Vector3Int pos)
        {
            position = pos;
        }

        public float FCost
        {
            get { return gCost + hCost; }
        }

        //pooling
        private static List<Node> pool = new();
        public static Node Get(Vector3Int pos)
        {
            if (pool.Count == 0) return new Node(pos);
            Node instance = pool[0];
            instance.position = pos;
            instance.offset = Vector2.zero;
            instance.parent = null;
            instance.gCost = 0;
            instance.hCost = 0;
            return instance;
        }
        private static void ReturnNode(Node node)
        {
            pool.Add(node);
        }
        public void Return() => ReturnNode(this);
    }
}
