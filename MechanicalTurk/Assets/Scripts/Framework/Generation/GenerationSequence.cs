using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationSequence : GenerationAlgorithm
{
    public List<GenerationAlgorithm> generationSequence = new List<GenerationAlgorithm>();
    public int seqIndex;

    public GenerationAlgorithm GetCurrentAlgorithm()
    {
         if (seqIndex >= generationSequence.Count)
               return null;

         return generationSequence[seqIndex];
    }

    public override void SetController(GenerationController controller)
    {
        base.SetController(controller);
        for(int i = 0; i < generationSequence.Count; i++)
        {
            generationSequence[i].Controller = controller;
        }
    }

    public override bool CanGenerate()
    {
        return generationSequence.Count > 0 && seqIndex == 0;
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
