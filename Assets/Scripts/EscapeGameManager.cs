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
 

public class EscapeGameManager : MonoBehaviourPunCallbacks
{
    public static EscapeGameManager Instance;
    
    [Tooltip("遊戲開始時間")]
    [SerializeField] public float StartTime = 3f;

    [Tooltip("遊戲結束時間")]
    [SerializeField] public float EndTime = 300f;

    public GameObject DemonWin;

    public GameObject MushroomWin;

    private GameObject spawnedPlayerPrefab;

    public GameObject VoiceObject;

    [SerializeField] public Text StartTimeText;

    public GameObject[] GetAllSurvivePlayer;

    public GameObject DemonSide;

    public GameObject MushroomSide;

    public Text TimeCount;

    private int RestTime;

    private float CurrTime;

    private bool StartOrNot;
    private bool EndOrNot = false;

    public GameObject UIPanel;

    void Start()
    {
        Instance = this;

        CurrTime = 0;
        StartOrNot = false;

        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Escape Player Control", transform.position, transform.rotation);
        Debug.Log("我創造出了自己!");
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
                //if(PhotonNetwork.IsMasterClient) ChooseATagger();
            }
        }

        if(StartOrNot && !EndOrNot)
        {
            CurrTime += Time.deltaTime;

            RestTime = (int)(EndTime - CurrTime);

            TimeCount.text = (RestTime).ToString();

            GetAllSurvivePlayer = GameObject.FindGameObjectsWithTag("Taggee");
            if(GetAllSurvivePlayer.Length == 0)
            {
                GameOver();
            }
        }

        if(CurrTime >= EndTime)
        {
            Debug.Log("Time is over");
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

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }

    public void GameOver(bool IsOverTime = false)
    {
        Debug.Log("Game over");

        EndOrNot = true;

        TimeCount.gameObject.SetActive(false);
        MushroomSide.SetActive(false);
        DemonSide.SetActive(false);

        Time.timeScale = 0f;

        if(GetAllSurvivePlayer.Length == 0 || IsOverTime)
        {
            DemonWin.SetActive(true);
        }
        else
        {
            MushroomWin.SetActive(true);
        }
    }

    // 玩家離開遊戲室時, 把他帶回到大廳
    public override void OnLeftRoom()
    {
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
        PhotonNetwork.LoadLevel("LoadScene");
        //PhotonNetwork.JoinLobby();
        //SceneManager.LoadScene(0);
    } 

    public void LeaveRoom()
    {
        DemonWin.SetActive(false);
        MushroomWin.SetActive(false);

        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("{0} 進入遊戲室", other.NickName);
    }

    public void CheckVoiceGroup(bool WhichTeam) // WhichTeam = true => Taggee // WhichTeam = false => Tagger
    {
        // if(WhichTeam)
        // {
        //     VoiceObject.GetComponent<Recorder>().InterestGroup = 1; // 1 for Mushroom
        // }
        // else
        // {
        //     VoiceObject.GetComponent<Recorder>().InterestGroup = 2; // 2 for Demon
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


