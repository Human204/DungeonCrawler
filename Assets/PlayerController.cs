
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BasicPlayerController : MonoBehaviour
{
    private Rigidbody rb;
    
    public GameObject cameraTarget;

    public float movementIntensity;

    public PlayerData PlayerData;

    public void MoveUp()
    {
        GameManager.Instance.TurnManager.Tick();
        if (rb == null) rb = GetComponent<Rigidbody>();

        // Debug.Log("MoveUp called");
        if (rb == null) Debug.LogWarning("Rigidbody not found!");

        Vector3 moveDirection = cameraTarget.transform.forward;
        if (CanMove(moveDirection))
            transform.position += moveDirection * movementIntensity;
    }

    public void MoveDown(){
        GameManager.Instance.TurnManager.Tick();
        if (rb == null) rb = GetComponent<Rigidbody>();
        Vector3 moveDirection = -cameraTarget.transform.forward;
        if(CanMove(moveDirection))
        transform.position += moveDirection * movementIntensity;
    }

    public void MoveLeft(){
        GameManager.Instance.TurnManager.Tick();
        if (rb == null) rb = GetComponent<Rigidbody>();

        Vector3 moveDirection = -cameraTarget.transform.right;
        if(CanMove(moveDirection))
        transform.position += moveDirection * movementIntensity;
    }
    public void MoveRight(){
        GameManager.Instance.TurnManager.Tick();
        if (rb == null) rb = GetComponent<Rigidbody>();
        Vector3 moveDirection = cameraTarget.transform.right;
        if(CanMove(moveDirection))
        transform.position += moveDirection * movementIntensity;
    }

    public void Attack()
    {
        GameManager.Instance.TurnManager.Tick();
        // add attack logic (check for collision as with canmove, -life for hit enemy collider object parent)
    }
    //check for walls and stuff
    private bool CanMove(Vector3 direction)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, movementIntensity))
        {
            Debug.Log("Hit object: " + hit.collider.name + ", Tag: " + hit.collider.tag);
            // if(!hit.collider.CompareTag("DungeonEnd")){
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.Log("Collision detected with: " + hit.collider.name);
                return false;
            }
            if (hit.collider.CompareTag("EnemyCollider"))
            {
                Debug.Log("hit an enemy");
                Debug.Log(hit.collider);

                GameObject[] playerDatas = GameObject.FindGameObjectsWithTag("Data");
                GameObject playerData = playerDatas[0];

                PlayerData controller = playerData.GetComponent<PlayerData>();

                // Debug.Log(hit.collider.GetComponentInParent<EnemyController>.hp);
                // attack logic (enemy -hp from inventory hp)
                int damage = controller.damage;
                Debug.Log(damage);

                // EnemyController enemyScript = hit.collider.transform.parent.GetComponent<EnemyController>();
                Debug.Log("pre attack");
                EnemyController enemyScript = hit.collider.GetComponentInParent<EnemyController>();
                Debug.Log(enemyScript.hp);
                if (enemyScript != null)
                {
                    // enemyScript.hp -= damage;
                    enemyScript.GotAttacked(damage);

                    Debug.Log("Enemy HP: " + enemyScript.hp);


                    if (enemyScript.hp < 0)
                    {
                        Destroy(enemyScript.gameObject);

                        PlayerData.Instance.score += 5;
                        return true;
                    }
                }
                else
                {
                    Debug.LogWarning("EnemyController not found on parent!");
                }



                return false;

            }
            if (hit.collider.CompareTag("DungeonEnd"))
            {
                EndDungeonLevel triggerScript = hit.collider.GetComponent<EndDungeonLevel>();
                if (triggerScript != null) triggerScript.EndLevel();

                PlayerData.Instance.score += 20;

                return true;
            }
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyController enemyScript = hit.collider.GetComponent<EnemyController>();
                if (enemyScript != null) enemyScript.Activate();
                return true;
            }

            if (hit.collider.CompareTag("HealthPotion"))
            {
                Destroy(hit.collider.gameObject);

                PlayerData.Instance.health += 25;

                return true;
            }
            if (hit.collider.CompareTag("AttackUpgrade"))
            {
                Destroy(hit.collider.gameObject);

                PlayerData.Instance.damage += 5;

                return true;
            }
        }
        return true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Controls controls = Object.FindFirstObjectByType<Controls>();
        // Controls controls = FindObjectOfType<Controls>();
        cameraControls camScript = GetComponentInChildren<cameraControls>();

        if (controls != null && camScript != null){
            controls.setPlayerCamera(this, camScript);
        }
    }
    
    void Update()
    {
        var ForwardDirection = cameraTarget.transform.forward;
        var RightDirection = cameraTarget.transform.right;

        rb.mass=(float)0.1;
        // rb.linearDamping=10;
        rb.linearDamping=0.1F;

        if (Input.GetKey(KeyCode.W)) 
        {
            rb.AddForce (ForwardDirection * movementIntensity);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce (-ForwardDirection * movementIntensity);
        }
        if (Input.GetKey(KeyCode.E))
        {
           rb.AddForce (RightDirection * movementIntensity);
        }
        if (Input.GetKey(KeyCode.Q))
        {
           rb.AddForce (-RightDirection * movementIntensity);
        }
    }
}