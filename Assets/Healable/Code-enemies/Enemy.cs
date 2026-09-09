using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public int damage = 10;
    public float speed = 2f;

    protected Transform player;
    protected EnemySpawner spawner;

    protected virtual void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    protected virtual void Update()
    {
        if (player != null)
        {
            FollowPlayer();
        }
    }

    protected virtual void FollowPlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
    }

    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
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
