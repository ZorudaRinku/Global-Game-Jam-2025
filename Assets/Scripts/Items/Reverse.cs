using UnityEngine;

public class Reverse : MonoBehaviour
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

    // gets current turn order, and sets the opposite turn order in place
    public void UseItem()
    {
        Debug.Log("Turn Order reversed!");
        GameManager.GetComponent<GameManager>().SetTurnOrderSetClockwise(!GameManager.GetComponent<GameManager>().GetTurnOrderSetClockwise());
    } // UseItem

} // Reverse
