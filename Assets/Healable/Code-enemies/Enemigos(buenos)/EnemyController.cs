using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyState
{
    Wander,
    Follow,
    Die,
    Attack

};

public class EnemyController : MonoBehaviour
{

    GameObject Player;

    public EnemyState CurrentState = EnemyState.Wander;

    public float Range;
    public float Speed;
    public float AttackRange;
    public float coolDown;


    private bool _chosenDirection = false;

    private bool _isDead = false;

    private bool _isCoolDownAttack = false;

    private Vector2 _randomDirection;


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
                case (EnemyState.Follow):
                    Follow();
                break;
                case (EnemyState.Die):

                break;
                case (EnemyState.Attack):
                Attack();
                break;
        }

        if (IsPlayerInRange(Range) && CurrentState != EnemyState.Die)
        {
            CurrentState = EnemyState.Follow;
        }
        else if(IsPlayerInRange(Range) && CurrentState != EnemyState.Die)
        {
            CurrentState=EnemyState.Wander;
        }


        if(Vector2.Distance(transform.position, Player.transform.position) <= AttackRange)
        {
            CurrentState = EnemyState.Attack;
        }
    }

    private bool IsPlayerInRange(float Range)
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
            GameController.DamagePlayer(1);
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        _isCoolDownAttack = true;
        yield return new WaitForSeconds(coolDown);
        _isCoolDownAttack = false;
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
