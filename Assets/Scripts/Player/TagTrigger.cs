using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagTrigger : MonoBehaviour
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
            GameManager.Instance.spawnedPlayerPrefab.GetComponent<SinglePlayerControl>().PlayerInstance.GetComponent<MashroomPlayer>().ChangeTagger();
            // MushroomDie();
        }   
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Tagger"))
        {
            GameManager.Instance.spawnedPlayerPrefab.GetComponent<SinglePlayerControl>().PlayerInstance.GetComponent<MashroomPlayer>().ChangeTagger();
        }
    }
}
