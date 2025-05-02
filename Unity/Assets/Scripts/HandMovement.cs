using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMovement : MonoBehaviour
{
    private float _qx, _qy, _qz, _qw, _height;
    public List<int> rotation_axis = new List<int> { 0, 2, 3, 1 };
    public List<int> rotation_multiplier = new List<int> { 1, -1, -1 };
    public List<int> rotation_displacement = new List<int> { 180, 0, 0 };
    private Rigidbody rb;

    private float speed = 0.2f;
    private float maxVelocity = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //ESP Datas
        float _qx = ESP32_com.qx;
        float _qy = ESP32_com.qy;
        float _qz = ESP32_com.qz;
        float _qw = ESP32_com.qw;
        float _height = ESP32_com.height;

        //Rotation
        Quaternion rawRotation = new Quaternion(-_qx, -_qy, _qz, _qw);

        // 2. Remap axes using rotation_axis
        Quaternion remapped = new Quaternion(
            rotation_axis[0] == 0 ? rawRotation.x : rotation_axis[0] == 1 ? rawRotation.y : rotation_axis[0] == 2 ? rawRotation.z : rawRotation.w,
            rotation_axis[1] == 0 ? rawRotation.x : rotation_axis[1] == 1 ? rawRotation.y : rotation_axis[1] == 2 ? rawRotation.z : rawRotation.w,
            rotation_axis[2] == 0 ? rawRotation.x : rotation_axis[2] == 1 ? rawRotation.y : rotation_axis[2] == 2 ? rawRotation.z : rawRotation.w,
            rotation_axis[3] == 0 ? rawRotation.x : rotation_axis[3] == 1 ? rawRotation.y : rotation_axis[3] == 2 ? rawRotation.z : rawRotation.w
        );

        // 3. Apply multipliers (inverting selected axes)
        Quaternion scaled = new Quaternion(
            remapped.x * rotation_multiplier[0],
            remapped.y * rotation_multiplier[1],
            remapped.z * rotation_multiplier[2],
            remapped.w // leave W as-is to preserve rotation direction
        );

        // 4. Apply displacement as additional quaternion rotation
        // Convert rotation_displacement (in degrees) to a Quaternion
        Quaternion displacement = Quaternion.Euler(
            rotation_displacement[0],
            rotation_displacement[1],
            rotation_displacement[2]
        );

        // 5. Combine displacement and sensor rotation
        transform.rotation = displacement * scaled;


        //Movement
        if (!Input.GetKey(KeyCode.Mouse1)){ // For Camera Movement
            float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            float vertical = -Input.GetAxis("Vertical") * Time.deltaTime * speed;
            float updown = Input.GetAxis("updown") * Time.deltaTime * speed;

            transform.position += new Vector3(vertical, updown, horizontal);
        }

        /*if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }*/
    }
}
