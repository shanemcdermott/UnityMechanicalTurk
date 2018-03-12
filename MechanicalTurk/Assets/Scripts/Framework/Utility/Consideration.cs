using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilitySystem
{
    /*Maps inputs into decision*/
    public abstract class Consideration : MonoBehaviour
    {
        public string description;

        public float[] floatParams;

        //Input and parameters determined by child implementation of score
        //Response Curve

        public float GetFloatParameter(int id)
        {
            return floatParams[id];
        }

        /*Compute a consideration score*/
        public virtual float Score(DecisionContext context)
        {
            return Score(context, null);
        }

        /*Compute a consideration score*/
        public abstract float Score(DecisionContext context, Consideration c);

        public abstract float ComputeResponseCurve(float score);
    }

}