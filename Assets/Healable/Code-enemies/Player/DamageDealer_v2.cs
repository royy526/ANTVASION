using UnityEngine;

public class DamageDealer_v2 : MonoBehaviour
{
    public float Damage = 3f;
    [SerializeField] private bool _destroyOnHit;

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
