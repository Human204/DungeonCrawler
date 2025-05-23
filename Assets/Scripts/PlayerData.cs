using Unity.Notifications.Android;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public int health = 100;
    public string inventory = new string("none");
    public int damage;
    public int score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetData()
    {
        health = 100;
        inventory = "sword";
        damage = 5;
        score = 0;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
