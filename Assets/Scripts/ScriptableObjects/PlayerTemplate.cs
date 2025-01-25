using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTemplate", menuName = "Scriptable Objects/PlayerTemplate")]
public class PlayerTemplate : ScriptableObject
{
    public bool PlayerAlive = true;
    [SerializeField] public int playerNumber;
    [SerializeField] public int xPos;
    [SerializeField] public int yPos;
}
