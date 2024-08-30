using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Tooltip("玩家名稱")]
    [SerializeField] private Text playerNameText;

    [Tooltip("玩家身分")]
    [SerializeField] private Slider playerTaggerOrTaggee;

    private Networkplayer target;
    

    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    void Update()
    {
        if(playerTaggerOrTaggee != null)
        {
            if(target.TaggerOrTaggee) playerTaggerOrTaggee.value = 1f;
            else playerTaggerOrTaggee.value = 0f;
        }
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void SetTarget(Networkplayer _target)
    {
        if (_target == null)
        {
            Debug.LogError("傳入的 PlayerManager instance 為空值", this);
            return;
        }
    
        target = _target;
        if (playerNameText != null)
        {
            playerNameText.text = target.photonView.Owner.NickName;
        } 

    }
}
