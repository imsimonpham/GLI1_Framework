using UnityEngine;

public class RefreshmentArea : MonoBehaviour
{
  
    void OnTriggerStay(Collider other)
    {
        AI enemy = other.gameObject.GetComponent<AI>();
        if(enemy != null)
        {
            enemy.StayImmune(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        AI enemy = other.gameObject.GetComponent<AI>();
        if (enemy != null)
        {
            enemy.StayImmune(false);
        }
    }
}
