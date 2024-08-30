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

using Unity.XR.CoreUtils;

using GorillaLocomotion;

public class EscapePlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject PlayerInstance;

    /*----------------------------------*/

    //public GameObject[] DieCamera;

    public bool IsDie = false;

    public bool IamDie = false;

    public bool IsEscape = false;

    //private bool IsChange = false;

    public bool Tagger = false;

    public int PlayerNum;

    /*----------------------------------*/

    private Transform CameraOffset;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    private Transform leftHand;

    private Transform rightHand;

    /*----------------------------------*/

    public Transform SkillPosition;

    public GameObject DestroyRoar;

    public GameObject ScreamRoar;

    public Transform FakeShroomPosition;

    public GameObject PoisonFog;

    public bool ScreamFlag = false;

    public bool DestroyFlag = false;

    public bool PoisonFlag = false;

    public bool FakeShroomFlag = false;

    private bool EndScream = false;

    private bool EndRoar = false;

    private bool EndFog = false;

    private int ScreamStall = 0;

    private int RoarStall = 0;

    private int FogStall = 0;

    /*----------------------------------*/

    public bool IsShock = false;

    private bool GetShock = false;

    public bool IsBlind = false;

    private bool GetBlind = false;

    float ShockTime = 0f;

    float BlindTime = 0f;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => this.photonView.Owner.GetPlayerNumber() >= 0);

        PlayerNum = this.photonView.Owner.GetPlayerNumber();

        if (this.photonView.IsMine) 
        {
            Debug.Log("I am " + PlayerNum);

            // XRRig rig = FindObjectOfType<XRRig>();
            XROrigin rig = FindObjectOfType<XROrigin>();

            CameraOffset = rig.transform.Find("Camera Offset");
            headRig = rig.transform.Find("Camera Offset/Main Camera");
            leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
            rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");
            leftHand = rig.transform.Find("Left");
            rightHand = rig.transform.Find("Right");

            CameraOffset.GetComponent<SkillTrigger>().MyController = this.gameObject;
            CameraOffset.GetComponent<GorillaLocomotion.Player>().Teleporting = true;
            Spell.Instance.MyController = this.gameObject;
            Spell.Instance.IntoGame = true;
            Spell.Instance.SurOrEsc = false;

            MapPosition(rig.gameObject.transform, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(CameraOffset, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(headRig, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(leftHandRig, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(rightHandRig, BornPlane.Instance.Positions[PlayerNum]);
        }
        
        SkillTrigger.Instance.SurOrEsc = true;

        if(PlayerNum %2 ==0)
        {
            if (this.photonView.IsMine) Spell.Instance.DemonOrShroom = false;
            this.BornMushroom(PlayerNum);
        }
        else
        {
            if (this.photonView.IsMine) Spell.Instance.DemonOrShroom = true;
            this.BornDemon(PlayerNum);
        }
    }

    void Update()
    {
        if(IsDie && !IamDie)
        {
            Dieing();
        }

        /*--------------- Check Flag --------------*/

        if(EndScream)
        {
            if( ScreamStall < 10) ScreamStall++;
            else
            {
                Debug.Log("EndScream");
                ScreamFlag = false;
                EndScream = false;
                ScreamStall = 0;
            }
        }
        if(EndRoar)
        {
            if(RoarStall < 10 ) RoarStall++;
            else
            {
                Debug.Log("EndRoar");
                DestroyFlag = false;
                EndRoar = false;
                RoarStall = 0;
            }
        }
        if(EndFog)
        {
            if(FogStall < 10) FogStall++;
            else
            {
                Debug.Log("EndFog");
                PoisonFlag = false;
                EndFog = false;
                FogStall = 0;
            }
        }

        /*--------------- ----- --------------*/

        if(IsEscape)
        {
            EscapeGameManager.Instance.GameOver();
        }

        /*--------------- Spell --------------*/

        if (this.photonView.IsMine) 
        {
            if(headRig != null) MapPosition(SkillPosition ,headRig);
            else Debug.Log("No Headrig");
        }

        if(ScreamFlag)
        {
            Debug.Log("收到scream");
            if (!this.photonView.IsMine)
            {
                Debug.Log("收到了 要scream囉");
                Screaming();
            }
            else EndScream = true;
        }

        if(DestroyFlag)
        {
            Debug.Log("收到Roar");
            if (!this.photonView.IsMine)
            {
                Debug.Log("收到了 要Roar囉");
                Roaring();
            }
           else  EndRoar = true;
        }

        if(PoisonFlag)
        {
            Debug.Log("收到fog");
            if (!this.photonView.IsMine)
            {
                Debug.Log("收到了 要fog囉");
                Fogging();
            }
            else EndFog = true;
        }

        if(FakeShroomFlag)
        {
            PhotonNetwork.Instantiate("Fake " + PlayerNum , FakeShroomPosition.position, FakeShroomPosition.rotation);
            Debug.Log("在網路中生成假蘑菇");
            FakeShroomFlag = false;
        }

        /*--------------- Shock --------------*/

        if(IsShock)
        {
            if (this.photonView.IsMine)  
            {
                if (!GetShock)  
                {
                    Debug.Log("被定身了");
                    Shocking(true);
                    GetShock = true;
                }
                else
                {
                    Debug.Log("又被定身了");
                    ShockTime = 0f;
                }
                IsShock = false;
            }
        }
        if(GetShock)
        {
            if (this.photonView.IsMine)  
            {
                ShockTime += Time.deltaTime;
                if(ShockTime >= 5)
                {
                    Shocking(false);
                    GetShock = false;
                    ShockTime = 0f;
                }
            }
        }

        /*--------------- Blind --------------*/

        if(IsBlind)
        {
            if (this.photonView.IsMine)  
            {
                if (!GetBlind)  
                {
                    Debug.Log("被致盲了");
                    Blinding(true);
                    GetBlind = true;
                }
                else
                {
                    Debug.Log("又被致盲了");
                    BlindTime = 0f;
                }
                IsBlind = false;
            }
        }
        if(GetBlind)
        {
            BlindTime += Time.deltaTime;
            if(BlindTime >= 5)
            {
                Blinding(false);
                GetBlind = false;
                BlindTime = 0f;
            }
        }     
    }

    private void Dieing()
    {
        this.PlayerInstance.SetActive(false);
        // CameraOffset.gameObject.GetComponent<GorillaLocomotion.Player>().disableMovement = true;

        if (this.photonView.IsMine) 
        {
            // headRig.gameObject.SetActive(false);
            // DieCamera.Instance.Camera[PlayerNum/2].gameObject.SetActive(true);
            OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);
            OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.LTouch);
            EscapeShroomDie.Instance.ExecuteDie(PlayerNum/2);
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

        if (this.photonView.IsMine)
        {
            leftHand.GetChild(0).gameObject.SetActive(false);
            leftHand.GetChild(1).gameObject.SetActive(true);
            rightHand.GetChild(0).gameObject.SetActive(false);
            rightHand.GetChild(1).gameObject.SetActive(true);
            CameraOffset.gameObject.GetComponent<GorillaLocomotion.Player>().jumpMultiplier = 1.8f;
        }

        if (this.photonView.IsMine) EscapeGameManager.Instance.CheckVoiceGroup(false);
        if (this.photonView.IsMine) EscapeGameManager.Instance.WhoAreYou(false);
    }

    public void Shocking( bool BeginOrNot )
    {
        Debug.Log("Shock !");

        if(BeginOrNot)
        {
            leftHandRig.GetComponent<XRController>().enableInputTracking = false;
            rightHandRig.GetComponent<XRController>().enableInputTracking = false;
        }
        else
        {
            leftHandRig.GetComponent<XRController>().enableInputTracking = true;
            rightHandRig.GetComponent<XRController>().enableInputTracking = true;
        }
    }

    public void Blinding( bool BeginOrNot )
    {
        Debug.Log("Blind !");
        if(BeginOrNot) EscapeGameManager.Instance.ExecuteBlur(true);
        else EscapeGameManager.Instance.ExecuteBlur(false);
    }

    public void Screaming()
    {
        VoiceControl.Instance.PlayVoice(9);
        ScreamRoar.SetActive(true);
    }

    public void Roaring()
    {
        VoiceControl.Instance.PlayVoice(10);
        DestroyRoar.SetActive(true);
    }

    public void Fogging()
    {
        VoiceControl.Instance.PlayVoice(11);
        PoisonFog.SetActive(true);
    }

    public void DropFake()
    {
        Debug.Log("有人丟了顆蘑菇");
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
                Debug.Log("送出");
                stream.SendNext(IsDie);
                stream.SendNext(IsEscape);
                //stream.SendNext(Tagger);

                // stream.SendNext(IsShock);
                // stream.SendNext(IsBlind);

                stream.SendNext(ScreamFlag);
                stream.SendNext(PoisonFlag);
                stream.SendNext(DestroyFlag);
            }
        }
        else
        {
            // 非為玩家本人的狀態, 單純接收更新的資料
            this.IsDie = (bool)stream.ReceiveNext();
            this.IsEscape = (bool)stream.ReceiveNext();

            // this.IsShock = (bool)stream.ReceiveNext();
            // this.IsBlind = (bool)stream.ReceiveNext();

            this.ScreamFlag = (bool)stream.ReceiveNext();
            this.PoisonFlag = (bool)stream.ReceiveNext();
            this.DestroyFlag = (bool)stream.ReceiveNext();
        }
    }
}
