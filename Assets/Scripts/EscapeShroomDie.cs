using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.XR.CoreUtils;

using GorillaLocomotion;

public class EscapeShroomDie : MonoBehaviour
{
    public static EscapeShroomDie Instance;

    public GameObject rig; 

    private Transform CameraOffset;

    private Transform headRig;

    private Transform leftHand;

    private Transform rightHand;
    
    public bool DieControl = false;

    void Start()
    {
        Instance = this;

        CameraOffset = rig.transform.Find("Camera Offset");
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHand = rig.transform.Find("Left");
        rightHand = rig.transform.Find("Right");
    }

    void Update()
    {
        if(DieControl)
        {
            Vector3 updateVector = Vector3.zero;
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
            {
                updateVector += Vector3.up;
            }

            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
            {
                updateVector += Vector3.down;
            }
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
            {
                updateVector += Vector3.left;
            }
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
            {
                updateVector += Vector3.right;
            }

            this.gameObject.transform.Translate(updateVector * Time.deltaTime);
        }
    }

    public void ExecuteDie(int n)
    {
        CameraOffset.gameObject.GetComponent<Rigidbody>().useGravity = false;
        CameraOffset.gameObject.GetComponent<GorillaLocomotion.Player>().enabled = false;
        leftHand.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);
        headRig.gameObject.GetComponent<Spell>().enabled = false;

        DieControl = true;

        MapPosition(rig.transform, transform.GetChild(n) );
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
