using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilitySystem
{
    /*Maps inputs into decision*/
    public abstract class Consideration : MonoBehaviour
    {
        public string description;
     
        //Input and parameters determined by child implementation of score
        //Response Curve

        public abstract float Score(DecisionContext context);

        public abstract float ComputeResponseCurve(float score);
    }

}