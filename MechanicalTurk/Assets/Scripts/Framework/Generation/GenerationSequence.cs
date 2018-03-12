using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationSequence : GenerationAlgorithm
{
    public GenerationAlgorithm[] generationSequence;
    public int seqIndex;

    public GenerationAlgorithm GetCurrentAlgorithm()
    {
         if (seqIndex >= generationSequence.Length)
               return null;

         return generationSequence[seqIndex];
    }

    public override bool CanGenerate()
    {
        return generationSequence.Length > 0 && seqIndex == 0;
    }

    public override void Setup()
    {
        seqIndex = 0;
        GetCurrentAlgorithm().Setup();
    }

    /// <summary>
    /// Increments @seqIndex and calls StartCurrentAlgorithm
    /// </summary>
    public virtual void StartNextAlgorithm()
    {
        seqIndex++;
        Generate();
    }

    public override void Generate()
    {
        GenerationAlgorithm alg = GetCurrentAlgorithm();
        if (alg)
        {
            alg.OnGenerationComplete.AddListener(StartNextAlgorithm);
            alg.Setup();
            if (alg.CanGenerate())
            {
                alg.Generate(true);
            }
        }
    }

}
