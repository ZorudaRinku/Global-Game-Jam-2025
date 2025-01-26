using UnityEngine;

public class Skip : MonoBehaviour, IItem
{
    [SerializeField] public Items itemType;
    [SerializeField] public GameObject GameManager;
    private string itemDescription;
    private string itemName;

    public void Start()
    {
        // initialize canvas text values
        itemName = itemType.itemName;
        itemDescription = itemType.itemDescription;
    } // Start

    public void UseItem()
    {
        //GameManager.GetComponent<GameManager>().Players[GameManager.GetComponent<GameManager>().GetCurrentPlayerIndex()].SetSkipItemUsed();
        //GameManager.GetComponent<GameManager>().EndTurn();
    } // UseItem

    public string getItemName()
    {
        return itemName;
    } // getItemName

    public string getItemDescription()
    {
        return itemDescription;
    } // getItemDescription
} // Skip
