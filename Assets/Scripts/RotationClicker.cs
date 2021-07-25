using UnityEngine;

public class RotationClicker : MonoBehaviour
{
    public string Rotation;

    FMOD.Studio.EventInstance rotationEV;
    void Start()
    {
        rotationEV = FMODUnity.RuntimeManager.CreateInstance(Rotation);
    }

    public void RotationChanged(int newRotation)
    {
        // Debug.Log(newRotation);
        if(newRotation % 360 == 0) {
            rotationEV.setParameterByName("rotationVal",1);
        } else if(newRotation % 90 == 0) {
            rotationEV.setParameterByName("rotationVal", 0.5f);
        } else {
            rotationEV.setParameterByName("rotationVal", 0);
        }
        rotationEV.start();
    }

}
