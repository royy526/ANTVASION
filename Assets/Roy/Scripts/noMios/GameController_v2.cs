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

    private static float _moveSpeed = 5f;

    private static float _fireRate = 0.5f;

    private static float _bulletSize = 0.5f; 

    public static float Health { get => _health; set => _health = value; }

    public static int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    public static float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    public static float AttackSpeedRate { get => _fireRate; set => _fireRate = value; }
    public static float BulletSize { get => _bulletSize; set => _bulletSize = value; }


    [SerializeField] private TextMeshProUGUI HealthText;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        HealthText.text = "Vida: " + _health;
        _health = 6;
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
        _fireRate -= Rate;
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

    public static void OnTriggerEnter2D(Collider2D Collition, GameObject Player)
    {
        if (Collition.gameObject.CompareTag("Enemy"))
        {
            DamagePlayer(1, Player);
        }
    }

}
