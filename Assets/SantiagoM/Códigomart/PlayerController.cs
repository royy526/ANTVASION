using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Estadísticas del player")]
    public int PlayerHealth = 6;
    private float playerSpeed = 5.0f;
    public float SpeedStat = 1.0f;
    public float PlayerPoison = 0f;
    public float PlayerDamage = 3f;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;
    private float attackDuration = 0.4f;
    public float BulletVelocity = 4f;
    private float shootTimer = 0.5f;
    public float ShootCooldown = 1f;
    public float PlayerLuck = 1f;
    private Vector2 playerMoves;
    private Rigidbody2D _playerRigidBody2D;
    public List<string> Inventory;
    [Header("Configuración del ataque")]
    public Transform Aim;
    public GameObject Bullet;
    private Vector3 LookAt;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject melee;
    private bool IsAttacking = false;
    
   
    void Start()
    {
        _playerRigidBody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMovement();
        PlayerAttack();
        PlayerLook();
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
    public void PlayerAttack()
    {
        MeleeTimer();
        shootTimer += Time.time;
        if (Input.GetMouseButton(0))
        {
            OnAttack();
        }
        if(Input.GetMouseButton(1)) //&& disparo == true)
        {
            OnShoot();
        }
    }
    void OnAttack()
    {
        if (!IsAttacking)
        {
            melee.SetActive(true);
            IsAttacking = true;
            //ANIMACIÓN DE ATAQUE   
        }
    }
    void OnShoot()
    {
        if(shootTimer > ShootCooldown)
        {
            shootTimer = 0;
            GameObject intBullet = Instantiate(Bullet, Aim.position, Aim.rotation);
            intBullet.GetComponent<Rigidbody2D>().AddForce(Aim.up * BulletVelocity, ForceMode2D.Impulse);
            Destroy(intBullet, 2f);
        }

    }
    void MeleeTimer()
    {
        if (IsAttacking)
        {
            AttackCooldown += Time.deltaTime;
            if (AttackCooldown >= attackDuration)
            {
                AttackCooldown = 0;
                IsAttacking = false;
                melee.SetActive(false);
            }
        }
    }
    private void PlayerLook()
    {
        LookAt = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        float anglerads = Mathf.Atan2(LookAt.y - transform.position.y, LookAt.x - transform.position.x);
        float angledeg = (180 / Mathf.PI) * anglerads - 90;
        transform.rotation = Quaternion.Euler(0, 0, angledeg);
    }
}
