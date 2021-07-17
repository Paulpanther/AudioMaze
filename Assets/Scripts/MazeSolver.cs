using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeSolver : MonoBehaviour
{

    private class DistanceData
    {
        public bool visited;
        public int distance = int.MaxValue;
        public SpaceNode previous;
    }
    
    private class SpaceNode
    {
        public Vector3Int pos;
        public List<SpaceNode> neighbors;
        public readonly Dictionary<Transform, DistanceData> distances = new Dictionary<Transform, DistanceData>();

        public SpaceNode(Vector3Int pos)
        {
            this.pos = pos;
        }
    }

    public Transform[] startObjects;

    private Tilemap _map;
    private readonly Dictionary<Vector3Int, SpaceNode> _nodes = new Dictionary<Vector3Int, SpaceNode>();

    private static readonly Vector3Int[] _relativeNeighbors = {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0),
    };

    private static readonly Vector3Int[] _extendedRelativeNeighbors = new [] {
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, -1, 0),
    }.Concat(_relativeNeighbors).ToArray();


    private void Awake()
    {
        _map = GetComponentInChildren<Tilemap>();
        var origin = _map.origin;
        var size = _map.size;
        
        // Find all space nodes
        for (var y = origin.y; y < origin.y + size.y; y++)
        {
            for (var x = origin.x; x < origin.x + size.x; x++)
            {
                var pos = new Vector3Int(x, y, 0);
                if (!_map.HasTile(pos))
                {
                    _nodes.Add(pos, new SpaceNode(pos));
                }
            }
        }

        // Calculate neighbors of space nodes
        foreach (var pos in _nodes.Keys)
        {
            var node = _nodes[pos];
            node.neighbors = GetNeighborsOf(pos);
        }

        // Find distances for each object
        foreach (var obj in startObjects)
        {
            // Find start
            var start = GetNeighborOfStart(obj);
            if (start == null)
            {
                throw new ApplicationException("Transform object hast no neighbor spaces");
            }
            
            // Prepare all spaces
            foreach (var node in _nodes.Values)
            {
                node.distances.Add(obj, new DistanceData());
            }
            
            // Calculate Distances
            CalculateDistances(start, obj);
        }
    }

    private Vector3 CellCenterToWorld(Vector3Int cell)
    {
        return _map.LocalToWorld(_map.CellToLocalInterpolated(cell + new Vector3(0.5f, 0.5f, 0)));
    }

    public Vector3 GetBestDirectionFor(Transform obj, Vector3 worldPos)
    {
        var cell = _map.WorldToCell(worldPos);
        var neighbors = _extendedRelativeNeighbors.Select(n => cell + n).ToList();
        var neighborScores = neighbors.Select(n => GetDistanceFrom(obj, n) ?? int.MaxValue).ToList();
        var min = neighborScores.Min();
        var index = neighborScores.IndexOf(min);
        return neighbors[index] - cell;
    }

    public float GetAccurateDistanceFrom(Transform obj, Vector3 worldPos)
    {
        var cell = _map.WorldToCell(worldPos);
        var nullableCellDistance = GetDistanceFrom(obj, cell);
        if (nullableCellDistance == null) return float.MaxValue;
      
        var cellDistance = (int) nullableCellDistance;
        var size = _map.cellSize.x;

        var neighborCells = GetNeighborsOf(worldPos);
        // neighborCells.ForEach(HighlightCell);
        var neighborDistances = neighborCells.Select(n => GetDistanceFrom(obj, n) ?? cellDistance).ToList();
        var relativeOffset = (worldPos - CellCenterToWorld(neighborCells[0])) / size;
        var relativeDistance = Blerp(neighborDistances[0], neighborDistances[1], neighborDistances[2],
            neighborDistances[3], relativeOffset.x, relativeOffset.y);
        return relativeDistance;
    }

    private void HighlightCell(Vector3Int pos)
    {
        var worldPos = CellCenterToWorld(pos);
        Debug.DrawLine(worldPos - new Vector3(0.2f, 0), worldPos + new Vector3(0.2f, 0), Color.cyan);
        Debug.DrawLine(worldPos - new Vector3(0, 0.2f), worldPos + new Vector3(0, 0.2f), Color.cyan);
    }

    public int GetDistanceFrom(Transform obj, Vector3 worldPos)
    {
        var pos = _map.WorldToCell(worldPos);
        var distance = GetDistanceFrom(obj, pos);

        if (distance != null)
        {
            return (int) distance;
        }
        
        Debug.LogError("No distance found");
        return int.MaxValue;
    }

    private int? GetDistanceFrom(Transform obj, Vector3Int pos)
    {
        if (_nodes.ContainsKey(pos))
        {
            var node = _nodes[pos];
            if (node.distances.ContainsKey(obj))
            {
                return node.distances[obj].distance;
            }

            return null;
        }

        return null;
    }

    private float Blerp(float c00, float c10, float c01, float c11, float tx, float ty)
    {
        return Mathf.Lerp(Mathf.Lerp(c00, c10, tx), Mathf.Lerp(c01, c11, tx), ty);
    }

    private List<Vector3Int> GetNeighborsOf(Vector3 pos)
    {
        Vector3[] corners = {
            new Vector3(-1, -1),
            new Vector3(1, -1),
            new Vector3(-1, 1),
            new Vector3(1, 1)
        };
        return corners.Select(c => _map.WorldToCell(c / 2 + pos)).ToList();
    }

    private List<SpaceNode> GetNeighborsOf(Vector3Int pos)
    {
        var positions = NeighborPositions(pos);
        var neighbors = new List<SpaceNode>();
        foreach (var neighborPos in positions)
        {
            if (_nodes.TryGetValue(neighborPos, out var neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private IEnumerable<Vector3Int> NeighborPositions(Vector3Int pos)
    {
        return _relativeNeighbors.Select(neighbor => neighbor + pos);
    }

    private SpaceNode GetNeighborOfStart(Transform obj)
    {
        var pos = _map.WorldToCell(obj.position);
        var neighborPos = NeighborPositions(pos);
        foreach (var neighbor in neighborPos)
        {
            if (_nodes.ContainsKey(neighbor))
            {
                return _nodes[neighbor];
            }
        }

        return null;
    }

    private void CalculateDistances(SpaceNode start, Transform obj)
    {
        // Breath First Search
        var queue = new Queue<SpaceNode>();
        queue.Enqueue(start);
        start.distances[obj].distance = 0;

        while (queue.Count != 0)
        {
            var node = queue.Dequeue();
            var nodeData = node.distances[obj];

            foreach (var neighbor in node.neighbors)
            {
                var neighborData = neighbor.distances[obj];
                if (!neighborData.visited)
                {
                    neighborData.visited = true;
                    neighborData.distance = nodeData.distance + 1;
                    neighborData.previous = node;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }
}
