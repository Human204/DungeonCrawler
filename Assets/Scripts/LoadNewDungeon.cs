using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class LoadNewDungeon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Wait());
        SceneManager.LoadScene("DungeonLevel");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Wait(){
        yield return new WaitForSeconds(5);
    }
}
