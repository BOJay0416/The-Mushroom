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

public class Networkplayer : MonoBehaviourPunCallbacks, IPunObservable

{

    public Transform head;

    public Transform lefthand;

    public Transform righthand;

    //public Transform Body;
    //public GameObject GOBody;

    public new PhotonView photonView;

    
    //private Transform Body;

    private Transform headRig;

    private Transform leftHandRig;

    private Transform rightHandRig;

    //private Transform BodyRig;

    public Material MatForTagger;
    public Material MatForTaggee;
    public Material MatForTaggerHand;
    public Material MatForTaggeeHand;

    public bool TaggerOrTaggee;
    public float TaggerSpawnTime = 3f;

    private int CurrScore;
    private float CurrTime;
    private bool Colddown = false;
    private float ColddownTime;

    private bool PreviousState;

    [Tooltip("指標- GameObject PlayerUI")]
    [SerializeField] public GameObject PlayerUIPrefab;

    //public Transform centereye;



    ///Start is called before the first frame update

    void Start()

    {
        
        //Body = GOBody.transform;

        photonView = GetComponent<PhotonView>();

        // XRRig rig = FindObjectOfType<XRRig>();
        XROrigin rig = FindObjectOfType<XROrigin>();

        headRig = rig.transform.Find("Camera Offset/Main Camera");

        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");

        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");

        //BodyRig = rig.transform.Find("Camera Offset/Main Camera/Capsule");

        CurrScore = 0;
        CurrTime = 0f;
        TaggerOrTaggee = false;
        PreviousState = TaggerOrTaggee;
        this.tag = "Taggee";

        //抓名字
        if (PlayerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUIPrefab);
            _uiGo.SendMessage("SetTarget", this, 
                SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("指標- GameObject PlayerUI 為空值", this);
        }

    }



    // Update is called once per frame

    void Update()

    {
        if(PreviousState != TaggerOrTaggee)
        {
            if(TaggerOrTaggee)
            {
                ChangeTagger();
            }
            else
            {
                ChangeTaggee();
            }
        }

        if(this.tag == "ReadyToTag")
        {
            ChangeTagger();
        }

        if(!TaggerOrTaggee)
        {
            CurrTime += Time.deltaTime;
            CurrScore = (int)CurrTime/1;
        }

        if(Colddown)
        {
            if(ColddownTime < TaggerSpawnTime) ColddownTime -= Time.deltaTime;
            else
            {
                Colddown = false;
            }
        }

        if (photonView.IsMine)
        {

            righthand.gameObject.SetActive(false);

            lefthand.gameObject.SetActive(false);

            head.gameObject.SetActive(false);

            //Body.gameObject.SetActive(false);



            MapPosition(head, headRig);

            MapPosition(lefthand, leftHandRig);

            MapPosition(righthand, rightHandRig);

            //MapPosition(Body, BodyRig);
        }

        PreviousState = TaggerOrTaggee;

    }



    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Debug.Log("Trigger!");

        if (other.CompareTag("Tagger"))
        {
            ChangeTagger();
        }
        else if(TaggerOrTaggee == true)
        {
            ChangeTaggee();
        }     
    }

    void OnCollisionEnter(Collision other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Debug.Log("Trigger!");

        if (other.gameObject.CompareTag("Tagger"))
        {
            ChangeTagger();
        }
        else if(TaggerOrTaggee == true)
        {
            ChangeTaggee();
        }     
    }

    void ChangeTagger()
    {
        Colddown = true;
        ColddownTime = 0;
        TaggerOrTaggee = true;
        this.tag = "Tagger";
        head.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = MatForTagger;
        head.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = MatForTagger;
        lefthand.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = MatForTaggerHand;
        righthand.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = MatForTaggerHand;

        head.transform.GetChild(0).gameObject.tag = "Tagger";
        head.transform.GetChild(1).gameObject.tag = "Tagger";
        lefthand.transform.GetChild(0).gameObject.tag = "Tagger";
        righthand.transform.GetChild(0).gameObject.tag = "Tagger";
        //Player.Instance.TaggerOrTaggee = true;
    }

    void ChangeTaggee()
    {
        TaggerOrTaggee = false;
        this.tag = "Taggee";
        head.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = MatForTaggee;
        head.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = MatForTaggee;
        lefthand.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = MatForTaggeeHand;
        righthand.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = MatForTaggeeHand;

        head.transform.GetChild(0).gameObject.tag = "Taggee";
        head.transform.GetChild(1).gameObject.tag = "Taggee";
        lefthand.transform.GetChild(0).gameObject.tag = "Taggee";
        righthand.transform.GetChild(0).gameObject.tag = "Taggee";
        //Player.Instance.TaggerOrTaggee = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 為玩家本人的狀態, 將 IsFiring 的狀態更新給其他玩家
            stream.SendNext(TaggerOrTaggee);
        }
        else
        {
            // 非為玩家本人的狀態, 單純接收更新的資料
            this.TaggerOrTaggee = (bool)stream.ReceiveNext();
        }
    }

} 
