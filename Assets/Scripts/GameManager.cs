using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject playerDataPrefab;
    public Button play;

    // public Dungeon
    public BasicPlayerController PlayerController;
    public TurnManager TurnManager { get; private set;}

    private void Awake()
   {
       if (Instance != null)
       {
           Destroy(gameObject);
           return;
       }
      
       Instance = this;
   }

    void Start()
    {
        TurnManager = new TurnManager();

        DontDestroyOnLoad(gameObject);
        if (PlayerData.Instance == null)
        {
            Instantiate(playerDataPrefab);
        }
    }

    public void StartGame()
    {
        PlayerData.Instance.ResetData();
        Debug.Log("PLAY");
        SceneManager.LoadScene("DungeonLevel");


    }
}
 