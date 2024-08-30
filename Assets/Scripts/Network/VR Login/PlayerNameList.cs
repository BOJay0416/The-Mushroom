using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

using TMPro;

namespace Photon.Pun.Demo.Asteroids
{
    public class PlayerNameList : MonoBehaviour
    {
        public TextMeshProUGUI PlayerNameText;

        private int ownerId;
        private bool isPlayerReady;

        bool avoidTrigger = false;

        float timeCount = 0f;

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                Debug.Log("fail");
            }
            else
            {
                Debug.Log("success");
                Hashtable initialProps = new Hashtable() {{AsteroidsGame.PLAYER_READY, isPlayerReady}, {AsteroidsGame.PLAYER_LIVES, AsteroidsGame.PLAYER_MAX_LIVES}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);

                // PlayerReadyButton.onClick.AddListener(() =>
                // {
                //     isPlayerReady = !isPlayerReady;
                //     SetPlayerReady(isPlayerReady);

                //     Hashtable props = new Hashtable() {{AsteroidsGame.PLAYER_READY, isPlayerReady}};
                //     PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                //     if (PhotonNetwork.IsMasterClient)
                //     {
                //         FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
                //     }
                // });
            }
        }

        public void SetReady()
        {
            isPlayerReady = !isPlayerReady;
            SetPlayerReady(isPlayerReady);

            Hashtable props = new Hashtable() {{AsteroidsGame.PLAYER_READY, isPlayerReady}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            if (PhotonNetwork.IsMasterClient)
            {
                FindObjectOfType<VRLoginControl>().LocalPlayerPropertiesUpdated();
            }
        }

        public void Initialize(int playerId, string playerName, GameObject Location)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;

            transform.position = Location.transform.GetChild(ownerId - 1).transform.position;
            transform.rotation = Location.transform.GetChild(ownerId - 1).transform.rotation;

            transform.SetParent(Location.transform.GetChild(ownerId));
            Debug.Log("List Init " + playerName);
        }

        private void OnPlayerNumberingChanged()
        {
            Debug.Log("OnPlayerNumberingChanged");
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    // PlayerColorImage.color = AsteroidsGame.GetColor(p.GetPlayerNumber());

                    // transform.position = Location.GetChild(p.GetPlayerNumber()).transform.position;
                    // transform.rotation = Location.GetChild(p.GetPlayerNumber()).transform.rotation;
                    // transform.SetParent(Location.GetChild(p.GetPlayerNumber()));

                    Debug.Log("Now I am " + ownerId);
                }
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
            Debug.Log("切換名字顏色");
            VoiceControl.Instance.PlayVoice(3);
            PlayerNameText.color = playerReady ?  new Color32(120, 209, 35, 255) : new Color32(225, 225, 225, 255);
        }

        void Update()
        {
            if(avoidTrigger)
            {
                timeCount += Time.deltaTime;
                if(timeCount >= 0.5)
                {
                    avoidTrigger = false;
                    timeCount = 0f;
                }
            }

            if( ( OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) ) || ( OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger) ))
            {
                if(PlayerNameText.text == VRLoginControl.Instance.UserName)
                {
                    Debug.Log("VR準備");
                    avoidTrigger = true;
                    SetReady();
                }
            }

            if(Input.GetKeyUp("g"))
            {
                Debug.Log("鍵盤準備");
                avoidTrigger = true;
                SetReady();
            }
        }
    }
}
