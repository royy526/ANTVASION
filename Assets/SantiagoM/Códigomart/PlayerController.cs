using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración del ataque")]
    public Transform Aim;
    public GameObject Bullet;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _melee;
    [SerializeField] private float _attackDuration = 0.4f;

    public List<string> Inventory;

    private PlayerStats _stats;
    private Rigidbody2D _rigidBody2D;
    private DamageDealer _meleeDamage;

    private Vector2 _moveInput;
    private bool _isAttacking;
    private float _attackTimer;
    private float _meleeCooldownTimer;
    private float _shootCooldownTimer;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _stats = GetComponent<PlayerStats>();

        if (_melee != null)
        {
            _meleeDamage = _melee.GetComponent<DamageDealer>();
        }
    }

    private void Update()
    {
        ReadMovementInput();
        HandleAttacks();
        PlayerLook();
    }

    private void FixedUpdate()
    {
        _rigidBody2D.linearVelocity = _moveInput * _stats.MoveSpeed.Value;
    }

    private void ReadMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector2(horizontal, vertical).normalized;
    }

    private void HandleAttacks()
    {
        _meleeCooldownTimer -= Time.deltaTime;
        _shootCooldownTimer -= Time.deltaTime;

        UpdateMeleeDuration();

        if (Input.GetMouseButton(0))
        {
            Melee();
        }

        if (Input.GetMouseButton(1))
        {
            Shoot();
        }
    }

    private void Melee()
    {
        if (_isAttacking || _meleeCooldownTimer > 0f) return;

        if (_meleeDamage != null)
        {
            _meleeDamage.Damage = _stats.Damage.Value;
        }

        _melee.SetActive(true);
        _isAttacking = true;
        _attackTimer = 0f;
        _meleeCooldownTimer = _stats.MeleeCooldown.Value;
        // ANIMACIÓN DE ATAQUE
    }

    private void UpdateMeleeDuration()
    {
        if (!_isAttacking) return;

        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackDuration)
        {
            _isAttacking = false;
            _melee.SetActive(false);
        }
    }

    private void Shoot()
    {
        if (_shootCooldownTimer > 0f) return;

        _shootCooldownTimer = _stats.ShootCooldown.Value;

        GameObject bullet = Instantiate(Bullet, Aim.position, Aim.rotation);

        DamageDealer bulletDamage = bullet.GetComponent<DamageDealer>();
        if (bulletDamage != null)
        {
            bulletDamage.Damage = _stats.Damage.Value;
        }

        bullet.GetComponent<Rigidbody2D>().AddForce(Aim.up * _stats.BulletSpeed.Value, ForceMode2D.Impulse);
        Destroy(bullet, 2f);
    }

    private void PlayerLook()
    {
        Vector3 lookAt = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        float angleRad = Mathf.Atan2(-lookAt.y - transform.position.y, -lookAt.x - transform.position.x);
        float angleDeg = angleRad * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }




}
