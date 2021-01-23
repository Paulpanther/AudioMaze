using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSoundDistanceNoiser : MonoBehaviour
{

    public MazeSolver maze;
    public Transform goal;

    private float? _lastDistance;
    
    private void Update()
    {
        var distance = maze.GetAccurateDistanceFrom(goal, transform.position);
        var delta = (distance - _lastDistance) ?? 0;
        Debug.Log(delta);
        // if (delta > 0.01)
        // {
        //     Debug.Log("Yay");
        // }
        // else
        // {
        //     Debug.Log("Nay");
        // }
        _lastDistance = distance;
    }
}
