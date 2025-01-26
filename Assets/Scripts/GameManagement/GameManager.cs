using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    [FormerlySerializedAs("_players")] [SerializeField] private GameObject[] playerObjects;
    [FormerlySerializedAs("playertemplates")] [SerializeField] private PlayerTemplate[] playerTemplates;
    [SerializeField] private GameObject throwObject;
    [FormerlySerializedAs("UI")] [SerializeField] private GameObject ui;
    [SerializeField] private GameObject bubble;
    private Animator _bubbleAnimator;
    public List<GameObject> Players;

    // Bubble Variables
    private int _bubblePopThreshold;
    private int _bubblePopCount;
    
    // Inventory Variables
    public int currentInventoryIndex;

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

        // player initialization phase
        Players = new List<GameObject>();

        // clears living states from previous game
        for(int i = 0; i < 4; i++)
        {
            playerTemplates[i].PlayerAlive = false;
        }

        // enables however many players were selected in main menu
        for(int i = 0; i < numPlayers.numberOfPlayers; i++)
        {
            playerTemplates[i].PlayerAlive = true;
            playerObjects[i].SetActive(true);
            Players.Add(playerObjects[i]);
            Debug.Log("Player " + i + " added successfully");
        }

        CurrentGameState = GameState.Game; // TODO: Change to Lobby when Menu is implemented
        CurrentRoundState = RoundState.InProgress;
        
        // Subscribe to bubble animation notifications
        bubble.GetComponent<Bubble>().OnBubblePopFinished += HandleBubbleFinishPop; // After the bubble pop animation finishes, reset the round
        bubble.GetComponent<Bubble>().OnBubbleResetFinished += HandleBubbleFinishReset; // After the bubble reset animation finishes, start the next round
    } // start

    // Methods
    
    public void ThrowObject(InputAction.CallbackContext context) // Triggered by Input Manager (Space or A)
    {
        // check if game is over before throwing object
        if (CurrentGameState == GameState.Lobby)
        {
            ReturnToMainMenu();
        }

        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        if (CurrentRoundState != RoundState.InProgress) return; // Player should not be able to throw object while the bubble is popping or resetting

        GameObject throwable = Players[CurrentPlayerIndex].GetComponent<Player>().ThrowObject();
        throwable.GetComponent<ThrowObject>().OnThrowObjectHitBubble += HandleBubbleHit; // Subscribe to the event of the object hitting the bubble
    } // ThrowObject

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
    } // HandleBubbleHit
    
    private void HandleBubbleFinishPop() // Triggered by Bubble.cs action event
    {
        // Set the current player to dead as the bubble has popped
        Players[CurrentPlayerIndex].GetComponent<Player>().alive = false;

        // Move on to the next player
        MoveToNextPlayer();
        
        int alivePlayers = 0;
        int winnerNumber = 0;
        
        // Check if there is only one player left
        foreach (var player in Players)
        {
            if (player.GetComponent<Player>().alive)
            {
                alivePlayers++;
                winnerNumber = player.GetComponent<Player>().number;
            }
        }

        // set game state to end if only one player remains
        if (alivePlayers == 1)
        {
            Debug.Log("Player " + winnerNumber + " wins!");
            CurrentGameState = GameState.Lobby;
            return;
        }
        
        // Reset the bubble
        _bubbleAnimator.SetTrigger("Reset");
    } // HandleBubbleFinishPop
    
    private void HandleBubbleFinishReset() // Triggered by Bubble.cs action event
    {
        _bubblePopCount = 0;
        _bubblePopThreshold = Random.Range(2, 10);
        CurrentRoundState = RoundState.InProgress;
    } // HandleBubbleFinishReset

    public void EndTurn(InputAction.CallbackContext context) // Triggered by Input Manager (Enter or B)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        if (GameObject.Find("ThrowObject(Clone)") != null) return; // Player should not be able to end their turn while the object is still in the air
        if (CurrentRoundState != RoundState.InProgress) return; // Player should not be able to end their turn while the bubble is popping or resetting

        if (Players[CurrentPlayerIndex].GetComponent<Player>().EndTurn())
        {
            MoveToNextPlayer();
            Debug.Log(Players[CurrentPlayerIndex].name + " Ended Turn");
            return;
        }
    } // EndTurn

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
        } while (!Players[CurrentPlayerIndex].GetComponent<Player>().alive);
        // while (!playerTemplates[CurrentPlayerIndex].PlayerAlive);
    } // MoveToNextPlayer

    public void InventoryLeft(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        currentInventoryIndex--;
        if (currentInventoryIndex < 0)
        {
            currentInventoryIndex = 3;
        }
    }
    
    public void InventoryRight(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        currentInventoryIndex++;
        if (currentInventoryIndex > 3)
        {
            currentInventoryIndex = 0;
        }
    }

    public void InventoryUse(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        Players[CurrentPlayerIndex].GetComponent<Player>().UseItem(currentInventoryIndex);
    }

    // Helpers
    
    // Getter for current player
    
    public GameObject GetCurrentPlayer()
    {
        return Players[CurrentPlayerIndex];
    } // GetCurrentPlayer

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    } // ReturnToMainMenu
    
} // GameManager