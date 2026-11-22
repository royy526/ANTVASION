using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    GameObject Player;

    private static float _health = 6;
    private static int _maxHealth = 10;
    private static float _moveSpeed = 0f;
    private static float _fireRate = .5f;
    private static float _bulletSize = 1f;
    private static float _damage = 1f;
    private static float _meleeColdoown = 1f;
    private static float _bulletSpeed = 10f;

    [Header("Valores base")]
    [SerializeField] private float _baseHealth = 6f;
    [SerializeField] private float _baseMoveSpeed = .1f;
    [SerializeField] private float _baseShootCooldown = 2;
    [SerializeField] private float _baseMeleeCooldown = 1;
    [SerializeField] private float _baseBulletSize = 1;
    [SerializeField] private float _baseDamage = 1f;
    [SerializeField] private float _baseBulletSpeed = 10f;

    public static float Health { get => _health; set => _health = value; }
    public static int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public static float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public static float AttackSpeedRate { get => _fireRate; set => _fireRate = value; }
    public static float BulletSize { get => _bulletSize; set => _bulletSize = value; }
    public static float Damage { get => _damage; set => _damage = value; }
    public static float MeleeCooldown { get => _meleeColdoown; set => _meleeColdoown = value; }
    public static float BulletSpeed { get => _bulletSpeed; set => _bulletSpeed = value; }


    [SerializeField] private TextMeshProUGUI HealthText;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        Player = GameObject.FindGameObjectWithTag("Player");
        ResetStats();
    }

    private void Start()
    {
        HealthText.text = "Vida: " + _health;
    }

    private void Update()
    {
        HealthText.text = "Vida: " + _health;
    }


    public static void DamagePlayer(int Damage, GameObject Player)
    {
        _health -= Damage;

        if (_health <= 0) { KillPlayer(Player); }
    }

    public static void HealPlayer(float HealAmount)
    {
        _health = Mathf.Min(_maxHealth, _health + HealAmount);
    }
    public static void FireRateChange(float Rate)
    {
        _fireRate -= Mathf.Max(0.1f, _fireRate - Rate);

    }

    public static void BulletSizeChange(float Size)
    {
        _bulletSize += Size;
    }

    public static void MoveSpeedChange(float Speed)
    {
        MoveSpeed += Speed;
    }

    public static void KillPlayer(GameObject Player)
    {
        Destroy(Player);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void DamageChange(float Damage)
    {
        _damage += Damage;
    }

    public static void MeleeCooldownChange(float Cooldown)
    {
        _meleeColdoown = Mathf.Max(0.05f, _meleeColdoown - Cooldown);
    }

    public static void BulletSpeedChange(float Speed)
    {
        _bulletSpeed += Speed;
    }

    private bool _collected;
    public static void OnTriggerEnter2D(Collider2D Collition, GameObject Player)
    {
        if (Collition.gameObject.CompareTag("Enemy")) { DamagePlayer(1, Player); }

        

    }


    private void ResetStats()
    {
        _health = _baseHealth;
        _moveSpeed = _baseMoveSpeed;
        _fireRate = _baseShootCooldown;
        _meleeColdoown = _baseMeleeCooldown;
        _bulletSize = _baseBulletSize;
        _damage = _baseDamage;
        _bulletSpeed = _baseBulletSpeed;
    }
}
