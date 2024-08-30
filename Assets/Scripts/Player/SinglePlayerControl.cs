using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun;

using Photon.Realtime;

using UnityEngine.XR;

using UnityEngine.Networking;

using UnityEngine.SceneManagement;

using Photon.Pun.UtilityScripts;

public class SinglePlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{

    // public GameObject[] MushroomPrefabs;

    // public GameObject[] DemonPrefabs;

    private GameObject PlayerInstance;

    public bool IsToDemon = false;

    public bool IsChange = false;

    public bool IsToMushroom = false;

    //public static SinglePlayerControl Instance;

    private Transform CameraOffset;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => this.photonView.Owner.GetPlayerNumber() >= 0);

        //Instance = this;

        int PlayerNum = this.photonView.Owner.GetPlayerNumber();

        if (this.photonView.IsMine) 
        {
            Debug.Log("I am " + PlayerNum);

            XRRig rig = FindObjectOfType<XRRig>();

            CameraOffset = rig.transform.Find("Camera Offset");
            headRig = rig.transform.Find("Camera Offset/Main Camera");
            leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
            rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");

            //Debug.Log("From " + rig.gameObject.transform.position + " to " + BornPlane.Instance.Positions[PlayerNum].position);

            MapPosition(rig.gameObject.transform, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(CameraOffset, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(headRig, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(leftHandRig, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(rightHandRig, BornPlane.Instance.Positions[PlayerNum]);

            //Debug.Log("Now is " + rig.gameObject.transform.position + " " + CameraOffset + " " + headRig);
        }

        this.BornMushroom(PlayerNum);
    }

    void Update()
    {
        if(IsToDemon && !IsChange)
        {
            ToDemon();
            if (!this.photonView.IsMine)  
            {
                Debug.Log("本體變成鬼，我也變成鬼了");
                IsToDemon = false;
            }
            else Debug.Log("我變成鬼了");
        }
    }

    private void BornMushroom(int gridStartIndex)
    {
        Debug.Log("Born a Mushroom " + gridStartIndex);
        //this.PlayerInstance = PhotonNetwork.Instantiate("Mushroom0"+(gridStartIndex+1), this.transform.position, this.transform.rotation);

        //this.child.transform.GetChild(0).gameObject.SetActive(true);
        this.PlayerInstance = this.transform.GetChild(gridStartIndex).gameObject;
        // if (this.photonView.IsMine) this.PlayerInstance.SetActive(false);
        // else this.PlayerInstance.SetActive(true);
        this.PlayerInstance.SetActive(true);
        this.PlayerInstance.transform.SetParent(this.transform);

        if (this.photonView.IsMine) GameManager.Instance.CheckVoiceGroup(true);
        if (this.photonView.IsMine) GameManager.Instance.WhoAreYou(true);
    }

    private void BornDemon(int gridStartIndex)
    {
        Debug.Log("Born a Demon" + gridStartIndex);
        //this.PlayerInstance = PhotonNetwork.Instantiate("Demon0"+(gridStartIndex+1), this.transform.position, this.transform.rotation);

        this.PlayerInstance = this.transform.GetChild((gridStartIndex+6)).gameObject;
        // if (this.photonView.IsMine) this.PlayerInstance.SetActive(false);
        // else this.PlayerInstance.SetActive(true);
        this.PlayerInstance.SetActive(true);
        this.PlayerInstance.transform.SetParent(this.transform);

        if (this.photonView.IsMine) GameManager.Instance.CheckVoiceGroup(false);
        if (this.photonView.IsMine) GameManager.Instance.WhoAreYou(false);
    }

    private void OnDestroy()
    {
        Destroy(this.PlayerInstance);
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }

    public void ToDemon()
    {
        Debug.Log("To Demon");
        //Destroy(this.PlayerInstance);
        this.PlayerInstance.SetActive(false);

        int PlayerNum = this.photonView.Owner.GetPlayerNumber();
        this.BornDemon(PlayerNum);

        IsChange = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 為玩家本人的狀態, 將狀態更新給其他玩家
            if(this.photonView.IsMine)
            {
                if(IsToDemon) Debug.Log("Going to change");
                stream.SendNext(IsToDemon);
                //IsToDemon = false;
            }
        }
        else
        {
            // 非為玩家本人的狀態, 單純接收更新的資料
            this.IsToDemon = (bool)stream.ReceiveNext();
        }
    }
}
