using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = gameManager.GetCurrentPlayer().transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = gameManager.GetCurrentPlayer().transform.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
    }
}
