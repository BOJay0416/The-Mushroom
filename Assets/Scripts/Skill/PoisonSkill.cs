using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSkill : MonoBehaviour
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
        Debug.Log("Poison");
        Debug.Log(other.gameObject);
        other.gameObject.GetComponent<SkillTrigger>().Blind = true;
    }
}
