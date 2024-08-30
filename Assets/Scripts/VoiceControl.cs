using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceControl : MonoBehaviour
{
    public static VoiceControl Instance;

    public AudioClip[] VoiceClip;

    private AudioSource voice;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        voice = GetComponent<AudioSource>();
    }

    public void PlayVoice( int n )
    {
        voice.PlayOneShot( VoiceClip[n] );
    }
}
