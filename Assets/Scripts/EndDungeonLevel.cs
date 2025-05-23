using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDungeonLevel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void EndLevel(){
        Debug.Log("ended level");
        SceneManager.LoadScene("loadingScene");
    }
    void Start(){   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
