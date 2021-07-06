using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    private MusicControllerFMOD mus;

    [NonSerialized] public int CheckPointNumber;
    [NonSerialized] public CallUpdate prevCheckpoint;
    [NonSerialized] public Level level;

    private Transform goal => level.win.transform;
    private MazeSolver maze => level.mazeSolver;

    private Player player => level.player;
    //public int goodCutOff;

    private bool _firstPass = false;
    
    private void Start()
    {
        mus = GetComponentInParent<MusicControllerFMOD>();
    }

    public bool HasPassedCheckpoint()
    {
        var isPassed = DistanceToGoal() >= maze.GetDistanceFrom(goal, player.transform.position);
        if (isPassed && !_firstPass)
        {
            _firstPass = true;
            //mus.CheckpointReached(CheckPointNumber);
        }
        return isPassed;
    }

    public float DistanceToGoal()
    {
        return maze.GetDistanceFrom(goal, transform.position);
    }

    public float DistanceToPlayer()
    {
        return maze.GetDistanceFrom(transform, player.transform.position);
    }

    public float? DistanceToPrevCheckpoint()
    {
        return prevCheckpoint == null ? (float?) null : maze.GetDistanceFrom(transform, prevCheckpoint.transform.position);
    }

    public float? InterpolateBetweenPrevAndPlayer()
    {
        var distanceToPrev = DistanceToPrevCheckpoint();
        // Debug.Log(distanceToPrev);
        if (distanceToPrev == null) return null;
        return Mathf.Clamp01((float) (1 - DistanceToPlayer() / distanceToPrev));
    }

    public float GetVolume() => HasPassedCheckpoint() ? 1f : InterpolateBetweenPrevAndPlayer() ?? 0;
}
