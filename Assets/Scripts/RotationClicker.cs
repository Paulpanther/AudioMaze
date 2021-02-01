using UnityEngine;

public class RotationClicker : MonoBehaviour
{
    public int majorTickCount = 4;
    public int minorTickCount = 12;

    private float prevTime = 0; 
    void FixedUpdate()
    {
        // 0 = 360 is north, 90 is west, 180 is south, 270 is east
        var degree = transform.rotation.eulerAngles.z;
        if (Time.fixedTime - prevTime > 0.5)
        {
            Debug.Log(GetNearestTick(degree, majorTickCount));
            Debug.Log(GetNearestTick(degree, minorTickCount));
            prevTime = Time.fixedTime;
        }
    }

    private float GetNearestTick(float degree, int tickCount)
    {
        return degree / (360f / tickCount);
    }
}
