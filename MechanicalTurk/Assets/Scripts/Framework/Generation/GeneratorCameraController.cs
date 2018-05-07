using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Generation;
using CameraControls;

namespace Framework.Generation
{

    public class GeneratorCameraController : CameraController
    {

        public GenerationController[] generationControllers;
        public Vector3 viewOffset = new Vector3(0f, 128f, 0f);
        public Vector3 viewCenterOffset = new Vector3(128, 0f, 128f);
        private int numReadyGenerators = 0;

        public virtual void AddAllGenControllersInScene()
        {
            generationControllers = FindObjectsOfType<GenerationController>();
            if(generationControllers == null)
            {
                generationControllers = new GenerationController[cameras.Length];
            }
            if(generationControllers.Length != cameras.Length)
            {
                Array.Resize(ref generationControllers, cameras.Length);
            }
        }

        public virtual void AlignCamerasWithGenerators()
        {
            for(int i = 0; i < cameras.Length && i < generationControllers.Length; i++)
            {
                if (generationControllers[i] == null) continue;
                AlignCameraWithTarget(i, viewOffset, generationControllers[i].transform.position + viewCenterOffset);
            }
        }

        public virtual void AddGenerationController(int viewCameraId, GenerationController controller)
        {
            if(generationControllers == null || generationControllers.Length < viewCameraId)
            {
                Array.Resize(ref generationControllers, cameras.Length);
                generationControllers[viewCameraId] = controller;
            }
        }

        /// <summary>
        /// Disables all circular view sequences before starting.
        /// Once each sequence is complete, GenerateAndRestartSequence is called.
        /// </summary>
        public override void StartSequences()
        {
            MakeSequencesFinite();
            OnViewSequenceComplete.AddListener(GenerateAndRestartSequence);
            RestartSequences();
        }

        /// <summary>
        /// Finds the GenerationController associated with targetCamera and starts the next generation sequence.
        /// The GenerationController's seed is incremented by one.
        /// </summary>
        /// <param name="targetCamera"></param>
        /// <param name="viewTarget"></param>
        public virtual void GenerateAndRestartSequence(Camera targetCamera, CameraViewTarget viewTarget)
        {
            if(numReadyGenerators < 0)
            {
                numReadyGenerators = 0;
            }

            int i = Array.IndexOf(cameras, targetCamera);
            generationControllers[i].Seed++;
            generationControllers[i].cityGenerator.OnGenerationComplete.AddListener(UpdateReadyGenerators);
            generationControllers[i].SetupAndGenerate();
        }

        ///Updates the number of generators that have finished. Once all are done, the view sequence restarts
        public virtual void UpdateReadyGenerators()
        {
            numReadyGenerators++;
            if(numReadyGenerators>=generationControllers.Length)
            {
                for(int i = 0; i < generationControllers.Length; i++)
                {
                    generationControllers[i].cityGenerator.OnGenerationComplete.RemoveListener(UpdateReadyGenerators);
                }
                numReadyGenerators = 0;
                RestartSequences();
            }
        }
    }


}