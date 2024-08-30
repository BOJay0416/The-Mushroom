using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class OpVideoControl : MonoBehaviour
{
    public GameObject videoPlayer;

    public GameObject BGM;

    private AudioSource bgMusic;

    private VideoPlayer VPlayer;

    void Start()
    {
        VPlayer = GetComponent<VideoPlayer>();

        VPlayer.loopPointReached += OnVideoComplete;

        bgMusic = BGM.GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    void OnVideoComplete(VideoPlayer vp)
    {
        Debug.Log("Finish");

        gameObject.SetActive(false);
        bgMusic.Play();
    }
}
