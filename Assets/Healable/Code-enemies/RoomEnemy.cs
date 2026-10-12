using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class RoomEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Min(1)] private int health = 100; // Usar _camelCase
    [SerializeField, Min(0)] private int damage = 10; // Usar _camelCase
    [SerializeField, Min(0f)] private float speed = 2f; // Usar _camelCase

    [Header("Attack")]
    [SerializeField, Min(0.1f)] private float attackRange = 0.8f; // Usar _camelCase
    [SerializeField, Min(0.1f)] private float attackCooldown = 1f; // Usar _camelCase
    [SerializeField] private Animator animator; // Usar _camelCase
    [SerializeField] private string attackTrigger = "Attack"; // Usar _camelCase

    [Header("Separation")]
    [SerializeField, Min(0.1f)] private float separationRadius = 0.7f; // Usar _camelCase
    [SerializeField, Min(0f)] private float separationStrength = 0.7f; // Usar _camelCase

    public event Action<RoomEnemy> Died;

    private readonly List<Vector2> path = new(); // Usar _camelCase
    private Rigidbody2D body; // Usar _camelCase
    private Transform player; // Usar _camelCase
    private EnemySpawnGrid spawnGrid; // Usar _camelCase
    private int pathIndex; // Usar _camelCase
    private float nextPathRefresh; // Usar _camelCase
    private float nextAttackTime; // Usar _camelCase
    private Vector2 movement; // Usar _camelCase
    private bool dead; // Usar _camelCase; Revisar: usar un verbo de estado (is, has o can)

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (GetComponent<Collider2D>() == null) // Agregar llaves al if
            gameObject.AddComponent<CircleCollider2D>();
        if (animator == null) // Agregar llaves al if
            animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Transform target, EnemySpawnGrid grid)
    {
        player = target;
        spawnGrid = grid;
    }

    private void Update() // Respetar el orden de los miembros
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

    private void FixedUpdate() // Respetar el orden de los miembros
    {
        if (!dead && movement.sqrMagnitude > 0.001f) // Agregar llaves al if
            body.MovePosition(body.position + movement * speed * Time.fixedDeltaTime);
    }

    private Vector2 GetSeparationForce()
    {
        Vector2 force = Vector2.zero;
        foreach (Collider2D nearby in Physics2D.OverlapCircleAll(body.position, separationRadius))
        {
            RoomEnemy other = nearby.GetComponentInParent<RoomEnemy>();
            if (other == null || other == this) // Agregar llaves al if
                continue;

            Vector2 offset = body.position - (Vector2)other.transform.position;
            float distance = offset.magnitude;
            if (distance > 0.001f) // Agregar llaves al if
                force += offset.normalized * (1f - distance / separationRadius);
        }
        return force;
    }

    private void StartAttackIfReady()
    {
        if (Time.time < nextAttackTime) // Agregar llaves al if
            return;

        nextAttackTime = Time.time + attackCooldown;
        if (animator != null && !string.IsNullOrEmpty(attackTrigger)) // Agregar llaves al if
            animator.SetTrigger(attackTrigger);
    }

    // Attack animation (to do)
    public void DealAttackHit() // Respetar el orden de acceso y static; Revisar si este código es necesario
    {
        // Player damage 
    }

    public void TakeDamage(int amount)
    {
        if (dead || amount <= 0) // Agregar llaves al if
            return;

        health -= amount;
        if (health <= 0) // Agregar llaves al if
            Die();
    }

    private void Die() // Revisar si se puede usar retorno temprano
    {
        if (dead) // Agregar llaves al if
            return;

        dead = true;
        Died?.Invoke(this); // Invocar el evento desde un método On...
        Destroy(gameObject);
    }
}
