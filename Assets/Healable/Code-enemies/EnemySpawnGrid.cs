using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Grid))]

public class EnemySpawnGrid : MonoBehaviour
{
    private static readonly Vector2Int[] _directions =
    {
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1),
        new(-1, -1), new(-1, 1), new(1, -1), new(1, 1)
    };

    private readonly HashSet<Vector2Int> _reservedSpawnCells = new();
    private Grid _grid;
    private float _cellSize;
    private int _width;
    private int _height;
    [SerializeField] private BoxCollider2D _Bounds;
    private LayerMask _obstacleMask;

    public void Configure(Bounds bounds, float requestedCellSize, LayerMask requestedObstacleMask)
    {
        _grid = GetComponent<Grid>();
        _cellSize = Mathf.Max(0.10f, requestedCellSize);
        _width = Mathf.Max(1, Mathf.FloorToInt(bounds.size.x / _cellSize));
        _height = Mathf.Max(1, Mathf.FloorToInt(bounds.size.y / _cellSize));
        _obstacleMask = requestedObstacleMask;

        _grid.cellSize = new Vector3(_cellSize, _cellSize, 1f);

        transform.position = new Vector3(
            bounds.min.x + _cellSize * 0.5f,
            bounds.min.y + _cellSize * 0.5f,
            0f
        );

        _reservedSpawnCells.Clear();
    }

    public bool TryGetRandomFreeCell(out Vector2 position)
    {
        List<Vector2Int> validCells = new();
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
            {
                Vector2Int cell = new(x, y);
                if (!_reservedSpawnCells.Contains(cell) && IsWalkable(cell))
                {
                    validCells.Add(cell);
                }
            }

        if (validCells.Count == 0)
        {
            position = default;
            return false;
        }

        Vector2Int selected = validCells[Random.Range(0, validCells.Count)];
        _reservedSpawnCells.Add(selected);
        position = CellCenter(selected);
        return true;
    }

    public bool TryGetPath(Vector2 from, Vector2 to, List<Vector2> outputPath)
    {
        outputPath.Clear();
        Vector2Int start = FindClosestWalkable(WorldToCell(from));
        Vector2Int goal = FindClosestWalkable(WorldToCell(to));
        if (!IsInside(start) || !IsInside(goal))
        {
            return false;
        }

        List<Vector2Int> open = new() { start };
        HashSet<Vector2Int> closed = new();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        Dictionary<Vector2Int, float> gScore = new() { [start] = 0f };
        Dictionary<Vector2Int, float> fScore = new() { [start] = Heuristic(start, goal) };

        while (open.Count > 0)
        {
            int best = 0;
            for (int i = 1; i < open.Count; i++)
                if (fScore.GetValueOrDefault(open[i], float.MaxValue) < fScore.GetValueOrDefault(open[best], float.MaxValue))
                {
                    best = i;
                }

            Vector2Int current = open[best];
            if (current == goal)
            {
                ReconstructPath(cameFrom, current, outputPath);
                return true;
            }

            open.RemoveAt(best);
            closed.Add(current);
            foreach (Vector2Int direction in _directions)
            {
                Vector2Int next = current + direction;
                if (!IsWalkable(next) || closed.Contains(next))
                {
                    continue;
                }

                bool diagonal = direction.x != 0 && direction.y != 0;
                if (diagonal && (!IsWalkable(current + new Vector2Int(direction.x, 0)) || !IsWalkable(current + new Vector2Int(0, direction.y))))
                {
                    continue;
                }

                float tentativeG = gScore[current] + (diagonal ? 1.41421356f : 1f);
                if (tentativeG >= gScore.GetValueOrDefault(next, float.MaxValue))
                {
                    continue;
                }

                cameFrom[next] = current;
                gScore[next] = tentativeG;
                fScore[next] = tentativeG + Heuristic(next, goal);
                if (!open.Contains(next))
                {
                    open.Add(next);
                }
            }
        }

        return false;
    }

    private static float Heuristic(Vector2Int a, Vector2Int b) => Vector2Int.Distance(a, b);

    private Vector2Int FindClosestWalkable(Vector2Int origin)
    {
        if (IsWalkable(origin))
        {
            return origin;
        }

        int maximumDistance = Mathf.Max(_width, _height);
        for (int distance = 1; distance <= maximumDistance; distance++)
            for (int y = -distance; y <= distance; y++)
                for (int x = -distance; x <= distance; x++)
                {
                    if (Mathf.Abs(x) != distance && Mathf.Abs(y) != distance)
                    {
                        continue;
                    }

                    Vector2Int candidate = origin + new Vector2Int(x, y);
                    if (IsWalkable(candidate))
                    {
                        return candidate;
                    }
                }

        return new Vector2Int(-1, -1);
    }

    private void ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current, List<Vector2> outputPath)
    {
        List<Vector2> reversePath = new() { CellCenter(current) };
        while (cameFrom.TryGetValue(current, out Vector2Int previous))
        {
            current = previous;
            reversePath.Add(CellCenter(current));
        }
        reversePath.Reverse();
        outputPath.AddRange(reversePath);
    }

    private bool IsWalkable(Vector2Int cell)
    {
        if (!IsInside(cell))
        {
            return false;
        }

        if (_obstacleMask.value == 0)
        {
            return true;
        }

        return Physics2D.OverlapCircle(CellCenter(cell), _cellSize * 0.35f, _obstacleMask) == null;
    }

    private bool IsInside(Vector2Int cell) => cell.x >= 0 && cell.x < _width && cell.y >= 0 && cell.y < _height;
    private Vector2Int WorldToCell(Vector2 worldPosition)
    {
        Vector3Int cell = _grid.WorldToCell(worldPosition);
        return new Vector2Int(cell.x, cell.y);
    }
    private Vector2 CellCenter(Vector2Int cell) => _grid.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
}
