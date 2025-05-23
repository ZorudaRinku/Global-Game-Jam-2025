using UnityEngine;

public class Reverse : MonoBehaviour, IItem
{
    [SerializeField] public Items itemType;
    private GameObject GameManager;
    private string itemDescription;
    private string itemName;

    public void Start()
    {
        // initialize canvas text values
        itemName = itemType.itemName;
        Debug.Log("Item Name: " + itemName);
        itemDescription = itemType.itemDescription;
        GameManager = GameObject.Find("GameManager");
    } // Start

    // gets current turn order, and sets the opposite turn order in place
    public void UseItem()
    {
        Debug.Log("Turn Order reversed!");
        GameManager.GetComponent<GameManager>().SetTurnOrderSetClockwise(!GameManager.GetComponent<GameManager>().GetTurnOrderSetClockwise());
    } // UseItem

    public string GetItemName()
    {
        if (itemName == null)
        {
            itemName = itemType.itemName;
        }
        return itemName;
    } // getItemName

    public string GetItemDescription()
    {
        if (itemDescription == null)
        {
            itemDescription = itemType.itemDescription;
        }
        return itemDescription;
    } // getItemDescription

} // Reverse
