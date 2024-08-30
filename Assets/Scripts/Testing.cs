using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GorillaLocomotion;
using UnityEngine.XR.Interaction.Toolkit;

public class Testing : MonoBehaviour
{
    float currTime = 0f;

    public GameObject left;
    public GameObject right;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("q"))
        {
            VoiceControl.Instance.PlayVoice(2);
        }

        if (Input.GetKey("w"))
        {
            Voice3DControl.Instance.PlayVoice(2);
        }

        if (Input.GetKey("e"))
        {
            VoiceControl.Instance.PlayVoice(3);
        }

        if (Input.GetKey("r"))
        {
            Voice3DControl.Instance.PlayVoice(3);
        }

        // currTime += Time.deltaTime;
        // if(currTime > 10 && currTime <= 20)
        // {
        //     left.GetComponent<XRController>().enableInputTracking = false;
        //     right.GetComponent<XRController>().enableInputTracking = false;
        // }
        // else
        // {
        //     left.GetComponent<XRController>().enableInputTracking = true;
        //     right.GetComponent<XRController>().enableInputTracking = true;
        // }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject);
    }
}
