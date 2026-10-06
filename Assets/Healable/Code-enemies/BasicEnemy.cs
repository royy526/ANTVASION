using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();

        health = 4;
        damage = 10;
        speed = .2f;
    }
}
