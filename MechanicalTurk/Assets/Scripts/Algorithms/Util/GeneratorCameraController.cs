using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Generation;
using Framework.Util;

namespace Algorithms.Util
{

    public class GeneratorCameraController : CameraController
    {

        public GenerationController[] generationControllers;

        public override void StartSequences()
        {
            MakeSequencesFinite();

            RestartSequences();
        }

        public override void RestartSequences()
        {
            for(int i = 0; i < generationControllers.Length; i++)
            {
                generationControllers[i].cityGenerator.OnGenerationComplete.RemoveListener(RestartSequences);
            }
            CameraView lastView = viewSequences[0].GetLast();
            lastView.OnLerpViewComplete.AddListener(GenerateAndRestartSequences);
            base.RestartSequences();
        }

        public virtual void GenerateAndRestartSequences()
        {
            for(int i = 0; i < generationControllers.Length; i++)
            {
                generationControllers[i].Seed++;
                generationControllers[i].cityGenerator.OnGenerationComplete.AddListener(RestartSequences);
                generationControllers[i].SetupAndGenerate();
            }
        }
    }


}