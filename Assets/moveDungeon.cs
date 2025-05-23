using Unity.VisualScripting;
using UnityEngine;

public class moveDungeon : MonoBehaviour
{
    float plane_x;
    float plane_z;
    // public GameObject dungeon; 
    public Transform plane;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane_x=plane.GetComponent<MeshRenderer>().bounds.size.x;
        plane_z=plane.GetComponent<MeshRenderer>().bounds.size.z;
        plane_x /= 2;
        plane_z /= 2;
        transform.position=new Vector3(-25,0,-35);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
