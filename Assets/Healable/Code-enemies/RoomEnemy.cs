using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class RoomEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Min(1)] private int health = 100;
    [SerializeField, Min(0)] private int damage = 10;
    [SerializeField, Min(0f)] private float speed = 2f;

    [Header("Attack")]
    [SerializeField, Min(0.1f)] private float attackRange = 0.8f;
    [SerializeField, Min(0.1f)] private float attackCooldown = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";

    [Header("Separation")]
    [SerializeField, Min(0.1f)] private float separationRadius = 0.7f;
    [SerializeField, Min(0f)] private float separationStrength = 0.7f;

    public event Action<RoomEnemy> Died;

    private readonly List<Vector2> path = new();
    private Rigidbody2D body;
    private Transform player;
    private EnemySpawnGrid spawnGrid;
    private int pathIndex;
    private float nextPathRefresh;
    private float nextAttackTime;
    private Vector2 movement;
    private bool dead;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (GetComponent<Collider2D>() == null)
            gameObject.AddComponent<CircleCollider2D>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Transform target, EnemySpawnGrid grid)
    {
        player = target;
        spawnGrid = grid;
    }

    private void Update()
    {
        if (dead || player == null)
        {
            movement = Vector2.zero;
            return;
        }

        Vector2 toPlayer = (Vector2)player.position - body.position;
        if (toPlayer.magnitude <= attackRange)
        {
            movement = Vector2.zero;
            StartAttackIfReady();
            return;
        }

        if (Time.time >= nextPathRefresh)
        {
            nextPathRefresh = Time.time + 0.25f;
            spawnGrid.TryGetPath(body.position, player.position, path);
            pathIndex = 0;
        }

        while (pathIndex < path.Count && Vector2.Distance(body.position, path[pathIndex]) < 0.15f)
            pathIndex++;

        Vector2 desiredDirection = pathIndex < path.Count
            ? (path[pathIndex] - body.position).normalized
            : toPlayer.normalized;
        movement = (desiredDirection + GetSeparationForce() * separationStrength).normalized;
    }

    private void FixedUpdate()
    {
        if (!dead && movement.sqrMagnitude > 0.001f)
            body.MovePosition(body.position + movement * speed * Time.fixedDeltaTime);
    }

    private Vector2 GetSeparationForce()
    {
        Vector2 force = Vector2.zero;
        foreach (Collider2D nearby in Physics2D.OverlapCircleAll(body.position, separationRadius))
        {
            RoomEnemy other = nearby.GetComponentInParent<RoomEnemy>();
            if (other == null || other == this)
                continue;

            Vector2 offset = body.position - (Vector2)other.transform.position;
            float distance = offset.magnitude;
            if (distance > 0.001f)
                force += offset.normalized * (1f - distance / separationRadius);
        }
        return force;
    }

    private void StartAttackIfReady()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;
        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);
    }

    // Attack animation (to do)
    public void DealAttackHit()
    {
        // Player damage 
    }

    public void TakeDamage(int amount)
    {
        if (dead || amount <= 0)
            return;

        health -= amount;
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        if (dead)
            return;

        dead = true;
        Died?.Invoke(this);
        Destroy(gameObject);
    }
}
