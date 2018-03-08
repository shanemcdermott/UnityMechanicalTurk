using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilitySystem
{

    public class SpawnDecision : Decision
    {
        public GameObject objectToSpawn;
        
        public override DecisionContext GetContext()
        {
            return new DecisionContext(id, gameObject, objectToSpawn);  
        }

        public override void Perform()
        {
            GameObject.Instantiate(objectToSpawn, transform);
        }


    }


}