using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;


public class ESP32_com : MonoBehaviour
{
    //Insert your IP here. It MUST be the same as the one in the python server
    public string IP = "100.65.101.157";
    //Insert the port you want to use. It MUST be the same as the one in the python server
    public int port = 5050;
    Socket client;

    public GameObject hand;
    public string strReceived; 
    private string[] strData = new string[11];
    public string[] strData_received = new string[11];
    public static float qw, qx, qy, qz, height, thumb, index, middle, ring, pinky;
    public static int touch;
    //public Transform hand, index1, index2, index3, middle1, middle2, middle3, ring1, ring2, ring3, pinky0, pinky1, pinky2, pinky3, thumb1, thumb2, thumb3;

    void Start()
    {
        //Create the client socket
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //This is the IP address of the computer running Python
        IPAddress ip = IPAddress.Parse(IP);
        //This is the port that Python is listening on
        IPEndPoint localEndPoint = new IPEndPoint(ip, port);
        //Connect the socket to the server
        client.Connect(localEndPoint);
        //Check if the socket is connected
        if (client.Connected)
        {
            Debug.Log("Socket connected to " + client.RemoteEndPoint.ToString());
        }
        else
        {
            Debug.Log("Socket not connected");
        }

    }

    //Function to receive a message from python
    string receive_message()
    {
        //Create a byte array to store the message.
        byte[] msg = new byte[1024];
        //Receive the message
        int bytesRec = client.Receive(msg);
        //Convert the byte array to a string
        string message = System.Text.Encoding.ASCII.GetString(msg, 0, bytesRec);
        //Return the message
        return message;
    }

    void Update()
    {
        
        strReceived = receive_message();
        strData = strReceived.Split(','); 

        if (strData.Length == 11) // Check if the length of strData is 8
        {
            strData_received = strData; // Assign strData to strData_received

            qx = float.Parse(strData_received[0]); 
            qy = float.Parse(strData_received[1]); 
            qz = float.Parse(strData_received[2]);
            qw = float.Parse(strData_received[3]);
            height = float.Parse(strData_received[4]);

            // hand.transform.rotation = new Quaternion(-qy, -qz, qx, qw);
            // hand.transform.position = new Vector3(0, height, 0);

            thumb = float.Parse(strData_received[5]);
            index = float.Parse(strData_received[6]);
            middle = float.Parse(strData_received[7]);
            ring = float.Parse(strData_received[8]);
            pinky = float.Parse(strData_received[9]);

            touch = int.Parse(strData_received[10]);

        }

           
               
    }
}
