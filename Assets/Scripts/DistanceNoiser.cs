using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*To make this work you have to have the player object be unaffected by raycasts, put the player object 
 * in the 'ignore raycasts' layer and make sure the objects you want to detect are tagged wall
 * 
 * Then: Apply this script to the player object
 * Create an audio source
 * Fine tune the values for triggerDistance and volumeConstant
 * */

public class DistanceNoiser : MonoBehaviour
{
    //For mixing purposes, multiply volume with volume constant 
    public float volumeConstant = 1;
    //Audio Source for wall Noise
    public AudioSource wallNoise;
    //distance where sound begins to play
    public float triggerDistance;

    private float currentPitch = 1.0f;

    //Direction Vectors
    private static Vector3 up = new Vector3(0, 1, 0);
    private static Vector3 down = new Vector3(0, -1, 0);
    private static Vector3 left = new Vector3(1, 0, 0);
    private static Vector3 right = new Vector3(-1, 0, 0);
    private Vector3[] directions = new Vector3[]{up,down,left,right };


    void playSound(int direction, float distance)
    {
        wallNoise.volume = 1 - distance*volumeConstant;
        float associatedPitch = 0;
        switch (direction)
        {
            case 0:
                //Debug.Log("playing from above " + distance);
                associatedPitch = 1.4f;
                break;
            case 1:
                //Debug.Log("playing from below " + distance);
                associatedPitch = 1.2f;
                break;
            case 2:
                //Debug.Log("playing from the right " + distance);
                associatedPitch = 1.0f;
                break;
            case 3:
                //Debug.Log("playing from the left " + distance);
                associatedPitch = 0.8f;
                break;
        }
        if (distance <= triggerDistance && !wallNoise.isPlaying)
        {
            
            wallNoise.Play();
        }
        if(currentPitch != associatedPitch)
        {
            currentPitch = associatedPitch;
            wallNoise.pitch = associatedPitch;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hitup = new RaycastHit();
        RaycastHit hitdown = new RaycastHit();
        RaycastHit hitleft = new RaycastHit();
        RaycastHit hitright = new RaycastHit();

        RaycastHit[] hits = new RaycastHit[] {hitup, hitdown, hitleft, hitright};
        int hitnum = 0;
        for(int i = 0; i < 4; i++)
        {
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y,0),directions[i]* triggerDistance, Color.red);
            //Debug.Log(i + " casting ray in direction" + directions[i] + " from "+ gameObject.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i],triggerDistance, 1 << LayerMask.NameToLayer("Walls"));
            if (hit)
            {
                //Debug.Log(hit.collider.gameObject);

                if (hit.collider.gameObject.tag == "Wall")
                {
                    hitnum++;
                    Debug.Log(i + " hits");
                    playSound(i, hit.distance);
                }
            }
        
        }
        if (hitnum == 0)
        {
            wallNoise.Stop();
        }

    }
}
