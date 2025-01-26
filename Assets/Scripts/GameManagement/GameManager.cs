using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine.UI;
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
    public List<GameObject> Players;
    public int CurrentPlayerIndex { get; private set; }
    [FormerlySerializedAs("_players")] [SerializeField] private GameObject[] playerObjects;
    [FormerlySerializedAs("playertemplates")] [SerializeField] private PlayerTemplate[] playerTemplates;
    [FormerlySerializedAs("itemobjects")][SerializeField] private GameObject[] itemObjects;
    [FormerlySerializedAs("UI")] [SerializeField] private GameObject ui;
    [SerializeField] private GameObject throwObject;
    [SerializeField] private GameObject bubble;
    private Animator _bubbleAnimator;
    private bool cancelPending;
    private bool turnOrderSetClockwise;
    
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
        // initialization phase
        _bubblePopThreshold = Random.Range(2, 10);
        _bubbleAnimator = bubble.GetComponent<Animator>();
        cancelPending = false;
        turnOrderSetClockwise = false;
        Players = new List<GameObject>();

        // clears living states from previous game
        for (int i = 0; i < 4; i++)
        {
            playerTemplates[i].PlayerAlive = false;
        }

        // enables however many players were selected in main menu
        for (int i = 0; i < numPlayers.numberOfPlayers; i++)
        {
            playerTemplates[i].PlayerAlive = true;
            playerObjects[i].SetActive(true);
            Players.Add(playerObjects[i]);
            Players[i].GetComponent<Player>().AddItemToInventory(itemObjects[0]);
        }

        CurrentGameState = GameState.Game; // TODO: Change to Lobby when Menu is implemented
        CurrentRoundState = RoundState.InProgress;

        // give starting items to players
        GiveItemsToPlayers();
        
        UpdateInventoryUI();
        UpdateAnimators();
        
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
        GiveItemsToPlayers();
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
        }
    } // EndTurn

    public void MoveToNextPlayer() // Move on to the next player
    {
        ResetInventoryUI();
        CurrentPlayerIndex = GetNextPlayerIndex();
        UpdateInventoryUI();
        UpdateAnimators();
    } // MoveToNextPlayer
    
    private void UpdateAnimators()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].transform.GetChild(1).GetComponent<Animator>().SetBool("Active", i == CurrentPlayerIndex);
        }
    } // UpdateAnimators

    private void ResetInventoryUI()
    {
        for (int i = 0; i < 4; i++)
        {
            Players[CurrentPlayerIndex].transform.GetChild(3).GetChild(i).localScale = new Vector3(1, 1, 1);
        }
    } // ResetInventoryUI

    private void UpdateInventoryUI()   
    {
        ResetInventoryUI();
        Players[CurrentPlayerIndex].transform.GetChild(3).GetChild(currentInventoryIndex).localScale = new Vector3(1.2f, 1.2f, 1.2f);
    } // UpdateInventoryUI

    public void InventoryLeft(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        Debug.Log("Inventory Index pre-decrement: " + currentInventoryIndex);
        currentInventoryIndex--;
        Debug.Log("Inventory Index set to: " + currentInventoryIndex);
        if (currentInventoryIndex < 0)
        {
            currentInventoryIndex = 3;
        }
        UpdateInventoryUI();
    } // InventoryLeft
    
    public void InventoryRight(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        currentInventoryIndex++;
        if (currentInventoryIndex > 3)
        {
            currentInventoryIndex = 0;
        }
        UpdateInventoryUI();
    } // InventoryRight

    public void InventoryUse(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return; // Prevents Input Manager from calling this method multiple times
        
        Players[CurrentPlayerIndex].GetComponent<Player>().UseItem(currentInventoryIndex);
        
        UpdateInventoryUI();
    } // InventoryUse

    // Grants two random items to each player that is alive
    public void GiveItemsToPlayers()
    {
        // loop through players, only granting items to players that are alive
        for (int i = 0; i < numPlayers.numberOfPlayers; i++)
        {
            if (Players[i].GetComponent<Player>().alive)
            {
                // give two random items to current player
                Players[i].GetComponent<Player>().AddItemToInventory(itemObjects[Random.Range(0, itemObjects.Length - 1)]);
                Players[i].GetComponent<Player>().AddItemToInventory(itemObjects[Random.Range(0, itemObjects.Length - 1)]);
            }
        }
    } // GiveItemsToPlayers

    // Removes all inventory items from each player
    public void RemoveItemsFromAllPlayers()
    {
        // loop through each player
        for (int i = 0; i < numPlayers.numberOfPlayers; i++)
        {
            // verify player is alive before making changes
            if (Players[i].GetComponent<Player>().alive)
            {
                // loop through each inventory slot, and delete each item
                for (int j = 0; j < 4; j++)
                {
                    Players[i].GetComponent<Player>().RemoveItemFromInventory(j);
                }
            }
        }
        // give two items back to players. 
        GiveItemsToPlayers();
    } // RemoveItemsFromAllPlayers

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    } // ReturnToMainMenu

    public void SetCancelPending(bool value)
    {
        cancelPending = value;
    } // SetCancelPending

    public void SetTurnOrderSetClockwise(bool value)
    {
        turnOrderSetClockwise = value;
    } // SetTurnOrderSetClockwise

    public GameObject GetCurrentPlayer()
    {
        return Players[CurrentPlayerIndex];
    } // GetCurrentPlayer

    public int GetBubbleStatus()
    {
        return _bubblePopThreshold - _bubblePopCount;
    } // GetBubbleStatus

    public int GetCurrentPlayerIndex()
    {
        return CurrentPlayerIndex;
    } // CurrentPlayerIndex

    public int GetNextPlayerIndex()
    {
        // Increment the current player index, skip dead players, and loop back to the first player if necessary
        int tempIndex = CurrentPlayerIndex;
        do
        {
            // handles turn order direction
            if (!turnOrderSetClockwise) tempIndex++;
            else tempIndex--;

            if (tempIndex >= playerObjects.Length)
            {
                tempIndex = 0;
            } else if (tempIndex < 0)
            {
                tempIndex = playerObjects.Length - 1;
            }
        } while (!Players[tempIndex].GetComponent<Player>().alive);

        return tempIndex;
    } // GetNextPlayerIndex

    public bool GetCancelPending()
    {
        return cancelPending;
    } // GetCancelPending
    
    public bool GetTurnOrderSetClockwise()
    {
        return turnOrderSetClockwise;
    } // GetTurnOrderSetClockwise

} // GameManager