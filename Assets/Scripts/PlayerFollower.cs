using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public GameObject player;

    /**
     * Distance to the last location
     * for which a new point is saved 
     */
    public float locationDistance = 0.25f;

    [Min(0)]
    public int capacity = 16;

    private Vector2 _lastLocation;
    private Queue<Vector2> _locationHistory;
    
    void Start()
    {
        _locationHistory = new Queue<Vector2>(capacity);
        _SaveNewLocation();
    }

    private void _SaveNewLocation()
    {
        _lastLocation = player.transform.position;
        _locationHistory.Enqueue(_lastLocation);
        if (_locationHistory.Count > capacity)
        {
            _locationHistory.Dequeue();
        }
    }

    void Update()
    {
        if ((_lastLocation - (Vector2) player.transform.position).magnitude > locationDistance)
        {
            _SaveNewLocation();
        }

        transform.position = _locationHistory.Peek();
    }
}
