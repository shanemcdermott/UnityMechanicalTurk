using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace Framework.Intelligence.UtilitySystem
{


    public class UtilityProperties : MonoBehaviour, IComparable
    {
        public float[] weights;

        private float weightedSum;

        public void Start()
        {
            weightedSum = weights.Sum() / weights.Length;
        }

        public void OnValidate()
        {
            weightedSum = weights.Sum();
        }

        /*Returns the expected utility of spawning this object*/
        public virtual float GetUtility()
        {
            return weightedSum;
        }

        public int CompareTo(object obj)
        {
            UtilityProperties props = (UtilityProperties)obj;
            return CompareTo(props);
        }

        public int CompareTo(UtilityProperties other)
        {
            return Mathf.RoundToInt(1000f * (GetUtility() - other.GetUtility())) / 1000;
        }
    }
}