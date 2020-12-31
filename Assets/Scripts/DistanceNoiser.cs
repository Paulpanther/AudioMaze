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

    public int numDirections = 4;

    //For mixing purposes, multiply volume with volume constant 
    public float volumeConstant = 1;
    //Audio Clip for Wall Noise
    public AudioClip wallSound;
    //Audio Source for wall Noise
    private AudioSource[] wallNoise;
    //distance where sound begins to play
    public float triggerDistance;

    //Direction Vectors
    private static Vector3 up = new Vector3(0, 1, 0);
    private static Vector3 down = new Vector3(0, -1, 0);
    private static Vector3 left = new Vector3(1, 0, 0);
    private static Vector3 right = new Vector3(-1, 0, 0);
    private Vector3[] directions = new Vector3[]{up,down,left,right };

	private void Start()
	{
		wallNoise = new AudioSource[numDirections];
        for(int i = 0; i < numDirections; i++) {
            float associatedPitch = 1.4f - 0.2f * (float)i;
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = wallSound;
            audioSource.pitch = associatedPitch;
            wallNoise[i] = audioSource;
        }
	}

    void playSound(int direction, float distance)
    {
        AudioSource noiseOfDirection = wallNoise[direction];
        if (distance <= triggerDistance)
        {            
            noiseOfDirection.volume = 1 - distance*volumeConstant;
            if(!noiseOfDirection.isPlaying) noiseOfDirection.Play();
        } 
        else 
        {
            if(noiseOfDirection.isPlaying) noiseOfDirection.Stop();
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
        for(int i = 0; i < numDirections; i++)
        {
            //Debug.Log(i + " casting ray in direction" + directions[i] + " from "+ gameObject.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], 100, 1 << LayerMask.NameToLayer("Walls"));
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y,0), directions[i].normalized * triggerDistance, hit.distance <= triggerDistance ? Color.red : Color.green);
            playSound(i, hit.distance);
        
        }

    }
}
