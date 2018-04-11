using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Intelligence.UtilitySystem
{

    public abstract class Decision : MonoBehaviour, IComparable
    {
        public int id;
        public float score;
        public DecisionScoreEvaluator scoreEvaluator;

        public abstract DecisionContext GetContext();

        public DecisionScoreEvaluator GetDSE()
        {
            return scoreEvaluator;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((Decision)obj);
        }

        public int CompareTo(Decision other)
        {
            return Mathf.RoundToInt(10000f * (score - other.score)) / 10000;
        }

        public abstract void Perform();
    }

}