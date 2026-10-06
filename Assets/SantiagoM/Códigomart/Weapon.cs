using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Enemy Enemy;
    private PlayerController PC;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            //enemy.TakeDamage(PC.PlayerDamage);
        }
    }
}
