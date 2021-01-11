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

    private readonly Vector3Int[] _relativeNeighbors = {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0),
    };
    
    private void Start()
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

    public int GetDistanceFrom(Transform obj, Vector3 worldPos)
    {
        var pos = _map.WorldToCell(worldPos);
        if (_nodes.ContainsKey(pos))
        {
            var node = _nodes[pos];
            if (node.distances.ContainsKey(obj))
            {
                return node.distances[obj].distance;
            }
            else
            {
                Debug.LogError("No distance found");
                return int.MaxValue;
            }
        }
        
        Debug.LogError("No distance found");
        return int.MaxValue;
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
