using UnityEngine;
using System.Collections.Generic;

public class RoomNode 
{
    public Vector2Int gridPos; // Usar PascalCase; Sin abreviaciones
    public RoomType type = RoomType.Normal; // Usar PascalCase

    public HashSet<Vector2Int> connections = new HashSet<Vector2Int>(); // Usar PascalCase

    public RoomNode(Vector2Int pos) // Sin abreviaciones
    {
        gridPos = pos;
    }
}
