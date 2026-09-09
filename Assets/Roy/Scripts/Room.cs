using UnityEngine;

public enum RoomType {Start, Normal, Boss, Treasure, Shop, Secret}
public class Room : MonoBehaviour
{
    [SerializeField] private GameObject _doorUp;
    [SerializeField] private GameObject _doorDown;
    [SerializeField] private GameObject _doorLeft;
    [SerializeField] private GameObject _doorRight;
    public RoomType RoomType {get; private set;}
    public Vector2Int GridPos {get; private set;}
    
    public void Setup(RoomType type, Vector2Int gridPos, bool up, bool down, bool left, bool right)
    {
        RoomType = type;
        GridPos = gridPos;

        if(_doorUp != null) 
        {
            _doorUp.SetActive(up);
        }
        if(_doorDown != null)
        {
            _doorDown.SetActive(down);
        }
        if(_doorLeft != null)
        {
            _doorLeft.SetActive(left);
        }
        if(_doorRight != null)
        {
            _doorRight.SetActive(right);
        }
    }
}
