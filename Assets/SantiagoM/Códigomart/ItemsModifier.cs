using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


[CreateAssetMenu(fileName = "item.asset" , menuName = "Inventory/Item")]
public class ItemsModifier : ScriptableObject
{
    [SerializeField] private SpriteRenderer _itemSprite;
    [SerializeField] private string _itemName;
    [SerializeField] private string _itemDescription;
    public float MaxHealth; // Sin abreviaciones | Respetar el orden de acceso y static | Separar los grupos de acceso con una línea en blanco
    public float MoveSpeed;
    public float Damage;
    public float ShootCooldown;
    public float BulletSpeed;
    public float MeleeCooldown;
    public float Luck; 
}

