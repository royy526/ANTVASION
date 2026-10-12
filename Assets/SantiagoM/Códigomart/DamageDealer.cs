// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float Damage = 3f;
    [SerializeField] private bool _destroyOnHit; // Revisar: usar un verbo de estado (is, has o can); Separar los grupos de acceso con una línea en blanco

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[DamageDealer en {name}] toca '{other.name}'", this);

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return; // Agregar llaves al if

        Debug.Log($"[DamageDealer] {Damage} de daño a {enemy.name}", this);
        enemy.TakeDamage(Damage);

        if (_destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}