using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void Play(){
        Debug.Log("Play");
        SceneManager.LoadScene("DungeonLevel");
    }

    public void Exit(){
        Debug.Log("Exit");
        Application.Quit();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
