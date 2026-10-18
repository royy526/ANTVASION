using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public float Health;
    public float MaxHealth = 6f;
    public float Damage = 10;
    public float Speed = .5f;

    private bool _isDead;

    [SerializeField] private float _activationDelay = 0.75f;

    protected Transform _player;
    protected EnemySpawner _spawner;
    protected Rigidbody2D _rigidBody;

    private float _activeTimer;

    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        Health = MaxHealth;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (_player == null)
        {
            return;
        }

        if (_activeTimer < _activationDelay)
        {
            _activeTimer += Time.fixedDeltaTime;
            _rigidBody.linearVelocity = Vector2.zero;
            return;
        }

        FollowPlayer();
    }

    protected virtual void FollowPlayer()
    {
        Vector2 direction = ((Vector2)_player.position - _rigidBody.position).normalized;
        _rigidBody.linearVelocity = direction * Speed;
    }

    public virtual void TakeDamage(float damageAmount)
    {
        Debug.Log($"[{name}] recibe {damageAmount} de daño, vida antes={Health}", this);
        if (_isDead)
        {
            return;
        }

        Health -= damageAmount;

        if (Health <= 0)
        {
            _isDead = true;
            Die();
        }
    }

    public void SetSpawner(EnemySpawner enemySpawner)
    {
        _spawner = enemySpawner;
    }

    protected virtual void Die()
    {
        if (_spawner != null)
        {
            _spawner.EnemyDied();
        }

        Destroy(gameObject);
    }


}

