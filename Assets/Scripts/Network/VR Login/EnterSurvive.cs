using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Demo.Asteroids
{
    public class EnterSurvive : MonoBehaviour
    {
        public bool avoidTrigger = false;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !avoidTrigger)
            {
                avoidTrigger = true;
                VRLoginControl.Instance.OnStartSurviveGameClicked();
            }
        }
    }
}
