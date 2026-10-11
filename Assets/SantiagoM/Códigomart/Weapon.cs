// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Enemy Enemy; // Usar _camelCase
    private PlayerController PC; // Usar _camelCase; Sin abreviaciones
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            //enemy.TakeDamage(PC.PlayerDamage);
        }
    }
}
