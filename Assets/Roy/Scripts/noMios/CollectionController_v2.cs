using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class Item
{
    public string Name;
    public string Description;
    public Sprite ItemImage;

}


public class CollectionController_v2 : MonoBehaviour
{
    public Item Item;
    public float HealthChange;
    public float MoveSpeedChange;
    public float AttackSpeedChange;
    public float BulletSizeChange;
    public float DamageChange;
    public float MeleeCooldownChange;
    public float BulletSpeedChange;
    protected ItemSpawner_v2 _spawner;


    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Item.ItemImage;
        Destroy(GetComponent<PolygonCollider2D>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameController.HealPlayer(HealthChange);
            GameController.MoveSpeedChange(MoveSpeedChange);
            GameController.FireRateChange(AttackSpeedChange);
            GameController.BulletSizeChange(BulletSizeChange);
            GameController.DamageChange(DamageChange);
            GameController.MeleeCooldownChange(MeleeCooldownChange);
            GameController.BulletSpeedChange(BulletSpeedChange);
            Destroy(gameObject);
        }
    }

    public void SetSpawner(ItemSpawner_v2 itemSpawner)
    {
        _spawner = itemSpawner;
    }
}
