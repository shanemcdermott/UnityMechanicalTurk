using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilitySystem
{

    public class DecisionMaker : MonoBehaviour
    {
        List<Decision> decisions;
        DecisionContext last;

        public void PerformBestDecision()
        {
            Decision decision = GetBestDecision();
            last = decision.GetContext();
        }

       public Decision GetBestDecision()
       {
            ScoreAllDecisions();
            decisions.Sort();
            return decisions[0];
       }

        public void ScoreAllDecisions()
        {
            float cutoff = 0f;
            for(int i = 0; i < decisions.Count; i++)
            {
                float bonus = decisions[i].GetContext().GetBonusFactor(last);
                if (bonus < cutoff)
                    continue;

                DecisionScoreEvaluator dse = decisions[i].GetDSE();
                decisions[i].score = dse.Score(decisions[i].GetContext(), bonus, cutoff);

                if (decisions[i].score > cutoff)
                {
                    cutoff = decisions[i].score;
                }
            }
        }

    }

}