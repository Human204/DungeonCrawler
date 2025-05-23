using UnityEngine;



public class lookAtCamera : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
        //                 Camera.main.transform.rotation * Vector3.up);
        transform.rotation = Quaternion.Euler(0, mainCamera.transform.rotation.eulerAngles.y, 0);
    }
}
