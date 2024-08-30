using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingObj : MonoBehaviour
{
    public GameObject DestroyVFX;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeenRoar()
    {
        Debug.Log("Be Destroy");
        
        Instantiate(DestroyVFX, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
