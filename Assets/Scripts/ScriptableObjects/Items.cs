using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Items : ScriptableObject
{
    public string itemDescription; // describe item effect on screen
    public string itemName; // print item name to screen
} // ScriptableObject