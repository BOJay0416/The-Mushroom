using System.Collections;

using System.Collections.Generic;

using Photon.Pun;

using Photon.Realtime;

using UnityEngine;

using UnityEngine.SceneManagement;

using UnityEngine.UI;

using UnityEngine.Networking;

// using UnityEngine.XR;

// using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun.UtilityScripts;

using Photon.Voice.Unity;

using Photon.Voice.PUN;
 

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    
    [Tooltip("遊戲開始時間")]
    [SerializeField] public float StartTime = 10f;

    [Tooltip("遊戲結束時間")]
    [SerializeField] public float EndTime = 300f;

    private GameObject spawnedPlayerPrefab;

    [SerializeField] public Text StartTimeText;

    public GameObject[] GetAllNetworkPlayer;

    public GameObject[] GetAllSurvive;

    public GameObject DemonWin;

    public GameObject MushroomWin;

    public GameObject VoiceObject;

    public GameObject DemonSide;

    public GameObject MushroomSide;

    public Text TimeCount;

    private int RestTime;

    private PhotonVoiceView recorder;

    private float CurrTime;

    private bool StartOrNot;

    private bool EndOrNot = false;

    public GameObject UIPanel;

    void Start()
    {
        Instance = this;

        CurrTime = 0;
        StartOrNot = false;

        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Single Player Control 1", transform.position, transform.rotation);
        Debug.Log("我創造出了自己!");

        PhotonVoiceView photonVoiceView = spawnedPlayerPrefab.GetComponent<PhotonVoiceView>();
        this.recorder = photonVoiceView;

        RestTime = (int)EndTime;
    }

    void Update()
    {
        if(!StartOrNot && PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            StartTimeText.gameObject.SetActive(true);
            if(StartTime > 0)
            {
                StartTime -= Time.deltaTime;
                StartTimeText.text = ((int)StartTime/1).ToString();
            }
            else
            {
                StartTimeText.gameObject.SetActive(false);
                StartOrNot = true;
                if(PhotonNetwork.IsMasterClient) ChooseATagger();
            }
        }

        if(StartOrNot && !EndOrNot)
        {
            CurrTime += Time.deltaTime;
            RestTime = (int)(EndTime - CurrTime);

            TimeCount.text = (RestTime).ToString();

            GetAllSurvive = GameObject.FindGameObjectsWithTag("Taggee");
            if(GetAllSurvive.Length == 0)
            {
                GameOver(false);
            }
            else Debug.Log("Not Yet");
        }

        if(CurrTime >= EndTime)
        {
            Debug.Log("Time is Over!");
            GameOver(true);
        }

        if(EndOrNot)
        {
            if(Input.GetKeyUp("x"))
            {
                EndOrNot = false;
                LeaveRoom();
            }
            if(OVRInput.GetUp(OVRInput.Button.Two))
            {
                EndOrNot = false;
                LeaveRoom();
            }
        }

        if(Input.GetKey("a"))
        {
            UIPanel.SetActive(true);
        }
        else
        {
            UIPanel.SetActive(false);
        }

        if(OVRInput.GetUp(OVRInput.Button.One))
        {
            UIPanel.SetActive(true);
        }
        else
        {
            UIPanel.SetActive(false);
        }
    }

    public void GameOver(bool IsOverTime)
    {
        Debug.Log("Game Over!");

        TimeCount.gameObject.SetActive(false);
        MushroomSide.SetActive(false);
        DemonSide.SetActive(false);

        EndOrNot = true;

        //Time.timeScale = 0f;

        if(!IsOverTime)
        {
            DemonWin.SetActive(true);
        }
        else
        {
            MushroomWin.SetActive(true);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }

    void ChooseATagger()
    {
        Debug.Log("Now it's going to choose a tagger in " + PhotonNetwork.CurrentRoom.PlayerCount + " player");

        GetAllNetworkPlayer = GameObject.FindGameObjectsWithTag("Player");

        int RandomNum = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount-1);
        Debug.Log("Choose " + RandomNum);

        GetAllNetworkPlayer[RandomNum].GetComponent<SinglePlayerControl>().IsToDemon = true;
    }

    // 玩家離開遊戲室時, 把他帶回到大廳
    public override void OnLeftRoom()
    {
        Debug.Log("回大廳");

        PhotonNetwork.Destroy(spawnedPlayerPrefab);
        PhotonNetwork.LoadLevel("LoadScene");
        //SceneManager.LoadScene(0);
    } 

    public void LeaveRoom()
    {
        DemonWin.SetActive(false);
        MushroomWin.SetActive(false);

        Debug.Log("離開");

        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("{0} 進入遊戲室", other.NickName);
    }

    // public void CheckVoiceGroup(bool WhichTeam) // WhichTeam = true => Taggee // WhichTeam = false => Tagger
    // {
    //     if(WhichTeam)
    //     {
    //         this.recorder.RecorderInUse.InterestGroup = 1; // 1 for Mushroom
    //     }
    //     else
    //     {
    //         this.recorder.RecorderInUse.InterestGroup = 2; // 2 for Demon
    //     }
    // }

    public void CheckVoiceGroup(bool WhichTeam) // WhichTeam = true => Taggee // WhichTeam = false => Tagger
    {
        Debug.Log("Check " + PhotonNetwork.NetworkClientState + " and " + Photon.Realtime.ClientState.Joined);
        // yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined);
        // if(WhichTeam)
        // {
        //     byte[] temp = new byte[1];
        //     temp[0] = (byte)1;
        //     VoiceObject.GetComponent<Recorder>().InterestGroup = 1; // 1 for Mushroom
        //     PunVoiceClient.Instance.Client.OpChangeGroups(null, temp);
        //     //PhotonNetwork.SetInterestGroups(null, temp);
        //     //PhotonNetwork.SetSendingEnabled(null, temp);
        // }
        // else
        // {
        //     byte[] temp = new byte[1];
        //     temp[0] = (byte)2;
        //     VoiceObject.GetComponent<Recorder>().InterestGroup = 2; // 2 for Demon
        //     PunVoiceClient.Instance.Client.OpChangeGroups(null, temp);
        //     //PhotonNetwork.SetInterestGroups(null, temp);
        //     //PhotonNetwork.SetSendingEnabled(null, temp);
        // }
    }

    public void WhoAreYou(bool WhichTeam) // WhichTeam = true => Taggee // WhichTeam = false => Tagger
    {
        if(WhichTeam)
        {
            MushroomSide.SetActive(true);
            DemonSide.SetActive(false);
        }
        else
        {
            MushroomSide.SetActive(false);
            DemonSide.SetActive(true);
        }
    }
}
