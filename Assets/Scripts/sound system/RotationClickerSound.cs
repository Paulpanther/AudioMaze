using UnityEngine;

public class RotationClickerSound : MonoBehaviour
{
    public string rotationClickSoundName = "Rotations";

    private AudioOut.Instance soundInstance;
    void Start()
    {
        soundInstance = AudioOut.CreateInstance(rotationClickSoundName);
    }

    public void RotationChanged(int newRotation)
    {
        if(newRotation % 360 == 0) {
            soundInstance.fmodInstance.setParameterByName("rotationVal",1);
        } else if(newRotation % 90 == 0) {
            soundInstance.fmodInstance.setParameterByName("rotationVal", 0.5f);
        } else {
            soundInstance.fmodInstance.setParameterByName("rotationVal", 0);
        }
        soundInstance.Start();
    }
}
