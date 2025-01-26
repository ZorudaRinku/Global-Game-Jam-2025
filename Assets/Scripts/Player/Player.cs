using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerTemplate playerNumber;
    public bool alive;
    public int number;
    private GameObject GameManager;
    private int _currentPlayerSentObjects;
    [SerializeField] private GameObject throwObject;
    private InventorySlot[] inventorySlots = new InventorySlot[4];
    private bool doublerPending;

    void Start()
    {
        // initialization phase
        alive = playerNumber.PlayerAlive;
        number = playerNumber.playerNumber;
        GameManager = GameObject.Find("GameManager");
        doublerPending = false;

        /*
        for (int i = 0; i < 4; i++)
        {
            _inventoryGameObjects[i] = transform.GetChild(3).GetChild(i).gameObject;
        }
        */
    } // Start

    // instantiate and throw the throwable object
    public GameObject ThrowObject()
    {
        GameObject throwable = Instantiate(throwObject, transform.GetChild(2).transform.position, Quaternion.identity); // Spawn throwable object at player's hand
        throwable.transform.SetParent(transform.parent); // Set the throwable object as a child of the UI or else it will not be visible
        Debug.Log($"{transform.name} Threw Object");
        _currentPlayerSentObjects++;
        return throwable;
    } // ThrowObject
    
    // End Turn
    public bool EndTurn()
    {
        // if player was hit with a doubler, it resets their turn when they end turn and clears doubler status
        if (doublerPending)
        {
            doublerPending = false;
            _currentPlayerSentObjects = 0;
            return false;
        }

        if (_currentPlayerSentObjects == 0) return false; // Player should not be able to end their turn without throwing an object
        _currentPlayerSentObjects = 0;
        Debug.Log($"{transform.name} Ended Turn");

        return true;
    } // EndTurn

    public void UseItem(int index)
    {
        Debug.Log("Using item at index: " + index);
        if (inventorySlots[index] == null) return; // Player should not be able to use an item that does not exist
        
        // prevent item use if a cancel item has previously been used
        if (GameManager.GetComponent<GameManager>().GetCancelPending())
        {
            Debug.Log("Item use cancelled");
            RemoveItemFromInventory(index);
            GameManager.GetComponent<GameManager>().SetCancelPending(false);
            return;
        }

        // use item interface
        inventorySlots[index].item.GetComponent<IItem>().UseItem();

        // destroy item after use and sort inventory
        RemoveItemFromInventory(index);
        UpdateInventory();
    } // UseItem

    public void UpdateInventory()
    {
        // Move the inventory game objects to the correct position (i.e. if the first item in _inventory is null, move the 2nd inventory game object to the first position, etc)
        for (int inventoryIndex = 0; inventoryIndex < 4; inventoryIndex++)
        {
            if (inventorySlots[inventoryIndex] != null) continue;
            
            for (int inventoryLookAheadIndex = inventoryIndex + 1; inventoryLookAheadIndex < inventorySlots.Length; inventoryLookAheadIndex++)
            {
                if (inventorySlots[inventoryLookAheadIndex] != null)
                {
                    inventorySlots[inventoryIndex] = inventorySlots[inventoryLookAheadIndex];
                    inventorySlots[inventoryLookAheadIndex] = null;
                    //_inventoryGameObjects[inventoryIndex].transform.position = _inventoryGameObjects[inventoryLookAheadIndex].transform.position;
                    //_inventoryGameObjects[inventoryLookAheadIndex].transform.position = transform.GetChild(3).GetChild(3).position;
                    break;
                }
            }
        }
    } // UpdateInventory

    public void AddItemToInventory(GameObject itemToAdd)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // check for empty inventory slots
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = new InventorySlot(Instantiate(itemToAdd, Vector3.zero, Quaternion.identity, transform));
                break;
            }
        }
    } // AddItemToInventory

    public void SetDoublerPending(bool value)
    {
        Debug.Log("Doubler status set for player " + playerNumber);
        doublerPending = value;
    } // SetDoublerPending

    public void RemoveItemFromInventory(int index)
    {
        if (inventorySlots[index] != null) inventorySlots[index] = null;
    } // RemoveItemFromInventory

    public bool GetDoublerPending()
    {
        return doublerPending;
    } // GetDoublerPending

} // Player

public class InventorySlot
{
    public GameObject item;

    public InventorySlot(GameObject _item)
    {
        item = _item;
    } // Constructor


} // InventorySlot