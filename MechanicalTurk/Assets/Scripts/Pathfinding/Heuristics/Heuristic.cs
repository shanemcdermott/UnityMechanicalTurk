using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heuristic<T> : MonoBehaviour
{

    public T goal;

    public virtual float Estimate(T node)
    {
        return 1f;
    }

}
