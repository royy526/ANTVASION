using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class GameController : MonoBehaviour // Revisar: GameController también está declarada en GameController_v2.cs
{
    public static GameController instance; // Usar PascalCase

    private static float _health = 10;

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
    }

    private void Update()
    {
        HealthText.text = "Vida: " + _health;
    }


    public static void DamagePlayer(int Damage) // Usar camelCase en el parámetro Damage
    {
        _health -= Damage;

        if (_health <= 0) { KillPlayer(); }
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

    public static void KillPlayer()
    {

    }

}
