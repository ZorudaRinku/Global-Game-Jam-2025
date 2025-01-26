using UnityEngine;

public class Skip : MonoBehaviour, IItem
{
    [SerializeField] public Items itemType;
    private GameObject GameManager;
    private string itemDescription;
    private string itemName;

    public void Start()
    {
        // initialize canvas text values
        itemName = itemType.itemName;
        itemDescription = itemType.itemDescription;
        GameManager = GameObject.Find("GameManager");
    } // Start

    public void UseItem()
    {
        // if player has doublerPending status, skip only ends one of their turns
        if (GameManager.GetComponent<GameManager>().Players[GameManager.GetComponent<GameManager>().GetCurrentPlayerIndex()].GetComponent<Player>().GetDoublerPending())
        {
            GameManager.GetComponent<GameManager>().Players[GameManager.GetComponent<GameManager>().GetCurrentPlayerIndex()].GetComponent<Player>().SetDoublerPending(false);
        }

        // proceed to next player and destroy this item
        GameManager.GetComponent<GameManager>().MoveToNextPlayer();
        Destroy(this);
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
