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

    public int numRays = 12;

    //For mixing purposes, multiply volume with volume constant 
    public float volumeConstant = 1;
    //Audio Clip for Wall Noise
    public AudioClip wallSound;
    //Audio Source for wall Noise
    private AudioSource[] wallNoise;
    //distance where sound begins to play
    public float triggerDistance;

	private void Start()
	{
		wallNoise = new AudioSource[numRays];
        for(int i = 0; i < numRays; i++) {
            Vector2 direction = Direction(i);
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = wallSound;
            audioSource.pitch = 1.0f + direction.y * 0.5f;
            audioSource.panStereo = direction.x;
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
        for(int i = 0; i < numRays; i++)
        {
            //Debug.Log(i + " casting ray in direction" + directions[i] + " from "+ gameObject.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Direction(i), 100, 1 << LayerMask.NameToLayer("Walls"));
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y,0), Direction(i).normalized * triggerDistance, hit.distance <= triggerDistance ? Color.red : Color.green);
            playSound(i, hit.distance);
        
        }

    }

    Vector2 Direction(int rayIndex) {
		float currentAngle = -transform.rotation.eulerAngles.z;
		Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        float angle = currentAngle + 360f / (float) numRays * ((float) rayIndex + 0.5f);
        float radians = angle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
        return direction;
    }
}
