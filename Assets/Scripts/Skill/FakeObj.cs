using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeObj : MonoBehaviour
{
    public GameObject DestroyVFX;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tagger"))
        {
            Instantiate(DestroyVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Tagger"))
        {
            Instantiate(DestroyVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }   
    }
}
