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

public class SinglePlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    //public static SinglePlayerControl Instance;

    public GameObject PlayerInstance;

    /*----------------------------------*/

    public bool IsToDemon = false;

    public bool IsToMushroom = false;

    public bool IsChange = false;

    /*----------------------------------*/

    private Transform CameraOffset;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    private Transform leftHand;

    private Transform rightHand;

    /*----------------------------------*/

    public Transform SkillPosition;

    public GameObject ScreamRoar;

    public GameObject PoisonFog;

    public bool ScreamFlag = false;

    public bool PoisonFlag = false;

    private bool EndScream = false;

    private bool EndFog = false;
    
    private int ScreamStall = 0;

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

        //Instance = this;

        int PlayerNum = this.photonView.Owner.GetPlayerNumber();

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

            MapPosition(rig.gameObject.transform, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(CameraOffset, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(headRig, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(leftHandRig, BornPlane.Instance.Positions[PlayerNum]);
            MapPosition(rightHandRig, BornPlane.Instance.Positions[PlayerNum]);
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

        /*--------------- Spell --------------*/

        if (this.photonView.IsMine) 
        {
            if(headRig != null) MapPosition(SkillPosition ,headRig);
            else Debug.Log("No Headrig");
        }

        if(ScreamFlag)
        {
            if (!this.photonView.IsMine)
            {
                Screaming();
            }
            else EndScream = true;
        }

        if(PoisonFlag)
        {
            if (!this.photonView.IsMine)
            {
                Fogging();
            }
            else EndFog = true;
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

    private void BornMushroom(int gridStartIndex)
    {
        Debug.Log("Born a Mushroom " + gridStartIndex);

        this.PlayerInstance = this.transform.GetChild(gridStartIndex).gameObject;
        this.PlayerInstance.SetActive(true);
        this.PlayerInstance.transform.SetParent(this.transform);

        if (this.photonView.IsMine) Spell.Instance.DemonOrShroom = false;

        if (this.photonView.IsMine) GameManager.Instance.CheckVoiceGroup(true);
        if (this.photonView.IsMine) GameManager.Instance.WhoAreYou(true);
    }

    private void BornDemon(int gridStartIndex)
    {
        Debug.Log("Born a Demon" + gridStartIndex);

        this.PlayerInstance = this.transform.GetChild((gridStartIndex+6)).gameObject;
        this.PlayerInstance.SetActive(true);
        this.PlayerInstance.transform.SetParent(this.transform);

        if (this.photonView.IsMine)
        {
            Spell.Instance.DemonOrShroom = true;

            leftHand.GetChild(0).gameObject.SetActive(false);
            leftHand.GetChild(1).gameObject.SetActive(true);
            rightHand.GetChild(0).gameObject.SetActive(false);
            rightHand.GetChild(1).gameObject.SetActive(true);

            CameraOffset.gameObject.GetComponent<GorillaLocomotion.Player>().jumpMultiplier = 1.8f;
        }

        OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.LTouch);
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
        if(BeginOrNot) GameManager.Instance.ExecuteBlur(true);
        else GameManager.Instance.ExecuteBlur(false);
    }

    public void Screaming()
    {
        VoiceControl.Instance.PlayVoice(9);
        ScreamRoar.SetActive(true);
    }

    public void Fogging()
    {
        VoiceControl.Instance.PlayVoice(11);
        PoisonFog.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 為玩家本人的狀態, 將狀態更新給其他玩家
            if(this.photonView.IsMine)
            {
                stream.SendNext(IsToDemon);
                // stream.SendNext(IsShock);
                // stream.SendNext(IsBlind);

                stream.SendNext(ScreamFlag);
                stream.SendNext(PoisonFlag);
            }
        }
        else
        {
            // 非為玩家本人的狀態, 單純接收更新的資料
            this.IsToDemon = (bool)stream.ReceiveNext();
            // this.IsShock = (bool)stream.ReceiveNext();
            // this.IsBlind = (bool)stream.ReceiveNext();

            this.ScreamFlag = (bool)stream.ReceiveNext();
            this.PoisonFlag = (bool)stream.ReceiveNext();
        }
    }
}
