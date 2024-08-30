using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class LoginObject : MonoBehaviour
    {
        public GameObject LoginPanel;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                LoginPanel.SetActive(true);
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
