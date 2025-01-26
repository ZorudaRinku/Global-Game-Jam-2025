using System;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public event Action OnBubblePopStarted;
    public event Action OnBubblePopFinished;
    public event Action OnBubbleResetStarted;
    public event Action OnBubbleResetFinished;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void BubblePopStarted()
    {
        OnBubblePopStarted?.Invoke();
    }

    private void BubblePopFinished()
    {
        OnBubblePopFinished?.Invoke();
    }
    
    private void BubbleResetStarted()
    {
        OnBubbleResetStarted?.Invoke();
    }
    
    private void BubbleResetFinished()
    {
        OnBubbleResetFinished?.Invoke();
    }
}
