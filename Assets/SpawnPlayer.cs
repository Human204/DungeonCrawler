// using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnPlayer : MonoBehaviour
{

    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // player.transform.position=transform.position;
        Vector3 position_vec=new Vector3(transform.position.x,1,transform.position.z);
        Instantiate(player,position_vec,Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
