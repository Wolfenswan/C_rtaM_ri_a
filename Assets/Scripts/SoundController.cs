using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    private int clipNumber = 0; 
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void NextClip()
    {
        //clipNumber++;
    }

}
