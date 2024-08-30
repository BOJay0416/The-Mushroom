using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : MonoBehaviour
{
    public GameObject Skin;

    public GameObject DestroyVFX;

    public float timeCount = 0f;

    bool visible = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("s"))
        {
            if(visible) Instantiate(DestroyVFX, transform.position + new Vector3(0, 0.15f, 0), Quaternion.Euler(new Vector3(90, 10, 0)));
            Skin.SetActive(false);
            timeCount = 0f;
            visible = false;
        }
        
        if(!visible)
        {
            if(timeCount >= 5)
            {
                Skin.SetActive(true);
                Instantiate(DestroyVFX, transform.position + new Vector3(0, 0.15f, 0), Quaternion.Euler(new Vector3(90, 10, 0)));
                visible = true;
            }
            else
            {
                timeCount += Time.deltaTime;
            }
        }
    }
}
