using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Event handler for generation algorithms.
 */ 
public class GenerationController : MonoBehaviour
{
    /*RNG Seed to be used for all generation processes*/
    public int Seed = 1;

    public GenerationSequence[] GenerationSequences;

    /*Static instance of Generation Controller*/
    public static GenerationController instance = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartGenerationSequences();
    }

    public void StartGenerationSequences()
    {
        Random.InitState(Seed);
        foreach (GenerationSequence sequence in GenerationSequences)
        {
            if (!sequence.IsReady())
            {
                sequence.Setup();
            }
            sequence.Generate();
        }
    }

   
}
