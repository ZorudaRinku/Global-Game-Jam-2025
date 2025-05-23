using UnityEngine;

public class Cancel : MonoBehaviour, IItem
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
        GameManager.GetComponent<GameManager>().SetCancelPending(true);
        Destroy(this);
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
}
