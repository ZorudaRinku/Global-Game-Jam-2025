using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerTemplate playerNumber;
    public bool alive;
    public int number;

    void Start()
    {
        alive = playerNumber.PlayerAlive;
        number = playerNumber.playerNumber;
    }
}
