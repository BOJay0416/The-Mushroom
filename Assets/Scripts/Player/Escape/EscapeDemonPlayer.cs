using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun;

using Photon.Realtime;

using UnityEngine.XR;

using UnityEngine.Networking;

using Unity.XR.CoreUtils;

using UnityEngine.SceneManagement;

public class EscapeDemonPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform Demon;

    public Transform DemonBody;

    public Transform lefthand;

    public Transform righthand;

    private new PhotonView photonView;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    private Vector3 Offset = new (0f,0.2f,0f);

    private bool BeingBlind = false;

    void Start()

    {
        photonView = GetComponent<PhotonView>();

        // XRRig rig = FindObjectOfType<XRRig>();
        XROrigin rig = FindObjectOfType<XROrigin>();

        headRig = rig.transform.Find("Camera Offset/Main Camera");

        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");

        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");

        if (this.transform.parent.gameObject.GetComponent<EscapePlayerControl>().photonView.IsMine)
        {
            rig.gameObject.tag = "Tagger";

            rig.transform.GetChild(0).gameObject.tag = "Tagger";

            rig.transform.GetChild(1).gameObject.tag = "Tagger";
        }

        this.tag = "Tagger";
    }

    void Update()

    {
        if (this.transform.parent.gameObject.GetComponent<EscapePlayerControl>().photonView.IsMine)
        {

            // righthand.gameObject.SetActive(false);

            // lefthand.gameObject.SetActive(false);

            DemonBody.gameObject.SetActive(false);

            MapPosition(Demon, headRig);

            MapPosition(lefthand, leftHandRig);

            MapPosition(righthand, rightHandRig);

        }

        if(BeingBlind)
        {
            HandleBlind();
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position - Offset;
        target.rotation = rigTransform.rotation;
    }

    void HandleBlind()
    {
        this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().IsBlind = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // if (!photonView.IsMine)
        // {
        //     return;
        // }

        if (other.CompareTag("Taggee"))
        {
            Debug.Log("Catch!");
        }   
    }

    void OnCollisionEnter(Collision other)
    {
        // if (!photonView.IsMine)
        // {
        //     return;
        // }

        if (other.gameObject.CompareTag("Taggee"))
        {
            Debug.Log("Catch!");
        }   
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }
}
