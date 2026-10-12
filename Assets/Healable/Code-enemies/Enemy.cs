// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;


[RequireComponent (typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public float health; // Usar PascalCase
    public float MaxHealth = 6f;
    public float damage = 10; // Usar PascalCase
    public float speed = .5f; // Usar PascalCase
    private bool _isDead; // Separar los grupos de acceso con una línea en blanco

    [SerializeField] private float _activationDelay = 0.75f;

    protected Transform player; // Usar _camelCase; Respetar el orden de acceso y static
    protected EnemySpawner spawner; // Usar _camelCase
    protected Rigidbody2D rb; // Usar _camelCase; Sin abreviaciones

    private float _activeTimer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        health = MaxHealth;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (_activeTimer < _activationDelay)
        {
            _activeTimer += Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        FollowPlayer();
    }

    protected virtual void FollowPlayer()
    {
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    public virtual void TakeDamage(float damageAmount) // Respetar el orden de acceso y static
    {
        Debug.Log($"[{name}] recibe {damageAmount} de daño, vida antes={health}", this);
        if (_isDead) return; // Agregar llaves al if

        health -= damageAmount;

        if (health <= 0)
        {
            _isDead = true;
            Die();
        }
    }

    public void SetSpawner(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
    }

    protected virtual void Die()
    {
        if (spawner != null)
        {
            spawner.EnemyDied();
        }

        Destroy(gameObject);
    }


}
