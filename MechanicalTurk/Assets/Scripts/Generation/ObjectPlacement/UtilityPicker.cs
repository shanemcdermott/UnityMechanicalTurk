using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilitySystem;

public class UtilityPicker : ObjectPicker
{
    [SerializeField]
    protected UtilityProperties[] options;

   
    public override GameObject[] GetObjectOptions()
    {
        GameObject[] goOptions = new GameObject[options.Length];
        for(int i =0; i < options.Length; i++)
        {
            goOptions[i] = options[i].gameObject;
        }

        return goOptions;
    }

    public override GameObject GetRandomChoice()
    {
        return options[UnityEngine.Random.Range(0, options.Length)].gameObject;
    }

    public override GameObject GetMinUtilChoice()
    {
        return options.Min().gameObject;
    }

    public override GameObject GetMaxUtilChoice()
    {
        return options.Max().gameObject;
    }

}
