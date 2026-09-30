using UnityEngine;
using System.Collections.Generic;

public class RoomNode 
{
    public Vector2Int gridPos;
    public RoomType type = RoomType.Normal;

    public HashSet<Vector2Int> connections = new HashSet<Vector2Int>();

    public RoomNode(Vector2Int pos)
    {
        gridPos = pos;
    }
}
