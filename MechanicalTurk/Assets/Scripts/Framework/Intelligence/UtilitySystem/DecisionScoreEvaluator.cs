using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Intelligence.UtilitySystem
{

    public class DecisionScoreEvaluator : MonoBehaviour
    {
        public string description;
        public float weight;
        public Consideration[] considerations;

        public float Score(DecisionContext context, float bonus, float min)
        {
            float finalScore = weight * bonus;
            foreach(Consideration consideration in considerations)
            {
                if ((0f > finalScore) || (finalScore < min)) break;

                float score = consideration.Score(context);
                float response = consideration.ComputeResponseCurve(score);

                finalScore *= Mathf.Clamp(response, 0f, 1f);
            }

            return Compensate(finalScore);
        }

        protected float Compensate(float originalScore)
        {
            float modificationFactor = 1f - (1f / considerations.Length);
            float makeupValue = (1 - originalScore) * modificationFactor;
            return originalScore + (makeupValue * originalScore);
        }
    }

}