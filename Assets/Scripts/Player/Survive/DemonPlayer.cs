using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun;

using Photon.Realtime;

using UnityEngine.XR;

using UnityEngine.Networking;

using UnityEngine.SceneManagement;

using Unity.XR.CoreUtils;

public class DemonPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform Demon;

    public Transform DemonBody;

    public Transform lefthand;

    public Transform righthand;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    /*----------------------------------*/

    private new PhotonView photonView;

    /*----------------------------------*/

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

        if (this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().photonView.IsMine)
        {
            rig.gameObject.tag = "Tagger";

            rig.transform.GetChild(0).gameObject.tag = "Tagger";

            rig.transform.GetChild(1).gameObject.tag = "Tagger";
        }

        this.tag = "Tagger";
    }

    void Update()

    {
        if (this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().photonView.IsMine)
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

    void HandleBlind()
    {
        this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().IsBlind = true;
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

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         // 為玩家本人的狀態, 將 IsFiring 的狀態更新給其他玩家
    //         stream.SendNext(TaggerOrTaggee);
    //     }
    //     else
    //     {
    //         // 非為玩家本人的狀態, 單純接收更新的資料
    //         this.TaggerOrTaggee = (bool)stream.ReceiveNext();
    //     }
    // }
}
