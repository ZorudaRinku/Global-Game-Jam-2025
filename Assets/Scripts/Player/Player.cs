using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerTemplate playerNumber;
    public bool alive;
    public int number;
    private int _currentPlayerSentObjects;
    [SerializeField] private GameObject throwObject;
    private GameObject[] _inventoryGameObjects = new GameObject[4];
    private GameObject[] _inventory = new GameObject[4];
    [SerializeField] private Sprite[] pileSprites; 
    [SerializeField] private GameObject cup;

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
        Vector3 spawnPoint = transform.GetChild(2).TransformPoint(new Vector3(0, transform.GetChild(3).GetComponent<RectTransform>().rect.height, 0));
        GameObject throwable = Instantiate(throwObject, spawnPoint, Quaternion.identity, cup.transform); // Spawn throwable object at player's hand
        throwable.transform.SetSiblingIndex(1); // Set the sibling index to 0 to place it at the top of the hierarchy
        Debug.Log($"{transform.name} Threw Object");
        _currentPlayerSentObjects++;
        UpdatePile();
        return throwable;
    } // ThrowObject

    private void UpdatePile()
    {
        Image pile = transform.GetChild(2).GetComponent<Image>();
        pile.sprite = pileSprites[Mathf.Clamp(4 - _currentPlayerSentObjects / 2, 0, 4)];
    }
    
    // End Turn
    public bool EndTurn()
    {
        if (_currentPlayerSentObjects == 0) return false; // Player should not be able to end their turn without throwing an object
        _currentPlayerSentObjects = 0;
        UpdatePile();
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
