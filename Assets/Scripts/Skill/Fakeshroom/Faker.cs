using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faker : MonoBehaviour
{
    public Rigidbody rb;

    private Vector3 jpdown;

    public GameObject Fakeshroom;

    bool hori = false;

    bool vert = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            hori = false;
            jpdown.z = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            hori = false;
            jpdown.z = -1;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            hori = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            vert = false;
            jpdown.x = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            vert = false;
            jpdown.x = -1;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow)|| Input.GetKeyUp(KeyCode.RightArrow))
        {
            vert = true;
        }

        if(hori)
        {
            if(jpdown.z == 0) hori = false;
            else if(jpdown.z > 0) jpdown.z -= 0.05f; 
            else if(jpdown.z < 0) jpdown.z += 0.05f;
        }

        if(vert)
        {
            if(jpdown.x == 0) vert = false;
            else if(jpdown.x > 0) jpdown.x -= 0.05f;
            else if(jpdown.x < 0) jpdown.x += 0.05f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Born");
            Instantiate(Fakeshroom, transform.position + new Vector3(0, 0.3f, 0), Quaternion.Euler(new Vector3(0, transform.rotation.y , 0)));
        }

        jpdown.y = rb.velocity.y;
        rb.velocity = jpdown;
    }
}
