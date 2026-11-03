using UnityEngine;

[System.Serializable]
public class Item
{
    public string Name;
    public string Description;
    public Sprite ItemImage;

}

public class CollectionContoller : MonoBehaviour
{
    public Item Item;

    public float HealthChange;

    public float MoveSpeedChange;

    public float AttackSpeedChange;

    public float BulletSizeChange;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Item.ItemImage;
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //PlayerController.collectedAmount++;
            GameController.HealPlayer(HealthChange);
            GameController.MoveSpeedChange(MoveSpeedChange);
            GameController.FireRateChange(AttackSpeedChange);
            GameController.BulletSizeChange(BulletSizeChange);
            Destroy(gameObject);
        }
    }
}
