using UnityEngine.UI;
using UnityEngine;

public class Foresight : MonoBehaviour
{
    [SerializeField] public Items itemType;
    [SerializeField] public GameObject GameManager;
    private Animator textAnimator;
    private Text itemName;
    private Text itemDescription;
    private Text foresightText;

    public void Start()
    {
        // initialize text fade animator
        textAnimator = GetComponent<Animator>();
        // initialize canvas text values
        itemName.text = itemType.itemName;
        itemDescription.text = itemType.itemDescription;
    } // Start

    public void UseItem()
    {
        // set the text for foresight
        foresightText.text = GenerateForesightText();

        // play text fade in animation
        textAnimator.SetTrigger("TriggerFadeIn");

        Destroy(this);
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
} // ForeSight
