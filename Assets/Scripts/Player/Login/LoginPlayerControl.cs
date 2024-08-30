using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun;

using Photon.Realtime;

using UnityEngine.XR;

using UnityEngine.Networking;

using UnityEngine.SceneManagement;

using Photon.Pun.UtilityScripts;

using Unity.XR.CoreUtils;

public class LoginPlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    private GameObject PlayerInstance;

    public Transform CameraOffset;

    public Transform headRig;

    public Transform leftHandRig;

    public Transform rightHandRig;

    public int PlayerNum;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => this.photonView.Owner.GetPlayerNumber() >= 0);

        PlayerNum = this.photonView.Owner.GetPlayerNumber();

        if (this.photonView.IsMine) 
        {
            Debug.Log("I am " + PlayerNum);

            // XRRig rig = FindObjectOfType<XRRig>();
            XROrigin rig = FindObjectOfType<XROrigin>();

            CameraOffset = rig.transform.Find("Camera Offset");
            headRig = rig.transform.Find("Camera Offset/Main Camera");
            leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
            rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");
        }
        this.BornMushroom(PlayerNum);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void BornMushroom(int gridStartIndex)
    {
        Debug.Log("Born a Mushroom " + gridStartIndex);

        this.PlayerInstance = this.transform.GetChild(gridStartIndex).gameObject;

        // if (this.photonView.IsMine) this.PlayerInstance.SetActive(false);
        // else this.PlayerInstance.SetActive(true);
        this.PlayerInstance.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
