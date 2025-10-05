using UnityEngine;
 
public class OutroEndingCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            
        }
    }
}