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
        return;
    } // UseItem

    public string getItemName()
    {
        return itemName;
    } // getItemName

    public string getItemDescription()
    {
        return itemDescription;
    } // getItemDescription
}
