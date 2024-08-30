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

using TMPro;

using Krivodeling.UI.Effects;
 

public class EscapeGameManager : MonoBehaviourPunCallbacks
{
    public static EscapeGameManager Instance;
    
    [Tooltip("遊戲開始時間")]
    [SerializeField] public float StartTime = 5f;

    [Tooltip("遊戲結束時間")]
    [SerializeField] public float EndTime = 300f;

    /*------------------------------------------*/

    public GameObject DemonWin;

    public GameObject MushroomWin;

    public GameObject spawnedPlayerPrefab;

    public GameObject VoiceObject;

    public GameObject[] GetAllSurvivePlayer;

    public GameObject DemonSide;

    public GameObject MushroomSide;

    public GameObject UIPanel;

    public Text TimeCount;

    public GameObject Scream;

    public GameObject Fog;

    public GameObject Roar;

    public GameObject Fake;

    public TextMeshProUGUI RoarCount;

    public TextMeshProUGUI FakeCount;

    public TextMeshProUGUI ScreamCount;

    public TextMeshProUGUI FogCount;

    public GameObject BlurUi;

    /*------------------------------------------*/

    private int RestTime;

    private float CurrTime;

    private bool StartOrNot;

    private bool EndOrNot = false;

    public bool avoidTrigger = false;

    int CurrSecond = 0;

    int PrevSecond = 0;

    float tmpTime = 0f;

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
        if(StartTime >= 0)
        {
            StartTime -= Time.deltaTime;
        }
        else
        {
            StartOrNot = true;
        }

        /*---------------------------------------------------*/

        if(StartOrNot && !EndOrNot)
        {
            CurrTime += Time.deltaTime;

            RestTime = (int)(EndTime - CurrTime);

            TimeCount.text = (RestTime).ToString();

            if(RestTime <= 10)
            {
                CurrSecond = RestTime;
                if(PrevSecond != CurrSecond)
                {
                    PrevSecond = CurrSecond;
                    VoiceControl.Instance.PlayVoice(0);
                }
            }

            GetAllSurvivePlayer = GameObject.FindGameObjectsWithTag("Taggee");
            if(GetAllSurvivePlayer.Length == 0)
            {
                GameOver();
            }
        }

        /*---------------------------------------------------*/

        if(CurrTime >= EndTime && !EndOrNot)
        {
            Debug.Log("Time is over");
            GameOver(true);
        }

        /*---------------------------------------------------*/

        if(OVRInput.Get(OVRInput.Button.One) && !EndOrNot)
        {
            UIPanel.SetActive(true);
        }
        else
        {
            UIPanel.SetActive(false);
        }

        /*---------------------------------------------------*/

        if(EndOrNot && avoidTrigger)
        {
            if(tmpTime < 1) tmpTime += Time.deltaTime;
            else
            {
                PhotonNetwork.Destroy(spawnedPlayerPrefab);
                avoidTrigger = false;
            }
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

        // Time.timeScale = 0f;
        if(!avoidTrigger && spawnedPlayerPrefab != null) PhotonNetwork.Destroy(spawnedPlayerPrefab);

        if(GetAllSurvivePlayer.Length == 0 || IsOverTime)
        {
            // DemonWin.SetActive(true);
            EndManager.Instance.ExecuteEndScene(true, true);
        }
        else
        {
            // MushroomWin.SetActive(true);
            EndManager.Instance.ExecuteEndScene(false, true);
        }
    }

    // 玩家離開遊戲室時, 把他帶回到大廳
    public override void OnLeftRoom()
    {
        // PhotonNetwork.Destroy(spawnedPlayerPrefab);
        PhotonNetwork.LoadLevel("VRLoadScene");
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

    public void UpdateSkill(int n)
    {
        if(n == 0)
        {
            ScreamCount.text = Spell.Instance.ScreamCount.ToString();
        }
        else if(n == 1)
        {
            RoarCount.text = Spell.Instance.RoarCount.ToString();
        }
        else if(n == 2)
        {
            FogCount.text = Spell.Instance.PoisonCount.ToString();
        }
        else if(n == 3)
        {
            FakeCount.text = Spell.Instance.FakeShroomCount.ToString();
        }
    }

    public void ExecuteBlur(bool start)
    {
        if(start) BlurUi.GetComponent<UIBlur>().BeginBlur(1); 
        else BlurUi.GetComponent<UIBlur>().EndBlur(1); 
    }

    public void WhoAreYou(bool WhichTeam) // WhichTeam = true => Taggee // WhichTeam = false => Tagger
    {
        if(WhichTeam)
        {
            MushroomSide.SetActive(true);
            DemonSide.SetActive(false);

            Fog.SetActive(true);
            Fake.SetActive(true);
        }
        else
        {
            MushroomSide.SetActive(false);
            DemonSide.SetActive(true);

            Scream.SetActive(true);
            Roar.SetActive(true);
        }
    }
}

        // if(!StartOrNot && PhotonNetwork.CurrentRoom.PlayerCount > 0)
        // {
        //     StartTimeText.gameObject.SetActive(true);
        //     if(StartTime > 0)
        //     {
        //         StartTime -= Time.deltaTime;
        //         StartTimeText.text = ((int)StartTime/1).ToString();
        //     }
        //     else
        //     {
        //         StartTimeText.gameObject.SetActive(false);
        //         StartOrNot = true;
        //         //if(PhotonNetwork.IsMasterClient) ChooseATagger();
        //     }
        // }


