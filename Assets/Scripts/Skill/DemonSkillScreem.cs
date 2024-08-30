using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSkillScreem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Screeming!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroyable"))
        {
            //other.gameObject.SetActive(false);
        }   
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Destroyable"))
        {
            //other.gameObject.SetActive(false);
        }   
    }
}
