using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPose2 : MonoBehaviour
{
    public int grabbing = 0;
    public GameObject GrabZone2;
    [SerializeField] private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GrabZone2.GetComponent<Renderer>().enabled = false;
    }

    void Update()
    {
        grabbing = ESP32_com.touch;

        if (grabbing == 1) {

            GrabZone2.GetComponent<Renderer>().enabled = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        else{
            GrabZone2.GetComponent<Renderer>().enabled = false;
            rb.constraints = RigidbodyConstraints.None;
        }

    }
}
