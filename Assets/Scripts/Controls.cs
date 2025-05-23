// using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Controls : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created 

    public Button moveUpButton;
    public Button moveDownButton;
    public Button moveLeftButton;
    public Button moveRightButton;

    public Button rotateLeftButton;
    public Button rotateRightButton;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI weaponText;

    public void setPlayerCamera(BasicPlayerController playerScript, cameraControls camScript){
        BasicPlayerController player=playerScript;
        cameraControls camera =camScript;

        moveUpButton.onClick.AddListener(player.MoveUp);
        moveDownButton.onClick.AddListener(player.MoveDown);
        moveLeftButton.onClick.AddListener(player.MoveLeft);
        moveRightButton.onClick.AddListener(player.MoveRight);
        rotateLeftButton.onClick.AddListener(camera.RotateLeft);
        rotateRightButton.onClick.AddListener(camera.RotateRight);
    }

    // }
    void Start()
    {
        UpdateHUD();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHUD();
    }

    public void UpdateHUD(){
         healthText.text = "HP: " + PlayerData.Instance.health.ToString();
    }
}
