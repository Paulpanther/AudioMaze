using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

public class Draught : MonoBehaviour
{
    public AudioMixerGroup audioOut;
    public AudioClip sound;

    public Player player;
    public Tilemap map;

    public bool useTilesForPosition;

    private MazeSolver maze;

    private static Vector3Int[] cardinalDirections = {
        Vector3Int.right,
        Vector3Int.up,
        Vector3Int.left,
        Vector3Int.down
    };

    private GameObject[] wrappers = new GameObject[cardinalDirections.Length];

    private Vector3 halfCell = new Vector3(0.5f, 0.5f, 0);

    // Start is called before the first frame update
    void Start()
    {
        maze = player.maze;
        for(int i = 0; i < wrappers.Length; i++) {
            wrappers[i] = new GameObject("Direction"+cardinalDirections[i]);
            wrappers[i].transform.parent = transform;
            wrappers[i].transform.localPosition = cardinalDirections[i];

            AudioSource audioSource = wrappers[i].AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = audioOut;
            audioSource.clip = sound;
            audioSource.spatialBlend = 1;
            audioSource.pitch = 2;
            audioSource.loop = true;
            audioSource.spatialize = true;
            audioSource.time = ((float)i) * 10f;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = useTilesForPosition ? map.WorldToCell(player.transform.position) + halfCell : player.transform.position;
        float total = 0;

        for(int i = 0; i < wrappers.Length; i++) 
        {
            var pos = wrappers[i].transform.position;
            bool isBlocked = map.HasTile(map.WorldToCell(pos));
            var furtherPos = pos + cardinalDirections[i];
            bool isFurtherBlocked = map.HasTile(map.WorldToCell(furtherPos));

            wrappers[i].GetComponent<AudioSource>().volume = (isBlocked ? 0 : 1) * (isFurtherBlocked ? 0.5f : 1);
            total += wrappers[i].GetComponent<AudioSource>().volume;

            //DrawPosition(pos, isBlocked ? Color.red : Color.white);
            //DrawPosition(furtherPos, isFurtherBlocked ? Color.red : Color.white);
        }


        // Be more quiet in open spaces
        if(total > 3) 
        {
            for(int i = 0; i < wrappers.Length; i++) 
            {
                wrappers[i].GetComponent<AudioSource>().volume /= (total - 2);
            }
        }
    }

    void DrawPosition(Vector3 pos, Color color)
    {
        Debug.DrawLine(pos + new Vector3(0.1f, 0.1f), pos - new Vector3(0.1f, 0.1f), color);
        Debug.DrawLine(pos + new Vector3(0.1f, -0.1f), pos - new Vector3(0.1f, -0.1f), color);
    }
}
