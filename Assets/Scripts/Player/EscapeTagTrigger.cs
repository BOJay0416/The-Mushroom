using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeTagTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tagger"))
        {
            EscapeGameManager.Instance.spawnedPlayerPrefab.GetComponent<EscapePlayerControl>().PlayerInstance.GetComponent<EscapeMushroomPlayer>().MushroomDie();
        }   

        if (other.gameObject.CompareTag("Safe"))
        {
           EscapeGameManager.Instance.spawnedPlayerPrefab.GetComponent<EscapePlayerControl>().IsEscape = true;
        }   
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Tagger"))
        {
            EscapeGameManager.Instance.spawnedPlayerPrefab.GetComponent<EscapePlayerControl>().PlayerInstance.GetComponent<EscapeMushroomPlayer>().MushroomDie();
        }   

        if (other.gameObject.CompareTag("Safe"))
        {
           EscapeGameManager.Instance.spawnedPlayerPrefab.GetComponent<EscapePlayerControl>().IsEscape = true;
        }   
    }
}
