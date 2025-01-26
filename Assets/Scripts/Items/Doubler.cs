using UnityEngine;

public class Doubler : MonoBehaviour, IItem
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
        // set doublerPending status on upcoming player
        GameManager.GetComponent<GameManager>().Players[GameManager.GetComponent<GameManager>().GetNextPlayerIndex()].GetComponent<Player>().SetDoublerPending(true);
        Destroy(this);
    } // UseItem

    public string GetItemName()
    {
        return itemName;
    } // getItemName

    public string GetItemDescription()
    {
        return itemDescription;
    } // getItemDescription
} // Doubler