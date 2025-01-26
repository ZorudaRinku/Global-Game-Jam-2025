using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerTemplate playerNumber;
    public bool alive;
    public int number;
    private int _currentPlayerSentObjects;
    [SerializeField] private GameObject throwObject;
    private GameObject[] _inventoryGameObjects = new GameObject[4];
    private GameObject[] _inventory = new GameObject[4];

    void Start()
    {
        alive = playerNumber.PlayerAlive;
        number = playerNumber.playerNumber;
        for (int i = 0; i < 4; i++)
        {
            _inventoryGameObjects[i] = transform.GetChild(3).GetChild(i).gameObject;
        }
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
        if (_currentPlayerSentObjects == 0) return false; // Player should not be able to end their turn without throwing an object
        _currentPlayerSentObjects = 0;

        Debug.Log($"{transform.name} Ended Turn");
        return true;
    } // EndTurn

    public void UseItem(int index)
    {
        if (_inventory[index] == null) return; // Player should not be able to use an item that does not exist
        //_inventory[index].GetComponent<Item>().UseItem();
        _inventory[index] = null;
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        // Move the inventory game objects to the correct position (i.e. if the first item in _inventory is null, move the 2nd inventory game object to the first position, etc)
        for (int inventoryIndex = 0; inventoryIndex < 4; inventoryIndex++)
        {
            if (_inventory[inventoryIndex] != null) continue;
            
            for (int inventoryLookAheadIndex = inventoryIndex + 1; inventoryLookAheadIndex < _inventory.Length; inventoryLookAheadIndex++)
            {
                if (_inventory[inventoryLookAheadIndex] != null)
                {
                    _inventory[inventoryIndex] = _inventory[inventoryLookAheadIndex];
                    _inventory[inventoryLookAheadIndex] = null;
                    _inventoryGameObjects[inventoryIndex].transform.position = _inventoryGameObjects[inventoryLookAheadIndex].transform.position;
                    _inventoryGameObjects[inventoryLookAheadIndex].transform.position = transform.GetChild(3).GetChild(3).position;
                    break;
                }
            }
        }
    }

} // Player
