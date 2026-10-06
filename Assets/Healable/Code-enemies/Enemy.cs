using UnityEngine;


[RequireComponent (typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public float health;
    public float MaxHealth = 6f;
    public float damage = 10;
    public float speed = .5f;
    private bool _isDead;

    [SerializeField] private float _activationDelay = 0.75f;

    protected Transform player;
    protected EnemySpawner spawner;
    protected Rigidbody2D rb;

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

    public virtual void TakeDamage(float damageAmount)
    {
        Debug.Log($"[{name}] recibe {damageAmount} de daño, vida antes={health}", this);
        if (_isDead) return;

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
