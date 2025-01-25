using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = gameManager.GetCurrentPlayer().transform.position;
    }
}
