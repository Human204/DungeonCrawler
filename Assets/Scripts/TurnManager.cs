using UnityEngine;

public class TurnManager
{
  private int m_TurnCount;

   public TurnManager()
   {
       m_TurnCount = 1;
   }

    public void Tick()
    {
        m_TurnCount += 1;
        Debug.Log("Current turn count : " + m_TurnCount);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController controller = enemy.GetComponent<EnemyController>();

            if (controller != null && controller.active==true)
            {
                controller.MakeTurn();
            }
        }
   }
}