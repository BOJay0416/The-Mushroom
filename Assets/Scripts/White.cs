using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class White : MonoBehaviour
{
    public float currTime = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if(currTime <= 2)
        {
            Color Imagecolor = new Color();
            Imagecolor.r = 1f;
            Imagecolor.g = 1f;
            Imagecolor.b = 1f;
            Imagecolor.a = (225 - ( 225 / 2 * (currTime))) / 225;

            GetComponent<Image>().color = Imagecolor;
        }
        else if(currTime > 2)
        {
            gameObject.SetActive(false);
        }
    }
}
