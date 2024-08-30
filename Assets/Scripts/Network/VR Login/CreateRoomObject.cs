using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class CreateRoomObject : MonoBehaviour
    {
        public GameObject CreateRoomPanel;

        public CapsuleCollider createCollider;

        void start()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                CreateRoomPanel.SetActive(true);
                GetComponent<Collider>().enabled = false;
            }
        }

        public void recoverCollider()
        {
            GetComponent<Collider>().enabled = true;
        }
    }
}

