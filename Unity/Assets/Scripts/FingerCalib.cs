using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Linq;

public class FingerCalib : MonoBehaviour
{

    public static float Cal_index, Cal_middle, Cal_ring, Cal_pinky, Cal_thumb;
    public float Mul_index, Mul_middle, Mul_ring, Mul_pinky, Mul_thumb;
    private static float Min_index=0, Min_middle=0, Min_ring=0, Min_pinky=0, Min_thumb=0;
    private static float Max_index=50, Max_middle=50, Max_ring=50, Max_pinky=50, Max_thumb=50;
    private float _thumb, _index, _middle, _ring, _pinky;
    public Transform index1, index2, index3, middle1, middle2, middle3, ring1, ring2, ring3, pinky0, pinky1, pinky2, pinky3, thumb1, thumb2, thumb3;

    public static bool isButtonPressed;
    
    private float timer = 10.5f;
    private float sec = 1.0f;

    // Lists to store a set of values to average over time
    private List<float> thumbValues = new List<float>();
    private List<float> indexValues = new List<float>();
    private List<float> middleValues = new List<float>();
    private List<float> ringValues = new List<float>();
    private List<float> pinkyValues = new List<float>();


    void Update()
    {
        float _thumb = ESP32_com.thumb;
        float _index = ESP32_com.index;
        float _middle = ESP32_com.middle;
        float _ring = ESP32_com.ring;
        float _pinky = ESP32_com.pinky;

        index1.transform.localRotation = Quaternion.Euler(-74, 106, 61-(Cal_index*Mul_index));
        index2.transform.localRotation = Quaternion.Euler(0, 0, index1.transform.localRotation.z - (Cal_index*Mul_index));
        index3.transform.localRotation = Quaternion.Euler(0, 0, index2.transform.localRotation.z - (Cal_index*Mul_index));
                        
        middle1.transform.localRotation = Quaternion.Euler(-80, 18, 151-(Cal_middle*Mul_middle));
        middle2.transform.localRotation = Quaternion.Euler(0, 0, middle1.transform.localRotation.z - (Cal_middle*Mul_middle));
        middle3.transform.localRotation = Quaternion.Euler(0, 0, middle2.transform.localRotation.z - (Cal_middle*Mul_middle));

        ring1.transform.localRotation = Quaternion.Euler(-69, -21, -169-(Cal_ring*Mul_ring));
        ring2.transform.localRotation = Quaternion.Euler(0, 0, ring1.transform.localRotation.z - (Cal_ring*Mul_ring));
        ring3.transform.localRotation = Quaternion.Euler(0, 0, ring2.transform.localRotation.z - (Cal_ring*Mul_ring));

        pinky0.transform.localRotation = Quaternion.Euler(-50, -26, 26);
        pinky1.transform.localRotation = Quaternion.Euler(-1, -2, -175-(Cal_pinky*Mul_pinky));
        pinky2.transform.localRotation = Quaternion.Euler(0, 0, pinky1.transform.localRotation.z - (Cal_pinky*Mul_pinky));
        pinky3.transform.localRotation = Quaternion.Euler(0, 0, pinky2.transform.localRotation.z );
                        
        thumb1.transform.localRotation = Quaternion.Euler(9, 156, 27-(Cal_thumb*Mul_thumb));
        thumb2.transform.localRotation = Quaternion.Euler(0, 0, thumb1.transform.localRotation.z - (Cal_thumb*Mul_thumb));
        thumb3.transform.localRotation = Quaternion.Euler(0, 0, thumb2.transform.localRotation.z - (Cal_thumb*Mul_thumb));


        if(isButtonPressed){
            timer -= sec * Time.deltaTime;
            //Debug.Log("Time: " + timer);
            if (timer < 0.0f){
                timer = 0.0f;
            }

            if(timer > 5.5f) {   
                Debug.Log("Open your hand");
                // Gather min values
                // Min_index = _index;
                // Min_middle = _middle;
                // Min_ring = _ring;
                // Min_pinky = _pinky;
                // Min_thumb = _thumb;
                if(timer < 7.0f)
                {
                    thumbValues.Add(_thumb);
                    indexValues.Add(_index);
                    middleValues.Add(_middle);
                    ringValues.Add(_ring);
                    pinkyValues.Add(_pinky);
                }
            }

            else if(timer > 0.5f) {
                Debug.Log("Close your hand");
                // Gather max values  
                if (timer < 2.0f)
                {
                    thumbValues.Add(_thumb);
                    indexValues.Add(_index);
                    middleValues.Add(_middle);
                    ringValues.Add(_ring);
                    pinkyValues.Add(_pinky);
                }
                else
                {
                    // Average over sored values to satore calibration values
                    if (thumbValues.Count > 0)
                    {
                        Min_thumb = thumbValues.Average();
                        Min_index = indexValues.Average();
                        Min_middle = middleValues.Average();
                        Min_ring = ringValues.Average();
                        Min_pinky = pinkyValues.Average();
                    }
                    // Clear the lists for future calibrations
                    thumbValues.Clear();
                    indexValues.Clear();
                    middleValues.Clear();
                    ringValues.Clear();
                    pinkyValues.Clear();
                }
            } 
                
            else if(timer > 0.0f){
                Debug.Log("Calibration complete!"); 
                isButtonPressed = false;
                Max_thumb = thumbValues.Average();
                Max_index = indexValues.Average();
                Max_middle = middleValues.Average();
                Max_ring = ringValues.Average();
                Max_pinky = pinkyValues.Average();
                Debug.Log("Min Thumb: " + Min_thumb + ", Max Thumb: " + Max_thumb);
                Debug.Log("Min Index: " + Min_index + ", Max Index: " + Max_index);
                Debug.Log("Min Middle: " + Min_middle + ", Max Middle: " + Max_middle);
                Debug.Log("Min Ring: " + Min_ring + ", Max Ring: " + Max_ring);
                Debug.Log("Min Pinky: " + Min_pinky + ", Max Pinky: " + Max_pinky);
            }
        }



        // Set up mul values
        Mul_index = Max_index - Min_index > 0 ? 50 / (Max_index - Min_index) : - 50 / (Max_index - Min_index);
        Mul_middle = Max_middle - Min_middle > 0 ? 50 / (Max_middle - Min_middle) : -50 / (Max_middle - Min_middle);
        Mul_ring = Max_ring - Min_ring > 0 ? 50 / (Max_ring - Min_ring) : -50 / (Max_ring - Min_ring);
        Mul_pinky = Max_pinky - Min_pinky > 0 ? 50 / (Max_pinky - Min_pinky) : -50 / (Max_pinky - Min_pinky);
        Mul_thumb = Max_thumb - Min_thumb > 0 ? 50 / (Max_thumb - Min_thumb) : -50 / (Max_thumb - Min_thumb);


        Cal_index = Mathf.Clamp(_index - Min_index, 0, Max_index-Min_index); 
        Cal_middle = Mathf.Clamp(_middle - Min_middle, 0, Max_middle - Min_middle);
        Cal_ring = Mathf.Clamp(_ring - Min_ring, 0, Max_ring - Min_ring);
        Cal_pinky = Mathf.Clamp(_pinky - Min_pinky, 0, Max_pinky - Min_pinky);
        Cal_thumb = Mathf.Clamp(_thumb - Min_thumb, 0, Max_thumb - Min_thumb);

    }

    public void ButtonPressed()
    {
        isButtonPressed = true;
        timer = 10.5f;
    }
}
