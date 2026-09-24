using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Estadísticas del player")]
    public int PlayerHealth = 6;
    private float playerSpeed = 5.0f;
    public float SpeedStat = 1.0f;
    public float PlayerPoison = 0f;
    public float PlayerDamage = 3f;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 2f;
    public float PlayerLuck = 1f;
    private Vector2 playerMoves;
    private Rigidbody2D _playerRigidBody2D;
    public List<string> Inventory;
    void Start()
    {
       _playerRigidBody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMovement();
    }

    public void PlayerMovement()
    {
        float ActualPlayerSpeed = playerSpeed * SpeedStat;
        float HorizontalMovement = Input.GetAxisRaw("Horizontal");
        float VerticalMovement = Input.GetAxisRaw("Vertical");
        playerMoves = new Vector2(HorizontalMovement * ActualPlayerSpeed, VerticalMovement * ActualPlayerSpeed).normalized;
        _playerRigidBody2D.linearVelocity = new Vector2(HorizontalMovement, VerticalMovement);
    }
    private void FixedUpdate()
    {

    }

    public void Attack()
    {

    }
}
