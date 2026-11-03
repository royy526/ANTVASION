using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float Damage = 3f;
    [SerializeField] private bool _destroyOnHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[DamageDealer en {name}] toca '{other.name}'", this);

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        Debug.Log($"[DamageDealer] {Damage} de daño a {enemy.name}", this);
        enemy.TakeDamage(Damage);

        if (_destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}