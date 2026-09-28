using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();

        health = 100;
        damage = 10;
        speed = 2f;
    }
}
