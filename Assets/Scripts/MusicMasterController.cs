using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicMasterController : MonoBehaviour
{
    public AudioMixer mixer;
    public Player pl;
    private int distanceMul;
    private bool wrongDirection = false;
    private string[] parameters = new string[] { "MusicLP", "MusicHP", "MusicDist","MusicThrsh" };
    private float[] wrongGoals = new float[] { 8000.00f, 500.0f,0.93f,-32.00f};
    private float[] rightGoals = new float[] { 20000.00f, 10.0f,0.0f,0.0f};
    //Initialise to rightGoals values
    private float[] currentVals = new float[] { 20000.00f, 10.0f,0.0f,0.0f};
    //Initialise to rightGoals values
    private float[] currentGoals = new float[] { 20000.00f, 10.0f,0.0f,0.0f};
    private float[] valueChangeRates = new float[] { 20000.0f, 1000.0f,1.0f,100f};
    private float[] sensitivity = new float[] { 100.0f, 10.0f,0.1f,1f};
    //Initialise to all true
    private bool[] goalsReached = new bool[] { true, true,true,true};
    private int size = 4;
    private bool wrongDirectionChanged = false;
    private bool rightDirectionChanged = false;
    



    public void directionChange()
    {
        if (wrongDirection)
        {
            wrongDirection = false;
            for (int i = 0; i < size; i++)
            {
                currentGoals[i] = rightGoals[i];
                goalsReached[i] = false;
            }
        }
        else
        {
            wrongDirection = true;
            for (int i = 0; i < size; i++)
            {
                currentGoals[i] = wrongGoals[i];
                goalsReached[i] = false;
            }

        }
    }

    private void Update()
    {
        if(pl.distanceChange<0 && !wrongDirectionChanged)
        {
            wrongDirectionChanged = true;
            rightDirectionChanged = false;
            directionChange();
        }else if (pl.distanceChange>0 && !rightDirectionChanged)
        {
            rightDirectionChanged = true;
            wrongDirectionChanged = false;
            directionChange();
        }

        for (int i = 0; i < size; i++)
        {
            if(!goalsReached[i] && (currentVals[i]>=currentGoals[i]-sensitivity[i]) && (currentVals[i] <= currentGoals[i] + sensitivity[i]))
            {
                // Debug.Log(parameters[i] + " reached goal");
                goalsReached[i] = true;
                currentVals[i] = currentGoals[i];
                mixer.SetFloat(parameters[i], currentVals[i]);
            }
            else if (!goalsReached[i]) {
                mixer.SetFloat(parameters[i], currentVals[i]);
                if (currentVals[i] < currentGoals[i])
                {
                    currentVals[i] += Time.deltaTime * valueChangeRates[i];
                }else if (currentVals[i] > currentGoals[i])
                {
                    currentVals[i] -= Time.deltaTime * valueChangeRates[i];
                }

            }

        }
        
    }
}
