using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
 using TMPro;
public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public void TryAgain()
    {
        Debug.Log("TryAgain");
        SceneManager.LoadScene("menuTest");
    }

    public void Exit()
    {
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
        scoreText.text="Score: " + PlayerData.Instance.score.ToString();
    }
    void UpdateHUD()
    {
        scoreText.text="Score: " + PlayerData.Instance.score.ToString();
    }
}
