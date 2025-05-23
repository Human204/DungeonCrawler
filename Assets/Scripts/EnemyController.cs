using UnityEngine;
using Unity.Entities.UniversalDelegates;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using System.Numerics;
using UnityEngine.EventSystems;

public class EnemyController : MonoBehaviour
{
    

    [Header("Enemy Type")]
    public string enemyType;

    [Header("EnemyInfo")]
    public int hp;
    public int attackDamage;

    public bool active = false;

    public System.Random random = new System.Random();
    public void GotAttacked(int damage)
    {
        hp -= damage;
        // Debug.Log(hp);
        Debug.Log($"Enemy HP: {hp} | ID: {gameObject.GetInstanceID()}");
    }

    public void Activate()
    {
        active = true;
    }

    private bool CanMove(UnityEngine.Vector3 direction)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, 1f))
        {
            // if(!hit.collider.CompareTag("DungeonEnd")){
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.Log("Collision detected with: " + hit.collider.name);
                return false;
            }

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Collision detected with: " + hit.collider.name);

                GameObject[] playerDatas = GameObject.FindGameObjectsWithTag("Data");
                GameObject playerData = playerDatas[0];

                PlayerData controller = playerData.GetComponent<PlayerData>();

                Debug.Log("Health before");
                Debug.Log(controller.health);

                controller.health -= attackDamage;

                Debug.Log(controller.health);

                if (controller.health <= 0)
                {
                    Debug.Log("Death");
                    SceneManager.LoadScene("GameOver");
                }

                return false;
            }
        }
        return true;
    }

    public UnityEngine.Vector3 MoveZ(UnityEngine.Vector3 currentPos, float zDistance, float zDistanceAbs)
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3();

        pos.x = transform.position.x;
        pos.z = transform.position.z;
        pos.y = transform.position.y;

        pos.z += (zDistance / zDistanceAbs);

        return pos;
    }
    public UnityEngine.Vector3 MoveX(UnityEngine.Vector3 currentPos, float xDistance, float xDistanceAbs)
    {
        UnityEngine.Vector3 pos = new UnityEngine.Vector3();

        pos.x = transform.position.x;
        pos.z = transform.position.z;
        pos.y = transform.position.y;

        pos.x += (xDistance / xDistanceAbs);

        return pos;
    }
    
    public void MakeTurn()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject player = players[0];

        float xDistanceAbs = Mathf.Abs(player.transform.position.x - transform.position.x);
        float zDistanceAbs = Mathf.Abs(player.transform.position.z - transform.position.z);

        float xDistance = player.transform.position.x - transform.position.x;
        float zDistance = player.transform.position.z - transform.position.z;

        Debug.Log("enemyTurn");
        Debug.Log(xDistance);
        Debug.Log(zDistance);

        // false=move x
        bool moveZ = true;

        if (xDistanceAbs > zDistanceAbs) moveZ = false;

        UnityEngine.Vector3 pos = new UnityEngine.Vector3();
        UnityEngine.Vector3 moveDirection = new UnityEngine.Vector3();

        // pos.x = transform.position.x;
        // pos.z = transform.position.z;
        // pos.y = transform.position.y;

        if (moveZ == true)
        {
            // pos.z += (zDistance / zDistanceAbs);

            pos = MoveZ(transform.position, zDistance, zDistanceAbs);

            Debug.Log("Moving z");
            Debug.Log(pos);

            moveDirection = (pos - transform.position).normalized;

            if (!CanMove(moveDirection))
            {
                pos = MoveX(transform.position, xDistance, xDistanceAbs);
                Debug.Log("unable to move z");
                Debug.Log(pos);
            }
        }


        else
        {
            pos = MoveX(transform.position, xDistance, xDistanceAbs);

            Debug.Log("Moving x");
            Debug.Log(pos);

            if (!CanMove(moveDirection))
            {
                pos = MoveZ(transform.position, zDistance, zDistanceAbs);
                Debug.Log("unable to move x");
                Debug.Log(pos);
            }
        }
        moveDirection = (pos - transform.position).normalized;

        if (CanMove(moveDirection)) transform.position = pos;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start() called. Initial HP: " + hp + " | Enemy ID: " + gameObject.GetInstanceID());

        if (enemyType == "Heavy bandit")
        {
            hp = random.Next(30, 50);
            attackDamage = random.Next(10, 15);
        }
        else if (enemyType == "Light bandit")
        {
            hp = random.Next(20, 30);
            attackDamage = random.Next(5, 10);
        }
        else
        {
            hp = random.Next(10, 25);
            attackDamage = random.Next(10, 20);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
