/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletController_v2 : MonoBehaviour
{
    public float LifeTime;

    public bool IsEnemyBullet = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DeathDealy());
        transform.localscale = new Vector2(GameController.BulletSize, GameController.BulletSize);
    }

    IEnumerator DeathDealy()
    {
        yield return new WaitForSeconds(LifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D Collider)
    {

        if (Collider.tag == "Enemy")
        {
            Collider.gameObject.GetComponent<EnemyController_v2>().Death;
            Destroy(gameObject);
        }

    }

    
}*/
