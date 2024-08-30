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

public class EscapeIKDemon : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform Demon;

    private new PhotonView photonView;

    private bool BeingBlind = false;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        XROrigin rig = FindObjectOfType<XROrigin>();

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
        if(BeingBlind)
        {
            HandleBlind();
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }

    void HandleBlind()
    {
        this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().IsBlind = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Taggee"))
        {
            Debug.Log("Catch!");
        }   
    }

    void OnCollisionEnter(Collision other)
    {
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
