using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
     public GameObject player; 
     public void RotateLeft(){
        transform.Rotate(new Vector3(0, -90, 0), Space.World);
     }
     public void RotateRight(){
        transform.Rotate(new Vector3(0, 90, 0), Space.World); 
     }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Align to player position
        transform.position = player.transform.position;

        // Rotate left
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * 100, Space.World);
        }
        
        // Rotate right
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * 100, Space.World);
        }
    }
}
