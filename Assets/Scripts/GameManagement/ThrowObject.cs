using System;
using UnityEngine;
using UnityEngine.Events;

public class ThrowObject : MonoBehaviour
{
    private Vector3 _startPos;
    private GameObject _bubbleGameObject;
    private float _startTime;
    [SerializeField] private float travelDuration = 2.0f; // Duration in seconds
    
    public event Action OnThrowObjectHitBubble;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the start position of the object
        _startPos = transform.position;
        
        // Find the cup object
        _bubbleGameObject = GameObject.Find("Bubble");
        
        // Record the start time
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards the cup with a constant duration
        float t = (Time.time - _startTime) / travelDuration;
        transform.position = Vector3.Lerp(_startPos, _bubbleGameObject.transform.position, t);
        
        float distanceToCup = Vector3.Distance(transform.position, _bubbleGameObject.transform.position);
        // Destroy if we've arrived to the cup
        if (distanceToCup < 0.1f)
        {
            OnThrowObjectHitBubble?.Invoke();
            Destroy(gameObject);
        }
    }
}
