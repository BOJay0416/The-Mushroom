using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Photon.Pun.Demo.Asteroids
{
    public class InputKey : MonoBehaviour
    {
        public GameObject LoginObject;

        public GameObject CreateObject;

        void OnTriggerEnter(Collider other)
        {
            if(!VRLoginControl.Instance.Singin)
            {
                if (other.gameObject.CompareTag("InputKey"))
                {
                    LoginObject.GetComponent<VRLoginPanel>().inputName(other.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text);
                }

                if (other.gameObject.CompareTag("BackKey"))
                {
                    LoginObject.GetComponent<VRLoginPanel>().backKey();
                }

                if (other.gameObject.CompareTag("EnterKey"))
                {
                    LoginObject.GetComponent<VRLoginPanel>().enterKey();
                }

                if (other.gameObject.CompareTag("Up"))
                {
                    LoginObject.GetComponent<VRLoginPanel>().changeKey(-1);
                }

                if (other.gameObject.CompareTag("Down"))
                {
                    LoginObject.GetComponent<VRLoginPanel>().changeKey(1);
                }
            }
            else if(VRLoginControl.Instance.Singin)
            {
                if (other.gameObject.CompareTag("InputKey"))
                {
                    CreateObject.GetComponent<VRCreatePanel>().inputName(other.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text);
                }

                if (other.gameObject.CompareTag("BackKey"))
                {
                    CreateObject.GetComponent<VRCreatePanel>().backKey();
                }

                if (other.gameObject.CompareTag("EnterKey"))
                {
                    CreateObject.GetComponent<VRCreatePanel>().enterKey();
                }

                if (other.gameObject.CompareTag("Up"))
                {
                    CreateObject.GetComponent<VRCreatePanel>().changeKey(-1);
                }

                if (other.gameObject.CompareTag("Down"))
                {
                    CreateObject.GetComponent<VRCreatePanel>().changeKey(1);
                }
            }
        }
    }
}
