using ExitGames.Client.Photon;
using Photon.Realtime;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun.UtilityScripts;

using Photon.Voice.Unity;

using Photon.Voice.PUN;

using GorillaLocomotion;

namespace Photon.Pun.Demo.Asteroids
{
    public class VRLoginControl : MonoBehaviourPunCallbacks
    {
        public static VRLoginControl Instance;

        [Header("Login Panel")]
        public GameObject LoginPanel;
        public GameObject LoginDoor;
        public string UserName;

        [Header("CreateRoom Panel")]
        public GameObject CreateRoomFail; //提示訊息
        public GameObject CreateRoomShroom;
        public GameObject RoomIndex; //代表房間的蘑菇
        public string MaxPlayersInput = "6";
        public Transform LobbyLocation;

        [Header("Inside Room Panel")]
        public GameObject StartSurviveGame; // 帶碰撞的漩渦
        public GameObject StartEscapeGame; // 帶碰撞的漩渦
        public int PlayerNumInRoom = 0;
        public GameObject PlayerListEntryPrefab; // 名字的canvas
        public GameObject NameLocation; // 名字的位置
        public Transform RoomLocation;
        public GameObject RoomBackShroom;

        [Header("Network Player")]
        private GameObject spawnedPlayerPrefab;
        private PhotonVoiceView recorder;

        [Header("Other Settings")]
        public GameObject PlayerControl_01;
        public GameObject OpVideo;
        public bool Singin = false;
        private bool SelectingRoom = false;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;

        #region UNITY

        public void Awake() // 沒問題
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
            
        }

        void Start()
        {
            Instance = this;
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster() // 再想想，還需要改改
        {
            // OpVideo.SetActive(false);

            Destroy(LoginPanel);
            LoginDoor.GetComponent<BoxCollider>().enabled = false;
            Singin = true;
            UserName = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log("Connect success");
            // 登入成功 讓玩家可以開始移動
            SelectingRoom = true;

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList) // 沒問題
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();

            Debug.Log("Update Roomlist");
        }

        public override void OnJoinedLobby() // 沒問題
        {
            // whenever this joins a new lobby, clear any previous room lists
            Debug.Log("Joined Lobby");

            VoiceControl.Instance.PlayVoice(2);

            cachedRoomList.Clear();
            ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby() // 沒問題
        {
            Debug.Log("Left Room");
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message) // 沒問題
        {
            Debug.Log("Fail to create room");
            CreateRoomFail.SetActive(true);
        }

        public override void OnJoinRoomFailed(short returnCode, string message) // 沒問題
        {
            Debug.Log("Fail to join room");
        }

        public override void OnJoinedRoom()  // 再想想，還需要改改
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            CreateRoomShroom.GetComponent<CreateRoomObject>().recoverCollider();

            cachedRoomList.Clear();
            CreateRoomFail.SetActive(false);
            SelectingRoom = false;

            VoiceControl.Instance.PlayVoice(2);
            
            Debug.Log("OnJoined Room");

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                // 處理玩家名單
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.GetComponent<PlayerNameList>().Initialize(p.ActorNumber, p.NickName, NameLocation);
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerNameList>().SetPlayerReady((bool) isPlayerReady);
                }
                playerListEntries.Add(p.ActorNumber, entry);

                PlayerNumInRoom++;
            }

            // int n = PhotonNetwork.Owner.GetPlayerNumber();
            int n = PhotonNetwork.LocalPlayer.ActorNumber;
            Debug.Log("我是 " + n + "個，準備出生");
            Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerNumber());
            Debug.Log(PhotonNetwork.LocalPlayer.NickName);

            Transform CameraOffset = PlayerControl_01.transform.Find("Camera Offset");
            Transform headRig = PlayerControl_01.transform.Find("Camera Offset/Main Camera");
            Transform leftHandRig = PlayerControl_01.transform.Find("Camera Offset/LeftHand Controller");
            Transform rightHandRig = PlayerControl_01.transform.Find("Camera Offset/RightHand Controller");

            // 切換身體
            CameraOffset.GetComponent<GorillaLocomotion.Player>().Teleporting = true;
            MapPosition(PlayerControl_01.transform, RoomLocation.GetChild(n).GetChild(0) );
            MapPosition(CameraOffset, RoomLocation.GetChild(n).GetChild(0) );
            MapPosition(headRig, RoomLocation.GetChild(n).GetChild(0) );
            MapPosition(leftHandRig, RoomLocation.GetChild(n).GetChild(1) );
            MapPosition(rightHandRig, RoomLocation.GetChild(n).GetChild(2) );
            // 在網路中創造自己
            spawnedPlayerPrefab = PhotonNetwork.Instantiate("Login Player Control", PlayerControl_01.transform.position, PlayerControl_01.transform.rotation);
            Debug.Log("我創造出了自己!");
            // 開啟語音連線
            PhotonVoiceView photonVoiceView = spawnedPlayerPrefab.GetComponent<PhotonVoiceView>();
            this.recorder = photonVoiceView;

            StartSurviveGame.gameObject.SetActive(CheckPlayersReady());
            StartEscapeGame.gameObject.SetActive(CheckPlayersReady() && (PlayerNumInRoom % 2 == 0 ));

            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            RoomBackShroom.SetActive(true);
        }

        public override void OnLeftRoom() //  可能可以 -> 怪怪的 位置會跑掉
        {
            Debug.Log("Left room");

            Transform CameraOffset = PlayerControl_01.transform.Find("Camera Offset");
            Transform headRig = PlayerControl_01.transform.Find("Camera Offset/Main Camera");
            Transform leftHandRig = PlayerControl_01.transform.Find("Camera Offset/LeftHand Controller");
            Transform rightHandRig = PlayerControl_01.transform.Find("Camera Offset/RightHand Controller");

            // 切換身體
            CameraOffset.GetComponent<GorillaLocomotion.Player>().Teleporting = true;
            MapPosition(PlayerControl_01.transform, LobbyLocation.GetChild(0) );
            MapPosition(CameraOffset, LobbyLocation.GetChild(0) );
            MapPosition(headRig, LobbyLocation.GetChild(0) );
            MapPosition(leftHandRig, LobbyLocation.GetChild(1) );
            MapPosition(rightHandRig, LobbyLocation.GetChild(2) );
            // 刪除網路中的自己
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
            Debug.Log("我刪了自己");

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }
            PlayerNumInRoom = 0;
            SelectingRoom = true;

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) // 可能可以
        {
            Debug.Log("New Player Enter!");

            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.GetComponent<PlayerNameList>().Initialize(newPlayer.ActorNumber, newPlayer.NickName, NameLocation);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            PlayerNumInRoom++;

            StartSurviveGame.gameObject.SetActive(CheckPlayersReady());
            StartEscapeGame.gameObject.SetActive(CheckPlayersReady() && (PlayerNumInRoom % 2 == 0 ));
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // 可能可以
        {
            Debug.Log("A player left");

            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            PlayerNumInRoom--;

            ReloadPlayerName();

            StartSurviveGame.gameObject.SetActive(CheckPlayersReady());
            StartEscapeGame.gameObject.SetActive(CheckPlayersReady() && (PlayerNumInRoom % 2 == 0 ) );
        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) // 沒問題
        {
            Debug.Log("Swift Master");
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartSurviveGame.gameObject.SetActive(CheckPlayersReady());
                StartEscapeGame.gameObject.SetActive(CheckPlayersReady() && (PlayerNumInRoom % 2 == 0 ));
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // 再想想，還需要改改，好像不用改
        {
            Debug.Log("Update Player Property");
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerNameList>().SetPlayerReady((bool) isPlayerReady);
                }
            }

            StartSurviveGame.gameObject.SetActive(CheckPlayersReady());
            StartEscapeGame.gameObject.SetActive(CheckPlayersReady() && (PlayerNumInRoom % 2 == 0 ));
        }

        #endregion

        #region UI CALLBACKS

        public void ReloadPlayerName()
        {
            Debug.Log("reload name");
            int i = 0;
            foreach (GameObject entry in playerListEntries.Values)
            {
                entry.transform.position = NameLocation.transform.GetChild(i).transform.position;
                entry.transform.rotation = NameLocation.transform.GetChild(i).transform.rotation;
                entry.transform.SetParent(NameLocation.transform.GetChild(i));
                i++;
            }
        }

        public void OnBackButtonClicked() // ? 大廳中離開
        {

            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

        }

        public void OnCreateRoomButtonClicked(string roomName) //放在 create room mushroom 那邊
        {
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInput, out maxPlayers);
            maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions {MaxPlayers = maxPlayers, PlayerTtl = 10000 };

            Debug.Log("Creating room...");

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public void OnLeaveGameButtonClicked() // 離開當前房間 該處理了
        {
            Debug.Log("離開當前房間");
            RoomBackShroom.GetComponent<RoomBackObject>().avoidTrigger = false;
            PhotonNetwork.LeaveRoom();
            RoomBackShroom.SetActive(false);
        }

        public void OnStartSurviveGameClicked() //放在 room 裡面
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            Debug.Log("ENTER SURVIVE GAME MODE !");

            PhotonNetwork.LoadLevel("SurviveMode");
        }

        public void OnStartEscapeGameClicked() //放在 room 裡面
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            Debug.Log("ENTER ESCPAE GAME MODE !");

            PhotonNetwork.LoadLevel("EscapeMode");
        }

        #endregion

        private bool CheckPlayersReady() // 沒問題
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool) isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        
        private void ClearRoomListView() // 沒問題
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                entry.GetComponent<RoomObject>().avoidTrigger = false;
                entry.SetActive(false);
                // Destroy(entry.gameObject);
            }
            Debug.Log("Clear Room shroom");

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated() // 沒問題
        {
            StartSurviveGame.gameObject.SetActive(CheckPlayersReady());
            StartEscapeGame.gameObject.SetActive(CheckPlayersReady() && (PlayerNumInRoom % 2 == 0 ));
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList) // 沒問題
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }

            Debug.Log("Update Room cache");
        }

        private void UpdateRoomListView() // 沒問題
        {
            int i = 0;
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                // GameObject entry = Instantiate(RoomListEntryPrefab);
                // entry.transform.SetParent(RoomListContent.transform);
                // entry.transform.localScale = Vector3.one;
                // entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                // roomListEntries.Add(info.Name, entry);

                GameObject entry = RoomIndex.transform.GetChild(i).gameObject;
                entry.SetActive(true);
                entry.GetComponent<RoomObject>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
                i++;
            }
            
            VoiceControl.Instance.PlayVoice(2);
            Debug.Log("Update Room shroom");
        }

        public void CheckVoiceGroup(bool WhichTeam)
        {
            Debug.Log("Check " + PhotonNetwork.NetworkClientState + " and " + Photon.Realtime.ClientState.Joined);
        }

        void MapPosition(Transform target, Transform rigTransform)
        {
            target.position = rigTransform.position;
            target.rotation = rigTransform.rotation;
        }

        void Update()
        {
            // if(SelectingRoom)
            // {
            //     if( OVRInput.GetUp(OVRInput.Button.One) )
            //     {
            //         ClearRoomListView();
            //         UpdateCachedRoomList(roomList);
            //         UpdateRoomListView();

            //         VoiceControl.Instance.PlayVoice(2);

            //         Debug.Log("手動 Update Roomlist");
            //     }
            // }
        }
    }
}
