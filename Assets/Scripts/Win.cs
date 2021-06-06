using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{

    private readonly List<Action> callbacks = new List<Action>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        callbacks.ForEach(it => it.Invoke());
    }

    public void RegisterCallback(Action callback)
    {
        callbacks.Add(callback);
    }
}

