using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Util
{

    /// <summary>
    /// Randomizes the object's transform within a specified range.
    /// </summary>
    public class RandomTransform : MonoBehaviour
    {
        public float scaleMin = 0.85f;
        public float scaleMax = 1.15f;
        // Use this for initialization
        void Awake()
        {
            RandomizeScale();
        }

        public void RandomizeScale()
        {
            float scale = Random.Range(scaleMin, scaleMax);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}