using UnityEngine;

public class RotationClicker : MonoBehaviour
{

    [Range(0, 1)] public float veryStrongClickVolume;
    [Range(-3, 3)] public float veryStrongClickPitch;
    [Range(0, 1)] public float strongClickVolume;
    [Range(-3, 3)] public float strongClickPitch;
    [Range(0, 1)] public float normalClickVolume;
    [Range(-3, 3)] public float normalClickPitch;

    private AudioSource _audioSource;

    void Start()
    {
        
        _audioSource = GetComponent<AudioSource>();
    }

    public void RotationChanged(int newRotation)
    {
        Debug.Log(newRotation);
        if(newRotation % 360 == 0) {
            _audioSource.pitch = veryStrongClickPitch;
            _audioSource.volume = veryStrongClickVolume;
        } else if(newRotation % 90 == 0) {
            _audioSource.pitch = strongClickPitch;
            _audioSource.volume = strongClickVolume;
        } else {
            _audioSource.pitch = normalClickPitch;
            _audioSource.volume = normalClickVolume;
        }
        _audioSource.Play();
    }

}
