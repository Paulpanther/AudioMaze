using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShepardsDistanceNoiser : ShepardsScale
{

    public float conversionFactor = 0.02f;

    void Update()
    {
        Player player = GetComponentInParent<Player>();
        this.shift = player.distanceChange * conversionFactor;
    }
}
