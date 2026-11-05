using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class RoomEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Min(1)] private int _health = 100;
    [SerializeField, Min(0)] private int _damage = 10;
    [SerializeField, Min(0f)] private float _speed = 2f;

    [Header("Attack")]
    [SerializeField, Min(0.1f)] private float _attackRange = 0.8f;
    [SerializeField, Min(0.1f)] private float _attackCooldown = 1f;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _attackTrigger = "Attack";

    [Header("Separation")]
    [SerializeField, Min(0.1f)] private float _separationRadius = 0.7f;
    [SerializeField, Min(0f)] private float _separationStrength = 0.7f;

    public event Action<RoomEnemy> Died;

    private readonly List<Vector2> _path = new();
    private Rigidbody2D _body;
    private Transform _player;
    private EnemySpawnGrid _spawnGrid;
    private int _pathIndex;
    private float _nextPathRefresh;
    private float _nextAttackTime;
    private Vector2 _movement;
    private bool _isDead;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _body.gravityScale = 0f;
        _body.freezeRotation = true;
        _body.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>();
        }
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }

    public void Initialize(Transform target, EnemySpawnGrid grid)
    {
        _player = target;
        _spawnGrid = grid;
    }

    private void Update()
    {
        if (_isDead || _player == null)
        {
            _movement = Vector2.zero;
            {
                return;
            }
        }

        Vector2 toPlayer = (Vector2)_player.position - _body.position;
        if (toPlayer.magnitude <= _attackRange)
        {
            _movement = Vector2.zero;
            StartAttackIfReady();
            return;
        }

        if (Time.time >= _nextPathRefresh)
        {
            _nextPathRefresh = Time.time + 0.25f;
            _spawnGrid.TryGetPath(_body.position, _player.position, _path);
            _pathIndex = 0;
        }

        while (_pathIndex < _path.Count && Vector2.Distance(_body.position, _path[_pathIndex]) < 0.15f)
            _pathIndex++;

        Vector2 desiredDirection = _pathIndex < _path.Count ? (_path[_pathIndex] - _body.position).normalized : toPlayer.normalized;
        _movement = (desiredDirection + GetSeparationForce() * _separationStrength).normalized;
    }

    private void FixedUpdate()
    {
        if (!_isDead && _movement.sqrMagnitude > 0.001f)
        {
            _body.MovePosition(_body.position + _movement * _speed * Time.fixedDeltaTime);
        }
    }

    private Vector2 GetSeparationForce()
    {
        Vector2 force = Vector2.zero;
        foreach (Collider2D nearby in Physics2D.OverlapCircleAll(_body.position, _separationRadius))
        {
            RoomEnemy other = nearby.GetComponentInParent<RoomEnemy>();
            if (other == null || other == this)
            {
                continue;
            }

            Vector2 offset = _body.position - (Vector2)other.transform.position;
            float distance = offset.magnitude;
            if (distance > 0.001f)
            {
                force += offset.normalized * (1f - distance / _separationRadius);
            }
        }
        return force;
    }

    private void StartAttackIfReady()
    {
        if (Time.time < _nextAttackTime)
        {
            return;
        }

        _nextAttackTime = Time.time + _attackCooldown;
        if (_animator != null && !string.IsNullOrEmpty(_attackTrigger))
        {
            _animator.SetTrigger(_attackTrigger);
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isDead || amount <= 0)
        {
            return;
        }

        _health -= amount;
        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        OnDie();
        Destroy(gameObject);
    }

    protected void OnDie()
    {
        Died?.Invoke(this);
    }
}

