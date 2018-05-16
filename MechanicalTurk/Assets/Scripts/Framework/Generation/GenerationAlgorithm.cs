using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Generation
{

    /// Wrapper for various generation algorithms that adds events
    public abstract class GenerationAlgorithm : MonoBehaviour
    {
        /// <summary>
        /// Unity Event dispatcher that triggers when Generate has finished.
        /// </summary>
        public UnityEvent OnGenerationComplete = new UnityEvent();

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

        /// <summary>
        /// Clears the results of a previous generation.
        /// </summary>
        public virtual void Clean()
        {

        }

        /// <summary>
        /// Removes all event listeners from #OnGenerationComplete
        /// </summary>
        void OnDisable()
        {
            OnGenerationComplete.RemoveAllListeners();
        }

    }

}