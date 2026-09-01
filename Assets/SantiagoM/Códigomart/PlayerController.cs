using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Estadísticas del player")]
    public int PlayerHealth = 6;
    private float playerSpeed = 1.0f;
    public float MovementSpeed = 1.0f;
    public float PlayerPoison = 0f;
    public float PlayerDamage = 4f;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 2f;
    public float PlayerLuck = 1f;
    private Rigidbody2D _playerRigidBody2D;
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
        float ActualPlayerSpeed = playerSpeed * MovementSpeed; 
        float HorizontalMovement = Input.GetAxisRaw("Horizontal");
        float VerticalMovement = Input.GetAxisRaw("Vertical");
        _playerRigidBody2D.linearVelocity = new Vector2(HorizontalMovement * ActualPlayerSpeed, VerticalMovement * ActualPlayerSpeed).normalized;
    }
}
