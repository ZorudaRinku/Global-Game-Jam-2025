using UnityEngine;

public class Equalizer : MonoBehaviour, IItem
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
        // trigger equalizer action in game manager
        GameManager.GetComponent<GameManager>().RemoveItemsFromAllPlayers();
    } // UseItem

    public string GetItemName()
    {
        return itemName;
    } // getItemName

    public string GetItemDescription()
    {
        return itemDescription;
    } // getItemDescription
}
