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

public class EscapePlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    private GameObject PlayerInstance;

    //public GameObject[] DieCamera;

    public bool IsDie = false;

    public bool IamDie = false;

    public bool IsEscape = false;

    //private bool IsChange = false;

    public bool Tagger = false;

    public int PlayerNum;

    private Transform CameraOffset;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => this.photonView.Owner.GetPlayerNumber() >= 0);

        PlayerNum = this.photonView.Owner.GetPlayerNumber();

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

            //Debug.Log("Now is " + rig.gameObject.transform.position + " " + CameraOffset.position + " " + headRig.position);
        }

        if(PlayerNum %2 ==0)
        {
            this.BornMushroom(PlayerNum);
        }
        else
        {
            this.BornDemon(PlayerNum);
        }
    }

    void Update()
    {
        if(IsDie && !IamDie)
        {
            Dieing();
        }

        if(IsEscape)
        {
            EscapeGameManager.Instance.GameOver();
        }
    }

    private void Dieing()
    {
        this.PlayerInstance.SetActive(false);
        //CameraOffset.gameObject.GetComponent<Player>().disableMovement = true;

        if (this.photonView.IsMine) 
        {
            headRig.gameObject.SetActive(false);
            DieCamera.Instance.Camera[PlayerNum/2].gameObject.SetActive(true);
        }

        IamDie = true;
    }

    private void BornMushroom(int gridStartIndex)
    {
        Debug.Log("Born a Mushroom " + gridStartIndex);

        this.PlayerInstance = this.transform.GetChild(gridStartIndex).gameObject;

        this.PlayerInstance.SetActive(true);
        this.PlayerInstance.transform.SetParent(this.transform);

        if (this.photonView.IsMine) EscapeGameManager.Instance.CheckVoiceGroup(true);
        if (this.photonView.IsMine) EscapeGameManager.Instance.WhoAreYou(true);
    }

    private void BornDemon(int gridStartIndex)
    {
        Debug.Log("Born a Demon" + gridStartIndex);

        this.PlayerInstance = this.transform.GetChild((gridStartIndex + 6)).gameObject;

        this.PlayerInstance.SetActive(true);
        this.PlayerInstance.transform.SetParent(this.transform);

        if (this.photonView.IsMine) EscapeGameManager.Instance.CheckVoiceGroup(false);
        if (this.photonView.IsMine) EscapeGameManager.Instance.WhoAreYou(false);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 為玩家本人的狀態, 將狀態更新給其他玩家
            if(this.photonView.IsMine)
            {
                if(IsDie) Debug.Log("Going to Die");
                stream.SendNext(IsDie);
                stream.SendNext(IsEscape);
                //stream.SendNext(Tagger);
            }
        }
        else
        {
            // 非為玩家本人的狀態, 單純接收更新的資料
            this.IsDie = (bool)stream.ReceiveNext();
            this.IsEscape = (bool)stream.ReceiveNext();
            //this.Tagger = (bool)stream.ReceiveNext();
        }
    }
}
