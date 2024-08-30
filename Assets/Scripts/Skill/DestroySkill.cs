using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySkill : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.MainModule MainM;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        MainM = ps.main;
        MainM.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleCollision(GameObject other)
    { 
        Debug.Log("Hit");
        Debug.Log(other.gameObject);
        if (other.gameObject.CompareTag("Destroyable")) other.gameObject.GetComponent<BreakingObj>().BeenRoar();
    }
}
