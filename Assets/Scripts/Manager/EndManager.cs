using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndManager : MonoBehaviour
{
    public static EndManager Instance;

    public bool End = false;

    private bool avoidTrigger = false;

    private bool WhereDM = false; // true for Demon / false for Mushroom

    private bool EorS = false; // true for escape / false for survive

    public GameObject Demon; 

    public GameObject Shroom;

    public GameObject Environment;

    public Transform Dposition; 

    public Transform Mposition; 

    public GameObject rig; 

    private Transform CameraOffset;

    private Transform headRig;

    private Transform leftHand;

    private Transform rightHand;

    public GameObject BGM1;

    public GameObject BGM2;

    void Start()
    {
        Instance = this;

        CameraOffset = rig.transform.Find("Camera Offset");
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHand = rig.transform.Find("Left");
        rightHand = rig.transform.Find("Right");
    }

    // Update is called once per frame
    void Update()
    {
        if(End && !avoidTrigger)
        {
            if(OVRInput.GetUp(OVRInput.Button.Three))
            {
                if(EorS)
                {
                    EscapeGameManager.Instance.LeaveRoom();
                }
                else
                {
                    GameManager.Instance.LeaveRoom();
                }
                avoidTrigger = true;
            }

            if(WhereDM)
            {
                MapPosition(rig.transform, Dposition );
                MapPosition(CameraOffset, Dposition );
                MapPosition(headRig, Dposition );
            }
            else
            {
                MapPosition(rig.transform, Mposition );
                MapPosition(CameraOffset, Mposition );
                MapPosition(headRig, Mposition );
            }
        }
    }

    public void ExecuteEndScene(bool DorM, bool mode) // true for Demon win / false for Mushroom win
    {
        CameraOffset.gameObject.GetComponent<Rigidbody>().useGravity = false;
        CameraOffset.gameObject.GetComponent<GorillaLocomotion.Player>().enabled = false;
        headRig.gameObject.GetComponent<Spell>().enabled = false;
        leftHand.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);

        End = true;
        EorS = mode;
        WhereDM = DorM;
        BGM1.SetActive(false);
        BGM2.SetActive(true);
        if(DorM)
        {
            Demon.SetActive(true);
            MapPosition(rig.transform, Dposition );
            MapPosition(CameraOffset, Dposition );
            MapPosition(headRig, Dposition );
        }
        else
        {
            Shroom.SetActive(true);
            MapPosition(rig.transform, Mposition );
            MapPosition(CameraOffset, Mposition );
            MapPosition(headRig, Mposition );
        }
        Environment.SetActive(false);
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
