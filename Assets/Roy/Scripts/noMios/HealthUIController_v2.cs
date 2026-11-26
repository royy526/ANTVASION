using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HealthUIController_v2 : MonoBehaviour
{
    public float Life;
    public float MaxLife;

    public Sprite FullHeart;
    
    public Image[] Hearts;

    public GameController _health;


    // Update is called once per frame
    void Update()
    {
        Life = GameController.Health;
        MaxLife = GameController.MaxHealth;

        for (int i = 0; i < Hearts.Length; i++)
        {
            if(i < Life)
            {
                Hearts[i].enabled = FullHeart;
            }
            else
            {
                Hearts[i].enabled=false;
            }
        }
    }
}
