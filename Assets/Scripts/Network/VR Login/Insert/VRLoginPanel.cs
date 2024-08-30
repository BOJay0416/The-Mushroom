using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class VRLoginPanel : MonoBehaviour
    {
        public GameObject Layer1;
        public GameObject Layer2;
        public GameObject Layer3;
        public GameObject Layer4;

        public GameObject connectPanel;
        public GameObject loginPanel;

        int currLayer = 0;

        public Text playerNameText;

        public string nameInput;

        // Start is called before the first frame update
        void Start()
        {
            nameInput = "Player001";
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void enterKey()
        {
            if (!nameInput.Equals(""))
            {
                Debug.Log("Login in!");
                Debug.Log("Please wait for connecting...");
                connectPanel.SetActive(true);
                loginPanel.SetActive(false);
                VRLoginControl.Instance.UserName = nameInput;

                PhotonNetwork.LocalPlayer.NickName = nameInput;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }

            VoiceControl.Instance.PlayVoice(3);
        }

        public void backKey()
        {
            Debug.Log("Delete");
            nameInput = nameInput.Substring(0,nameInput.Length - 1);
            playerNameText.text = nameInput;

            VoiceControl.Instance.PlayVoice(1);
        }

        public void inputName(string c)
        {
            Debug.Log("Input " + c);
            nameInput += c;
            playerNameText.text = nameInput;

            VoiceControl.Instance.PlayVoice(1);
        }

        public void changeKey(int i)
        {
            currLayer = (currLayer + i + 4) % 4;
            Debug.Log("Keyboard Going " + currLayer);
            SetActivePanel(currLayer +1 );

            VoiceControl.Instance.PlayVoice(1);
        }

        public void SetActivePanel(int i)
        {
            string activePanel = "Layer " + i;
            Debug.Log("Keyboard " + activePanel);

            Layer1.SetActive(activePanel.Equals(Layer1.name));
            Layer2.SetActive(activePanel.Equals(Layer2.name));
            Layer3.SetActive(activePanel.Equals(Layer3.name)); 
            Layer4.SetActive(activePanel.Equals(Layer4.name));
        }
    }
}
