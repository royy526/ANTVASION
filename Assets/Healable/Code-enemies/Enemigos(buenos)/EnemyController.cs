// Sugerencia general de Clean Code: usar if de una sola línea con llaves cuando sea legible.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyState // Mover el enum público a su propio archivo | Revisar: EnemyState y EnemyType también están declarados en EnemyController_v2.cs
{
    Wander,
    Follow,
    Die,
    Attack

};

public enum EnemyType // Mover el enum público a su propio archivo
{
    Melee,
    Ranged
};

public class EnemyController : MonoBehaviour
{

    GameObject Player; // Usar _camelCase

    public EnemyState CurrentState = EnemyState.Wander; // Respetar el orden de acceso y static
    public EnemyType EnemyType;

    public float Range;
    public float Speed;
    public float AttackRange;
    public float coolDown; // Usar PascalCase


    private bool _chosenDirection = false; // Revisar: usar un verbo de estado (is, has o can)

    private bool _isDead = false;

    private bool _isCoolDownAttack = false;

    private Vector2 _randomDirection;

    public GameObject BulletPrefab; // Respetar el orden de acceso y static


    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
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

        transform.position += -transform.right * Speed * Time.deltaTime;
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
                    GameController.DamagePlayer(1);
                    StartCoroutine(Cooldown());
                    break;
                case (EnemyType.Ranged): // Separar los case con una línea en blanco
                    GameObject Bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity) as GameObject; // Usar camelCase sin _
                    break;
            }
        }
    }

    private IEnumerator Cooldown()
    {
        _isCoolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        _isCoolDownAttack = false;
    }

    public void Death() // Usar un verbo en el nombre del método | Respetar el orden de acceso y static
    {
        Destroy(gameObject);
    }
}
