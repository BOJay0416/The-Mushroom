using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class RoomBackObject : MonoBehaviour
    {
        public bool avoidTrigger = false;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !avoidTrigger)
            {
                avoidTrigger = true;
                VRLoginControl.Instance.OnLeaveGameButtonClicked();
            }
        }
    }
}
