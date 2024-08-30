using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class RoomObject : MonoBehaviour
    {
        public Text RoomNameText;
        public Text RoomPlayersText;

        public bool avoidTrigger = false;

        private string roomName;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Initialize(string name, byte currentPlayers, byte maxPlayers)
        {
            roomName = name;

            RoomNameText.text = name;
            RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !avoidTrigger)
            {
                avoidTrigger = true;
                if (!PhotonNetwork.InLobby)
                {
                    PhotonNetwork.JoinLobby();
                }   
                PhotonNetwork.JoinRoom(roomName);
                // gameObject.GetComponent<SphereCollider>().enabled = false;
            }
        }
    }
}
