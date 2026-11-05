using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyState
{
    Wander, Follow, Die, Attack
};

public enum EnemyType
{
    Melee,
    Ranged
};


public class EnemyController_v2 : MonoBehaviour
{
    GameObject Player;
    public EnemyState CurrentState = EnemyState.Wander;
    public EnemyType EnemyType;
    public float Range;
    public float Speed;
    public float AttackRange;
    public float CoolDown;

    Rigidbody2D RigidBody;

    private bool _chosenDirection = false;
    private bool _isDead = false;
    private bool _isCoolDownAttack = false;
    private Vector2 _randomDirection;

    public float _health = 2f;

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
            case (EnemyState.Follow):
                Follow();
            break;
            case (EnemyState.Die):
                Death();
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
        //transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 2.5f));
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
                case (EnemyType.Ranged):
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

    public void Death()
    {
        Destroy(gameObject);
    }

    public void ReceivedDamage(Collider2D Collition)
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

    private void OnTriggerEnter2D(Collider2D Collition)
    {
        ReceivedDamage(Collition);
    }
}
