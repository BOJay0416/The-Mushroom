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

public class EscapeMushroomPlayer : MonoBehaviourPunCallbacks, IPunObservable
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

    public float SpawnTime = 3f;

    private Vector3 Offset = new (0f,0.2f,0f);

    public GameObject SmokeVFX;

    private bool BeingShock = false;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // XRRig rig = FindObjectOfType<XRRig>();
        XROrigin rig = FindObjectOfType<XROrigin>();

        headRig = rig.transform.Find("Camera Offset/Main Camera");

        this.tag = "Taggee";
    }

    void Update()
    {
        if (this.transform.parent.gameObject.GetComponent<EscapePlayerControl>().photonView.IsMine)
        {
            mashroomBody.gameObject.SetActive(false);
            MapPosition(mashroom, headRig);
        }

        if(BeingShock)
        {
            HandleShock();
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position - Offset;
        target.rotation = rigTransform.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!this.transform.parent.gameObject.GetComponent<EscapePlayerControl>().photonView.IsMine)
        {
            return;
        }

        if (other.gameObject.CompareTag("Tagger"))
        {
            Debug.Log("被抓到了QQ " + mashroomBody.position);
            MushroomDie();
        }   

        if (other.gameObject.CompareTag("Safe"))
        {
            Debug.Log("脫逃啦");
            EscapeGameManager.Instance.avoidTrigger = true;
            this.transform.parent.gameObject.GetComponent<EscapePlayerControl>().IsEscape = true;
        }   
    }

    void OnCollisionEnter(Collision other)
    {
        if (!this.transform.parent.gameObject.GetComponent<EscapePlayerControl>().photonView.IsMine)
        {
            return;
        }

        if (other.gameObject.CompareTag("Tagger"))
        {
            Debug.Log("被抓到了QQ " + mashroomBody.position);
            MushroomDie();
        }   

        if (other.gameObject.CompareTag("Safe"))
        {
            Debug.Log("脫逃啦");
            EscapeGameManager.Instance.avoidTrigger = true;
            this.transform.parent.gameObject.GetComponent<EscapePlayerControl>().IsEscape = true;
        }   
    }

    void HandleShock()
    {
        this.transform.parent.gameObject.GetComponent<SinglePlayerControl>().IsShock = true;
    }

    public void MushroomDie()
    {
        this.tag = "Untagged";

        VoiceControl.Instance.PlayVoice(8);

        Instantiate(SmokeVFX, transform.position, transform.rotation);

        this.transform.parent.gameObject.GetComponent<EscapePlayerControl>().IsDie = true;
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
