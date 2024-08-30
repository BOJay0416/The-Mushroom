using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enterpassword : MonoBehaviour
{
    public Text passwordtext;
    public string password;

    void OnTriggerEnter(Collider other)
    {
        password += other.tag;
        passwordtext.text = password;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(0, 0, -1 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(0, 0, 1 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(1 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(-1 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.Translate(0, -1 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.Translate(0, 1 * Time.deltaTime, 0);
        }
    }

    //public void press0()
    //{
    //    password += 0;
    //    passwordtext.text = password;
    //}

    //public void press1()
    //{
    //    password += 1;
    //    passwordtext.text = password;
    //}

    //public void press2()
    //{
    //    password += 2;
    //    passwordtext.text = password;
    //}

    //public void press3()
    //{
    //    password += 3;
    //    passwordtext.text = password;
    //}

    //public void press4()
    //{
    //    password += 4;
    //    passwordtext.text = password;
    //}

    //public void press5()
    //{
    //    password += 5;
    //    passwordtext.text = password;
    //}

    //public void press6()
    //{
    //    password += 6;
    //    passwordtext.text = password;
    //}

    //public void press7()
    //{
    //    password += 7;
    //    passwordtext.text = password;
    //}

    //public void press8()
    //{
    //    password += 8;
    //    passwordtext.text = password;
    //}

    //public void press9()
    //{
    //    password += 9;
    //    passwordtext.text = password;
    //}
}
