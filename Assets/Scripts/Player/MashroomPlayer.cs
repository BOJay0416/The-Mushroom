using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun;

using Photon.Realtime;

using UnityEngine.XR;

using UnityEngine.Networking;

using UnityEngine.SceneManagement;

public class MashroomPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("香菇本體")]
    public Transform mashroom;

    [Tooltip("香菇本體")]
    public Transform mashroomBody;

    private new PhotonView photonView;

    
    //private Transform Body;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    public AudioClip CatchSound;

    private AudioSource myAudioSource;

    private Vector3 Offset = new (0f,0.2f,0f);

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XRRig rig = FindObjectOfType<XRRig>();

        headRig = rig.transform.Find("Camera Offset/Main Camera");

        myAudioSource = GetComponent<AudioSource>();

        this.tag = "Taggee";
    }

    void Update()
    {
        if (this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().photonView.IsMine)
        {
            mashroomBody.gameObject.SetActive(false);
            MapPosition(mashroom, headRig);
        }
        //mashroom.gameObject.SetActive(false);
        //MapPosition(mashroom, headRig);
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position - Offset;
        target.rotation = rigTransform.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().photonView.IsMine)
        {
            return;
        }

        Debug.Log("Trigger! with " + other.gameObject.name);

        if (other.gameObject.CompareTag("Tagger"))
        {
            Debug.Log("被抓到了QQ");
            ChangeTagger();
        }   
    }

    void OnCollisionEnter(Collision other)
    {
        if (!this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().photonView.IsMine)
        {
            return;
        }

        Debug.Log("Collision! with " + other.gameObject.name);

        if (other.gameObject.CompareTag("Tagger"))
        {
            Debug.Log("被抓到了QQ");
            ChangeTagger();
        }   
    }

    void ChangeTagger()
    {
        //PhotonNetwork.Instantiate("msVFX_Stylized Smoke 4", mashroomBody.position, mashroomBody.rotation);

        myAudioSource.PlayOneShot(CatchSound);

        this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().IsToDemon = true;
    }

    void ChangeTaggee()
    {
        this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().IsToMushroom = true;
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
