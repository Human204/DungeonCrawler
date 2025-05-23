using UnityEngine;

public class PegCollisionDetector : MonoBehaviour
{
    public GameObject sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation=sprite.transform.rotation;
    }
}
