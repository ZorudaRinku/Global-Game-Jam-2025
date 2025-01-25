using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    private Vector3 _startPos;
    private GameObject _cup;
    private float _startTime;
    [SerializeField] private float travelDuration = 2.0f; // Duration in seconds

    [SerializeField] private float maxScaleMultiplier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the start position of the object
        _startPos = transform.position;
        
        // Find the cup object
        _cup = GameObject.Find("Cup");
        
        // Record the start time
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards the cup with a constant duration
        float t = (Time.time - _startTime) / travelDuration;
        transform.position = Vector3.Lerp(_startPos, _cup.transform.position, t);
        // Depending on the distance between the startPos and the cup, make the image slightly larger in the in the middle to simulate it getting closer to the camera
        
        // Calculate the midpoint between the start position and the cup
        Vector3 midpoint = (_startPos + _cup.transform.position) / 2;

        // Calculate the distance to the midpoint and the cup
        float distanceToMidpoint = Vector3.Distance(transform.position, midpoint);
        float distanceToCup = Vector3.Distance(transform.position, _cup.transform.position);
        float totalDistance = Vector3.Distance(_startPos, _cup.transform.position);

        // Adjust the scale based on the distance to the midpoint and the cup
        float scaleMultiplier = 1.0f + (1.0f - (distanceToMidpoint / (totalDistance / 2))) * maxScaleMultiplier;
        if (distanceToCup < totalDistance / 2)
        {
            scaleMultiplier = 1.0f + (distanceToCup / (totalDistance / 2)) * maxScaleMultiplier;
        }
        transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
        
        // Destroy if we've arrived to the cup
        if (distanceToCup < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
