using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Player Controls/Turn Order
    private int _currentPlayerIndex = 2;
    [FormerlySerializedAs("_players")] [SerializeField] private GameObject[] playerObjects;
    [SerializeField] private GameObject throwObject;
    [SerializeField] private GameObject UI;
    private Dictionary<GameObject, bool> players;
    
    // Bubble Variables
    private int _bubblePopThreshold;

    void Start()
    {
        players = new Dictionary<GameObject, bool>();
        foreach (var player in playerObjects)
        {
            players.Add(player, false); // Initialize all players as not eliminated
        }
    }
    
    public void ThrowObject(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        Debug.Log(GetCurrentPlayer().name);

        // Get the current player
        GameObject currentPlayer = GetCurrentPlayer();

        GameObject throwable = Instantiate(throwObject, currentPlayer.transform.GetChild(2).transform.position, Quaternion.identity);
        throwable.transform.SetParent(UI.transform);
    }

    public void EndTurn(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times

        if (GameObject.Find("ThrowObject(Clone)") != null) return; // Player should not be able to end their turn while the object is still in the air

        Debug.Log(GetCurrentPlayer().name);

        // Increment the current player index
        _currentPlayerIndex++;

        // If the current player index is greater than the number of players, reset it to 0
        if (_currentPlayerIndex >= players.Count)
        {
            _currentPlayerIndex = 0;
        }
    }

    // Getter for the current player index
    public int GetCurrentPlayerIndex()
    {
        return _currentPlayerIndex;
    }

    // Getter for players
    public Dictionary<GameObject, bool> GetPlayers()
    {
        return players;
    }

    // Getter for current player
    public GameObject GetCurrentPlayer()
    {
        int index = 0;
        foreach (var player in players)
        {
            if (index == _currentPlayerIndex)
            {
                return player.Key;
            }
            index++;
        }
        return null;
    }

    // Method to eliminate a player
    public void EliminatePlayer(GameObject player)
    {
        if (players.ContainsKey(player))
        {
            players[player] = true; // Mark the player as eliminated
        }
    }

    // Method to check if a player is eliminated
    public bool IsPlayerEliminated(GameObject player)
    {
        if (players.ContainsKey(player))
        {
            return players[player];
        }
        return false;
    }
}