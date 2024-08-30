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
 

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    
    [Tooltip("遊戲開始時間")]
    [SerializeField] public float StartTime = 0f;

    [Tooltip("遊戲結束時間")]
    [SerializeField] public float EndTime = 300f;

    public GameObject spawnedPlayerPrefab;

    [SerializeField] public Text StartTimeText;

    public GameObject[] GetAllNetworkPlayer;

    public GameObject[] GetAllSurvive;

    private PhotonVoiceView recorder;

    /*------------------------------------------*/

    public GameObject DemonWin;

    public GameObject MushroomWin;

    public GameObject VoiceObject;

    public GameObject DemonSide;

    public GameObject MushroomSide;

    public Text TimeCount;

    public GameObject UIPanel;

    public GameObject Scream;

    public GameObject Fog;

    public TextMeshProUGUI ScreamCount;

    public TextMeshProUGUI FogCount;

    public GameObject BlurUi;

    /*------------------------------------------*/

    private int RestTime;

    private float CurrTime;

    private bool StartOrNot;

    private bool EndOrNot = false;

    int CurrSecond = 0;

    int PrevSecond = 0;

    void Start()
    {
        Instance = this;

        CurrTime = 0;
        StartOrNot = false;

        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Single Player Control", transform.position, transform.rotation);
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
            if(StartTime <= 10)
            {
                StartTime += Time.deltaTime;
                CurrSecond = (int)StartTime / 1;
                if(PrevSecond != CurrSecond)
                {
                    PrevSecond = CurrSecond;
                    VoiceControl.Instance.PlayVoice(0);
                }
                StartTimeText.text = (10 - ( (int)StartTime / 1 ) ).ToString();
            }
            else
            {
                PrevSecond = 0;
                CurrSecond = 0;
                StartTimeText.gameObject.SetActive(false);
                StartOrNot = true;

                Spell.Instance.IntoGame = true;
                Spell.Instance.SurOrEsc = true;
                SkillTrigger.Instance.SurOrEsc = false;

                if(PhotonNetwork.IsMasterClient) ChooseATagger();
            }
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

            GetAllSurvive = GameObject.FindGameObjectsWithTag("Taggee");
            if(GetAllSurvive.Length == 0)
            {
                GameOver(false);
            }
        }

        /*---------------------------------------------------*/

        if(CurrTime >= ( EndTime - 0.5f ) && !EndOrNot)
        {
            Debug.Log("Time is Over!");
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
    }

    public void GameOver(bool IsOverTime)
    {
        Debug.Log("Game Over!");

        TimeCount.gameObject.SetActive(false);
        MushroomSide.SetActive(false);
        DemonSide.SetActive(false);

        EndOrNot = true;

        //Time.timeScale = 0f;
        PhotonNetwork.Destroy(spawnedPlayerPrefab);

        if(!IsOverTime)
        {
            // DemonWin.SetActive(true);
            EndManager.Instance.ExecuteEndScene(true, false);
        }
        else
        {
            // MushroomWin.SetActive(true);
            EndManager.Instance.ExecuteEndScene(false, false);
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

        // PhotonNetwork.Destroy(spawnedPlayerPrefab);
        PhotonNetwork.LoadLevel("VRLoadScene");
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

    public void UpdateSkill(int n)
    {
        if(n == 0)
        {
            ScreamCount.text = Spell.Instance.ScreamCount.ToString();
        }

        else if(n == 2)
        {
            FogCount.text = Spell.Instance.PoisonCount.ToString();
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

            Scream.SetActive(false);
            Fog.SetActive(true);
        }
        else
        {
            MushroomSide.SetActive(false);
            DemonSide.SetActive(true);

            Scream.SetActive(true);
            Fog.SetActive(false);
        }
    }
}
