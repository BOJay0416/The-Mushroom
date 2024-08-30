using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun;

using Photon.Realtime;

using UnityEngine.XR;

using UnityEngine.Networking;

using UnityEngine.SceneManagement;

using Unity.XR.CoreUtils;

public class LoginShroomPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("香菇本體")]
    public Transform mushroom;

    [Tooltip("香菇本體")]
    public Transform mushroomBody;

    private new PhotonView photonView;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    private AudioSource myAudioSource;

    private Vector3 Offset = new (0f,0.2f,0f);

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        // XRRig rig = FindObjectOfType<XRRig>();
        XROrigin rig = FindObjectOfType<XROrigin>();

        headRig = rig.transform.Find("Camera Offset/Main Camera");

        myAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (this.transform.parent.gameObject.GetComponent<LoginPlayerControl>().photonView.IsMine)
        {
            mushroomBody.gameObject.SetActive(false);
            MapPosition(mushroom, headRig);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position - Offset;
        target.rotation = rigTransform.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }

}
