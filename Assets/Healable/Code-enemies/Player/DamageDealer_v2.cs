using UnityEngine;

public class DamageDealer_v2 : MonoBehaviour // Usar PascalCase | Revisar: otra copia de DamageDealer_v2 declara el mismo tipo
{
    public float Damage = 3f;
    [SerializeField] private bool _destroyOnHit; // Revisar: usar un verbo de estado (is, has o can) | Separar los grupos de acceso con una línea en blanco

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[DamageDealer en {name}] toca '{other.name}'", this);

        Enemy_v2 enemy = other.GetComponentInParent<Enemy_v2>();
        if (enemy == null) { return; }

        Debug.Log($"[DamageDealer] {Damage} de da�o a {enemy.name}", this);
        enemy.TakeDamage(Damage);

        if (_destroyOnHit) { Destroy(gameObject); }
    }
}
