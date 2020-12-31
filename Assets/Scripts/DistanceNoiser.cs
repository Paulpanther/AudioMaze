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

    //Resolution of Ray casting
    public int numRays = 12;

    //For mixing purposes, multiply volume with volume constant 
    public float volumeConstant = 1;
    //distance where sound begins to play
    public float triggerDistance = 10;
    //Way how wall distance is mapped to sound volume
    public DistanceMapping distanceMapping = DistanceMapping.INVERSE_QUADRATIC;

   //Audio Clip for Wall Noise
    public AudioClip wallSound;
    //Audio Sources for directions of wall noise
    private AudioSource[] wallNoise;

    private float playerRadius = 0.2f; //TODO make dynamic

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

    void playSound(int rayIndex, float distance)
    {
        AudioSource noiseOfDirection = wallNoise[rayIndex];
        if (distance <= triggerDistance)
        {
            noiseOfDirection.volume = mapDistance(distance);
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
		Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        for(int i = 0; i < numRays; i++)
        {
            Vector2 direction = Direction(i);
            Vector2 rayStart = pos + direction*playerRadius;
            RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, 100, 1 << LayerMask.NameToLayer("Walls"));
            Debug.DrawRay(rayStart, direction * triggerDistance, Color.red);
            playSound(i, hit.distance);
            
            if (hit.collider && hit.distance <= triggerDistance)
			{
				Debug.DrawRay(rayStart, hit.point - rayStart, Color.green);
			}
        
        }

    }

    Vector2 Direction(int rayIndex) 
    {
		float currentAngle = -transform.rotation.eulerAngles.z;
		Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        float angle = currentAngle + 360f / (float) numRays * ((float) rayIndex + 0.5f);
        float radians = angle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
        return direction;
    }

    public enum DistanceMapping 
    {
        LINEAR_DECREASE, INVERSE_LINEAR, INVERSE_QUADRATIC
    }

    private float mapDistance(float distance) 
    {
        switch(distanceMapping) 
        {
            case DistanceMapping.LINEAR_DECREASE : 
                return 1f - (distance/triggerDistance) * volumeConstant;
            
            case DistanceMapping.INVERSE_LINEAR : 
                return 1f / (distance + 1f);
            
            case DistanceMapping.INVERSE_QUADRATIC : default :
                return 1f / (distance * distance + 1f);
        }
    }
}
