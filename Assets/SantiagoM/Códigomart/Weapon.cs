// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Enemy _enemy;
    private PlayerController _playerController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            //enemy.TakeDamage(PC.PlayerDamage);
        }
    }
}
