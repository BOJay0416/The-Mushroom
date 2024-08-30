using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAction : MonoBehaviour
{
    public GameObject HostObj;

    ParticleSystem ps;
    ParticleSystem.MainModule MainM;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        MainM = ps.main;
        MainM.stopAction = ParticleSystemStopAction.Callback;
    }

    void Update()
    {
        
    }

    void OnParticleSystemStopped()
    {
        Debug.Log("粒子停止");
        HostObj.gameObject.SetActive(false);
        // Destroy(gameObject);
    }

    // void OnParticleCollision(GameObject other)
    // { 
    //     Debug.Log(other.gameObject);
    // }
}
