using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum GameState
{
    Lobby,
    Game
}

public enum RoundState
{
    InProgress,
    BubblePopped,
    ResetRound
}

public class GameManager : MonoBehaviour
{
    // Player Controls/Turn Order
    public NumPlayers numPlayers;
    public int CurrentPlayerIndex { get; private set; }
    private int _currentPlayerSentObjects;
    [FormerlySerializedAs("_players")] [SerializeField] private GameObject[] playerObjects;
    [SerializeField] private GameObject throwObject;
    [FormerlySerializedAs("UI")] [SerializeField] private GameObject ui;
    [SerializeField] private GameObject bubble;
    private Animator _bubbleAnimator;
    public Dictionary<GameObject, bool> Players { get; private set; }
    
    // Bubble Variables
    private int _bubblePopThreshold;
    private int _bubblePopCount;

    // State Machine

    private GameState _previousGameState;
    public GameState CurrentGameState { get; private set; }
    private RoundState _previousRoundState;
    public RoundState CurrentRoundState { get; private set; }
    
    // Events
    
    void Start()
    {
        _bubblePopThreshold = Random.Range(2, 10);
        _bubbleAnimator = bubble.GetComponent<Animator>();
        Players = new Dictionary<GameObject, bool>();
        foreach (var player in playerObjects)
        {
            Players.Add(player, true); // Initialize all players as alive
        }
        
        CurrentGameState = GameState.Game; // TODO: Change to Lobby when Menu is implemented
        CurrentRoundState = RoundState.InProgress;
        
        // Subscribe to bubble animation notifications
        bubble.GetComponent<Bubble>().OnBubblePopFinished += HandleBubbleFinishPop; // After the bubble pop animation finishes, reset the round
        bubble.GetComponent<Bubble>().OnBubbleResetFinished += HandleBubbleFinishReset; // After the bubble reset animation finishes, start the next round
    }

    // Methods
    
    public void ThrowObject(InputAction.CallbackContext context) // Triggered by Input Manager (Space or A)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        if (CurrentRoundState != RoundState.InProgress) return; // Player should not be able to throw object while the bubble is popping or resetting
        
        GameObject throwable = Instantiate(throwObject, GetCurrentPlayer().transform.GetChild(2).transform.position, Quaternion.identity); // Spawn throwable object at player's hand
        throwable.transform.SetParent(ui.transform); // Set the throwable object as a child of the UI or else it will not be visible
        throwable.GetComponent<ThrowObject>().OnThrowObjectHitBubble += HandleBubbleHit; // Subscribe to the event of the object hitting the bubble
        _currentPlayerSentObjects++;
        
        Debug.Log($"{GetCurrentPlayer().name} Threw Object");
    }

    private void HandleBubbleHit() // Triggered by ThrowObject.cs action event
    {
        if (CurrentRoundState != RoundState.InProgress) return; // Player should not be able to hit the bubble while the bubble is popping or resetting
        
        _bubbleAnimator.SetTrigger("Hit"); // Play the hit animation
        _bubblePopCount++; // Increment the bubble pop count
        if (_bubblePopCount >= _bubblePopThreshold) // Check if the bubble has popped
        {
            CurrentRoundState = RoundState.BubblePopped; // Set the round state to BubblePopped
            _bubbleAnimator.SetTrigger("Pop"); // Play the pop animation
        }
        
        Debug.Log("Bubble Hit " + _bubblePopCount + "/" + _bubblePopThreshold);
    }
    
    private void HandleBubbleFinishPop() // Triggered by Bubble.cs action event
    {
        // Set the current player to dead as the bubble has popped
        Players[GetCurrentPlayer()] = false;
        
        // Move on to the next player
        MoveToNextPlayer();
        
        // Check if there is only one player left
        int alivePlayers = 0;
        foreach (var player in Players)
        {
            if (player.Value)
            {
                alivePlayers++;
            }
        }
        
        if (alivePlayers == 1)
        {
            Debug.Log("Game Over");
            CurrentGameState = GameState.Lobby;
            return;
        }
        
        // Reset the bubble
        _bubbleAnimator.SetTrigger("Reset");
    }
    
    private void HandleBubbleFinishReset() // Triggered by Bubble.cs action event
    {
        _bubblePopCount = 0;
        _bubblePopThreshold = Random.Range(2, 10);
        CurrentRoundState = RoundState.InProgress;
    }

    public void EndTurn(InputAction.CallbackContext context) // Triggered by Input Manager (Enter or B)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times

        if (GameObject.Find("ThrowObject(Clone)") != null) return; // Player should not be able to end their turn while the object is still in the air
        
        if (CurrentRoundState != RoundState.InProgress) return; // Player should not be able to end their turn while the bubble is popping or resetting
        
        if (_currentPlayerSentObjects == 0) return; // Player should not be able to end their turn without throwing an object

        Debug.Log(GetCurrentPlayer().name + " Ended Turn");

        MoveToNextPlayer();
    }

    private void MoveToNextPlayer() // Move on to the next player
    {
        // Increment the current player index, skip dead players, and loop back to the first player if necessary
        do
        {
            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= playerObjects.Length)
            {
                CurrentPlayerIndex = 0;
            }
        } while (!Players[GetCurrentPlayer()]);
        _currentPlayerSentObjects = 0;
    }

    // Helpers
    
    // Getter for current player
    public GameObject GetCurrentPlayer()
    {
        return playerObjects[CurrentPlayerIndex];
    }
}