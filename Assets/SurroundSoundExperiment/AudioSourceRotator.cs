using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceRotator : MonoBehaviour
{
    public float speed;

    void Start()
    {
    }

    void Update()
    {
        transform.RotateAround(
            transform.position,
            transform.up,
            Time.deltaTime * speed
        );
    }
}