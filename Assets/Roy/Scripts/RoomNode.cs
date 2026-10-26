using UnityEngine;
using System.Collections.Generic;

public class RoomNode 
{
    public Vector2Int GridPosition;
    public RoomType Type = RoomType.Normal;

    public HashSet<Vector2Int> Connections = new HashSet<Vector2Int>();

    public RoomNode(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
    }
}
