using UnityEngine;

public class Equalizer : MonoBehaviour, IItem
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
