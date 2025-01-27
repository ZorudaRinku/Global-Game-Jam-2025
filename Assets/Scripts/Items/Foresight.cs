using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Foresight : MonoBehaviour, IItem
{
    [SerializeField] public Items itemType;
    private GameObject _gameManager;
    private TMP_Text _headerText;
    private Animator _headerTextAnimator;
    private string itemName;
    private string itemDescription;

    public void Start()
    {
        // initialization phase
        _headerTextAnimator = GameObject.Find("HeaderText").GetComponent<Animator>();
        _headerText = GameObject.Find("HeaderText").GetComponent<TextMeshProUGUI>();
        _gameManager = GameObject.Find("GameManager");
        itemName = itemType.itemName;
        itemDescription = itemType.itemDescription;
    } // Start

    public void UseItem()
    {
        // set the text for foresight
        _headerText.text = GenerateForesightText();

        // play text fade in animation
        _headerTextAnimator.SetTrigger("TriggerFadeIn");
    } // UseItem

    // determines how close the bubble is to popping
    public string GenerateForesightText()
    {
        // initialization
        string textToPrint;

        // if bubble is not going to pop on next hit
        if (_gameManager.GetComponent<GameManager>().GetBubbleStatus() > 1)
        {
            textToPrint = "Bubble will not pop on next throw.";
        }
        else
        {
            textToPrint = "Bubble is about to pop!";
        }

        return textToPrint;
    } // GenerateForesightText

    // triggered when FadeInText animation completes
    public void OnAnimationFinish()
    {
        //Destroy(this);
    } // OnAnimationFinish

    public string GetItemName()
    {
        return itemName;
    } // getItemName

    public string GetItemDescription()
    {
        return itemDescription;
    } // getItemDescription
} // ForeSight