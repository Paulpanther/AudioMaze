using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSoundDistanceNoiser : MonoBehaviour
{

    public MazeSolver maze;
    public Transform goal;
    
    private void Update()
    {
        var distance = maze.GetAccurateDistanceFrom(goal, transform.position);
        Debug.Log(distance);
    }
}
