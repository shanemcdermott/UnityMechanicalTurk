using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// Wrapper for various generation algorithms that adds events
public abstract class GenerationAlgorithm : MonoBehaviour
{
    
    public UnityEvent OnGenerationComplete;

    void Awake()
    {
        if (OnGenerationComplete == null)
        {
            OnGenerationComplete = new UnityEvent();
        }
    }

    /// <summary>
    /// Checks to see if generation prerequisites are met.
    /// </summary>
    /// <returns>true if ready for generation.</returns>
    public abstract bool CanGenerate();

    
    /// <summary>
    /// Collects any prerequisites for generation.
    /// </summary>
    public abstract void Setup();

    /// <summary>
    /// Invokes OnGenerationComplete when finished.
    /// </summary>
    public virtual void Generate(bool ShouldInvokeOnComplete)
    {
        Generate();
        if (ShouldInvokeOnComplete)
        {
            OnGenerationComplete.Invoke();
        }
    }

    public abstract void Generate();


    void OnDisable()
    {
        OnGenerationComplete.RemoveAllListeners();
    }
	
}
