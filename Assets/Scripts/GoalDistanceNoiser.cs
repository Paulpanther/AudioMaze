using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDistanceNoiser : MonoBehaviour
{

    public MazeSolver maze;
    public Transform goal;
    
    private void Update()
    {
        var distance = maze.GetDistanceFrom(goal, transform.position);
        Debug.Log(distance);
    }
}
