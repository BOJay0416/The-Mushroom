using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //[SerializeField] private byte maxPlayersPerRoom = 4; //控制人數 (尚未實作)
    [Tooltip("顯示/隱藏 遊戲玩家名稱與 Play 按鈕")]
    [SerializeField] private GameObject controlPanel;
    [Tooltip("顯示/隱藏 連線中 字串")]
    [SerializeField] private GameObject progressLabel;

    private GameObject spawnedPlayerPrefab;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update

    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        //ConnectToServer(); //把加入遊戲設成以按鈕執行
    }

    // Update is called once per frame
    public void ConnectToServer()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        
        PhotonNetwork.ConnectUsingSettings(); //原來的就是連線

        /*
        if (PhotonNetwork.IsConnected)
        {
            // 已連線, 嚐試隨機加入一個遊戲室
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // 未連線, 嚐試與 Photon Cloud 連線
            PhotonNetwork.ConnectUsingSettings();
        }
        */
        
        Debug.Log("conning to the server...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to server!");
        base.OnConnectedToMaster();
        /*
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsOpen = true;*/

        PhotonNetwork.JoinRandomOrCreateRoom(); //加入隨機Room
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN 呼叫 OnJoinRandomFailed(), 隨機加入遊戲室失敗.");
        
        // 隨機加入遊戲室失敗. 可能原因是 1. 沒有遊戲室 或 2. 有的都滿了.    
        // 我們自己開一個遊戲室.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
        progressLabel.SetActive(false);
        controlPanel.SetActive(false);
        Debug.Log("進入遊戲房間");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("我是第一個進入遊戲室的玩家");
            Debug.Log("我得主動做載入場景 'Room for 1' 的動作");
            PhotonNetwork.LoadLevel("Game Scene 1");
            spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
        }
        //base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }
} 