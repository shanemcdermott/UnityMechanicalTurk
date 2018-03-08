using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilitySystem
{

   [System.Serializable]
    public class DecisionContext
    {
        /*What are you trying to do?*/
        public int decisionId;
        /*Who is asking?*/
        public GameObject intelligence;

        //Optional target
        public GameObject targetObject;

        public DecisionContext(int decisionIdentifier, GameObject intelligenceController)
        {
            decisionId = decisionIdentifier;
            intelligence = intelligenceController;
            targetObject = null;
        }

        public DecisionContext(int decisionIdentifier, GameObject intelligenceController, GameObject targetObject)
        {
            this.decisionId = decisionIdentifier;
            this.intelligence = intelligenceController;
            this.targetObject = targetObject;
        }

        public GameObject GetIntelligence()
        {
            return intelligence;
        }

        public virtual float GetBonusFactor(DecisionContext lastDecision)
        {
            if (lastDecision.decisionId == decisionId)
                return 0.5f;
            else
                return 1f;
        }
    }

}