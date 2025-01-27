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
    private GameObject GameManager;
    private int _currentPlayerSentObjects;
    [SerializeField] private GameObject throwObject;
    [SerializeField] private GameObject[] itemContainers;
    private InventorySlot[] inventorySlots = new InventorySlot[4];
    private bool doublerPending;
    [SerializeField] private Sprite[] pileSprites; 
    [SerializeField] private GameObject cup;

    void Start()
    {
        // initialization phase
        alive = playerNumber.PlayerAlive;
        number = playerNumber.playerNumber;
        GameManager = GameObject.Find("GameManager");
        doublerPending = false;
        UpdateInventory();
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
        // if player was hit with a doubler, it resets their turn when they end turn and clears doubler status
        if (doublerPending)
        {
            doublerPending = false;
            _currentPlayerSentObjects = 0;
            return false;
        }

        if (_currentPlayerSentObjects == 0) return false; // Player should not be able to end their turn without throwing an object
        _currentPlayerSentObjects = 0;
        UpdatePile();
        Debug.Log($"{transform.name} Ended Turn");

        return true;
    } // EndTurn

    public bool UseItem(int index)
    {
        Debug.Log("Using item at index: " + index);
        if (inventorySlots[index] == null) return false; // Player should not be able to use an item that does not exist
        
        // prevent item use if a cancel item has previously been used
        if (GameManager.GetComponent<GameManager>().GetCancelPending())
        {
            Debug.Log("Item use cancelled");
            RemoveItemFromInventory(index);
            GameManager.GetComponent<GameManager>().SetCancelPending(false);
            return false;
        }

        // use item interface
        inventorySlots[index].item.GetComponent<IItem>().UseItem();

        // destroy item after use and sort inventory
        RemoveItemFromInventory(index);
        UpdateInventory();
        return true;
    } // UseItem

    public void UpdateInventory()
    {
        /*
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
        */

        for (int i = 0; i < 4; i++)
        {
            Color color = itemContainers[i].GetComponent<Image>().color;
            color.a = 0;
            itemContainers[i].GetComponent<Image>().color = color;
        }

    } // UpdateInventory

    public void AddItemToInventory(GameObject itemToAdd)
    {
        Debug.Log("Adding " + itemToAdd + " to " + playerNumber);
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // check for empty inventory slots
            if (inventorySlots[i] == null)
            {
                InventorySlot tempItem = new InventorySlot(Instantiate(itemToAdd, transform.GetChild(3).GetChild(i).position, Quaternion.identity, transform.GetChild(3).GetChild(i)));
                inventorySlots[i] = tempItem;
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
        if (inventorySlots[index] != null)
        {
            Destroy(inventorySlots[index].item);
            inventorySlots[index] = null;
        }
    } // RemoveItemFromInventory

    public bool GetDoublerPending()
    {
        return doublerPending;
    } // GetDoublerPending

    public string GetItemName(int index)
    {
        string itemName;
        if (inventorySlots[index] == null) itemName = "No Item";
        else itemName = inventorySlots[index].item.GetComponent<IItem>().GetItemName();
        return itemName;
    } // GetItemName

    public string GetItemDescription(int index)
    {
        string itemDescription;
        if (inventorySlots[index] == null) itemDescription = "No Description";
        else itemDescription = inventorySlots[index].item.GetComponent<IItem>().GetItemDescription();
        return itemDescription;
    } // GetItemName

} // Player

public class InventorySlot
{
    public GameObject item;

    public InventorySlot(GameObject _item)
    {
        item = _item;
    } // Constructor


} // InventorySlot