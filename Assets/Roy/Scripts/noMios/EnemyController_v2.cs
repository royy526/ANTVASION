// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyState // Mover el enum público a su propio archivo | Revisar: EnemyState y EnemyType ya están declarados en EnemyController.cs
{
    Wander, Follow, Die, Attack
};

public enum EnemyType // Mover el enum público a su propio archivo
{
    Melee,
    Ranged
};


public class EnemyController_v2 : MonoBehaviour // Usar PascalCase
{
    GameObject Player; // Usar _camelCase
    public EnemyState CurrentState = EnemyState.Wander; // Respetar el orden de acceso y static | Separar los grupos de acceso con una línea en blanco
    public EnemyType EnemyType;
    public float Range;
    public float Speed;
    public float AttackRange;
    public float CoolDown;

    Rigidbody2D RigidBody; // Usar _camelCase

    private bool _chosenDirection = false; // Revisar: usar un verbo de estado (is, has o can)
    private bool _isDead = false;
    private bool _isCoolDownAttack = false;
    private Vector2 _randomDirection;

    public float _health = 2f; // Usar PascalCase | Respetar el orden de acceso y static

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        RigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        switch (CurrentState)
        {
            case (EnemyState.Wander):
                Wander();
            break;
            case (EnemyState.Follow): // Separar los case con una línea en blanco
                Follow();
            break;
            case (EnemyState.Die): // Separar los case con una línea en blanco
                Death();
            break;
            case (EnemyState.Attack): // Separar los case con una línea en blanco
            Attack();
            break;
        }

        if (IsPlayerInRange(Range) && CurrentState != EnemyState.Die)
        {
            CurrentState = EnemyState.Follow;
        }
        else if (IsPlayerInRange(Range) && CurrentState != EnemyState.Die)
        {
            CurrentState=EnemyState.Wander;
        }


        if (Vector2.Distance(transform.position, Player.transform.position) <= AttackRange)
        {
            CurrentState = EnemyState.Attack;
        }

        
    }

    private bool IsPlayerInRange(float Range) // Usar camelCase en el parámetro Range
    {
        return Vector2.Distance(transform.position, Player.transform.position) <= Range;
    }

    private IEnumerator ChooseDirection()
    {
        _chosenDirection= true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        _randomDirection = new Vector2(0, Random.Range(0, 360));
        Quaternion nextRotation = Quaternion.Euler(_randomDirection );
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
        _chosenDirection = false;
    }
    
    void Wander()
    {
        if (!_chosenDirection)
        {
            StartCoroutine(ChooseDirection());
        }

        Vector2 newPosition = RigidBody.position + _randomDirection * Time.fixedDeltaTime;
        RigidBody.MovePosition(newPosition);
        if (IsPlayerInRange(Range))
        {
            CurrentState = EnemyState.Follow;
        }
    }

    void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, Speed * Time.deltaTime);
    }

    void Attack()
    {
        if (!_isCoolDownAttack)
        {
            switch (EnemyType)
            {
                case (EnemyType.Melee):
                    GameController.DamagePlayer(1, Player);
                    StartCoroutine(Cooldown());
                    break;
                case (EnemyType.Ranged): // Separar los case con una línea en blanco
                    //GameObject Bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity) as GameObject;
                    //Bullet.GetComponent<BulletController>().GetPlayer(Player.transform);

                    break;
            }
        }
    }

    private IEnumerator Cooldown()
    {
        _isCoolDownAttack = true;
        yield return new WaitForSeconds(CoolDown);
        _isCoolDownAttack = false;
    }

    public void Death() // Usar un verbo en el nombre del método | Respetar el orden de acceso y static
    {
        Destroy(gameObject);
    }

    public void ReceivedDamage(Collider2D Collition) // Corregir la ortografía de Collition | Usar camelCase en el parámetro Collition
    {


        if (Collition.gameObject.CompareTag("bullet") || Collition.gameObject.CompareTag("Melee"))
        {
            _health -= 1;
            if (_health == 0)
            {
                Death();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D Collition) // Corregir la ortografía de Collition | Usar camelCase en el parámetro Collition | Respetar el orden de los miembros
    {
        ReceivedDamage(Collition);
    }
}
