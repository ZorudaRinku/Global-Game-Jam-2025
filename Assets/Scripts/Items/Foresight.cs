using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Foresight : MonoBehaviour, IItem
{
    [SerializeField] public Items itemType;
    [SerializeField] public GameObject GameManager;
    private TMP_Text foresightText;
    private Animator textAnimator;
    private string itemName;
    private string itemDescription;
    

    public void Start()
    {
        // initialize text fade animator
        textAnimator = GetComponent<Animator>();
        // initialize TMP element
        foresightText = GetComponent<TMP_Text>();
        // initialize canvas text values
        itemName = itemType.itemName;
        itemDescription = itemType.itemDescription;
    } // Start

    public void UseItem()
    {
        Debug.Log("Using Foresight");
        // set the text for foresight
        foresightText.text = GenerateForesightText();

        // play text fade in animation
        textAnimator.SetTrigger("TriggerFadeIn");

    } // UseItem

    // determines how close the bubble is to popping
    public string GenerateForesightText()
    {
        
        // intialization
        string textToPrint;

        // if bubble is not going to pop on next hit
        if (GameManager.GetComponent<GameManager>().GetBubbleStatus() > 0)
        {
            textToPrint = "Bubble will not pop on next throw.";
        }
        else
        {
            textToPrint = "Bubble is about to pop!";
        }

        return textToPrint;
        
    } // GetBubbleStatus

    public void OnAnimationFinish()
    {
        Destroy(this);
    } // OnAnimationFinish

    public string getItemName()
    {
        return itemName;
    } // getItemName

    public string getItemDescription()
    {
        return itemDescription;
    } // getItemDescription
} // ForeSight
