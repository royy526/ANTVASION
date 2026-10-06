using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public Stat MaxHealth = new Stat { BaseValue = 6f };
    public Stat MoveSpeed = new Stat { BaseValue = 5f };
    public Stat Damage = new Stat { BaseValue = 3f };
    public Stat ShootCooldown = new Stat { BaseValue = 1f };
    public Stat BulletSpeed = new Stat { BaseValue = 4f };
    public Stat MeleeCooldown = new Stat { BaseValue = 0.6f };
    public Stat Luck = new Stat { BaseValue = 1f };
}