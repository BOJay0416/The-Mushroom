using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosionshroom : MonoBehaviour
{
    public Collider[] Box;

    public float force;

    public float radius;

    void Update () 
    {
        Box = Physics.OverlapSphere(this.transform.position, 10f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Bang!");
        foreach (Collider hit in Box)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, this.transform.position, radius);
            }
        }
        Destroy(gameObject);
    }
}
