using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWarmth : MonoBehaviour
{
    
    private static int numFrequencies = 10;

    [Range(50, 1000)]
    public float frequency = 100;
    [Range(0, 1)]
    public float amplitude = 0.5f;

    public float distort = 0.01f;

    //public int overTones = 1;
    [Range(0, /*numFrequencies*/10)]
    public int usedFrequencies = 3;

    private float phase = 0;

    private float[] phases = new float[numFrequencies];

    void OnAudioFilterRead(float[] data, int channels)
    {
        float sampleRate = 44100;
        // float increment = frequency * 2 * Mathf.PI / sampleRate;

        float[] increments = new float[numFrequencies];
        for(int i = 0; i < usedFrequencies; i++)
        {
            increments[i] = frequency * (1.0f-distort*i) * 2 * Mathf.PI / sampleRate;
        }

        for (int dataPointer = 0; dataPointer < data.Length; dataPointer += channels)
        {
            // phase = phase + increment;
            // data[i] += Mathf.Sin(phase) * amplitude;

            for(int i = 0; i < usedFrequencies; i++)
            {
                phases[i] += increments[i];
                data[dataPointer] += Mathf.Sin(phases[i]) * amplitude * Mathf.Pow(-1, i) / (i+1);
                phases[i] %= 2 * Mathf.PI;
            }

            if (channels == 2) data[dataPointer + 1] = data[dataPointer];

            // phase %= 2 * Mathf.PI;
        }
    }


}