using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShepardsScale : Synthesizer
{

    public class ShepardFrequency : Frequency 
    {

        private ShepardsScale scale;

        public float shiftPhase = 0;

        private float sampleRate;

        public ShepardFrequency(ShepardsScale scale, float frequency, float amplitude, float initialShiftPhase) : base(frequency, amplitude)
        {
            this.scale = scale;
            this.shiftPhase = initialShiftPhase;
        }
        

        public override void CalculateIncrement(float sampleRate)
        {
            this.sampleRate = sampleRate;
            base.CalculateIncrement(sampleRate);
        }

        public override float NextSample()
        {
            shiftPhase = (shiftPhase + this.scale.shift / sampleRate + 1f) % 1f;
            frequency = scale.frequencyPerRelativeFrequency(shiftPhase);
            amplitude = scale.amplitudePerRelativeFrequency(shiftPhase);
            CalculateIncrement(this.sampleRate);
            
            this.phase += increment;
            float nextSample = Mathf.Sin(this.phase) * this.amplitude;
            this.phase %= 2f * Mathf.PI;
            return nextSample;
        }


    }

    public float baseFrequency = 50;
    public float totaloctaves = 3;
    public int numTones = 1;
    public float shift = 0.2f;


    // Start is called before the first frame update
    new void Start()
    {
        baseAmplitude = 1f / numTones;
        frequencies = new Frequency[numTones];
        for(int i = 0; i < numTones; i++) {
            frequencies[i] = new ShepardFrequency(this,0,0, ((float)i) / ((float)numTones));
        }

        base.Start();   
    }

    // Update is called once per frame
    void Update()
    {
        // phase = (phase + increment + 1f) % 1f;
        // Debug.Log(phase);
        // //float phase = transform.position.x / 10f;
        // for(int i = 0; i < numTones; i++) {
        //     float relativeFrequency = (((float)i) / ((float)numTones) + phase) % 1f;
        //     frequencies[i].frequency = frequencyPerRelativeFrequency(relativeFrequency);
        //     frequencies[i].amplitude = amplitudePerRelativeFrequency(relativeFrequency);
        // }
    }


    private float frequencyPerRelativeFrequency(float relativeFrequency) {
        return baseFrequency * Mathf.Pow(2f, relativeFrequency * totaloctaves);
    }

    private float amplitudePerRelativeFrequency(float relativeFrequency)
    {
        //Returns 0 for 0 and 1, and sine in between with max at 0.5
        return Mathf.Sin(relativeFrequency * Mathf.PI); 
    }
}
