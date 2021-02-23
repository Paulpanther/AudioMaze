using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIMixerSlider : MonoBehaviour
{

    public AudioMixer mixer;
    public string parameterName;

    private void Start()
    {
        mixer.GetFloat(parameterName, out var volume);
        GetComponent<Slider>().value = Mathf.Pow(10, volume / 20);
    }

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat(parameterName, Mathf.Log10(sliderValue) * 20);
    }
}
