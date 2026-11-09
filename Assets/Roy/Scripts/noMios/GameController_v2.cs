// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour // Revisar: GameController también está declarada en GameController.cs
{
    public static GameController instance; // Usar PascalCase

    GameObject Player; // Usar _camelCase

    private static float _health = 6; // Respetar el orden de acceso y static

    private static int _maxHealth = 10; // Sin abreviaciones

    private static float _moveSpeed = 5f;

    private static float _fireRate = 0.5f;

    private static float _bulletSize = 0.5f; 

    public static float Health { get => _health; set => _health = value; }

    public static int MaxHealth { get => _maxHealth; set => _maxHealth = value; } // Sin abreviaciones

    public static float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    public static float AttackSpeedRate { get => _fireRate; set => _fireRate = value; }
    public static float BulletSize { get => _bulletSize; set => _bulletSize = value; }


    [SerializeField] private TextMeshProUGUI HealthText; // Usar _camelCase | Respetar el orden de los miembros
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


    public static void DamagePlayer(int Damage, GameObject Player) // Usar camelCase en el parámetro Damage | Usar camelCase en el parámetro Player
    {
        _health -= Damage;

        if (_health <= 0) { KillPlayer(Player); }
    }

    public static void HealPlayer(float HealAmount) // Usar camelCase en el parámetro HealAmount
    {
        _health = Mathf.Min(_maxHealth, _health + HealAmount);
    }
    public static void FireRateChange(float Rate) // Usar un verbo en el nombre del método | Usar camelCase en el parámetro Rate
    {
        _fireRate -= Rate;
    }

    public static void BulletSizeChange(float Size) // Usar un verbo en el nombre del método | Usar camelCase en el parámetro Size
    {
        _bulletSize += Size;
    }

    public static void MoveSpeedChange(float Speed) // Usar un verbo en el nombre del método | Usar camelCase en el parámetro Speed
    {
        MoveSpeed += Speed;
    }

    public static void KillPlayer(GameObject Player) // Usar camelCase en el parámetro Player
    {
        Destroy(Player);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void OnTriggerEnter2D(Collider2D Collition, GameObject Player) // Corregir la ortografía de Collition | Usar camelCase en el parámetro Collition | Usar camelCase en el parámetro Player | Respetar el orden de los miembros | Respetar el orden de acceso y static | Revisar: Unity espera un método de instancia con un solo Collider2D
    {
        if (Collition.gameObject.CompareTag("Enemy"))
        {
            DamagePlayer(1, Player);
        }
    }

}
