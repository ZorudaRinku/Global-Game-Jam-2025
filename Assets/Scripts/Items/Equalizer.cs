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
        if (itemName == null) // Unfortunately, this method gets called before Start() is called, so we need to check if itemName is null
        {
            itemName = itemType.itemName;
        }
        return itemName;
    } // getItemName

    public string GetItemDescription()
    {
        if (itemDescription == null) // Unfortunately, this method gets called before Start() is called, so we need to check if itemDescription is null
        {
            itemDescription = itemType.itemDescription;
        }
        return itemDescription;
    } // getItemDescription
}
