using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum SelectionMethod
{
    Random,
    MinUtility,
    MaxUtility
}

public abstract class ObjectPicker : MonoBehaviour
{
    [SerializeField]
    public SelectionMethod defaultSelectionMethod;

    public abstract GameObject[] GetObjectOptions();

    public virtual GameObject GetChoice(SelectionMethod selectionMethod)
    {
        switch (selectionMethod)
        {
            case SelectionMethod.Random:
                return GetRandomChoice();
            case SelectionMethod.MinUtility:
                return GetMinUtilChoice();
            case SelectionMethod.MaxUtility:
                return GetMaxUtilChoice();
        }

        return null;
    }

    public virtual GameObject GetChoice()
    {
        return GetChoice(defaultSelectionMethod);
    }

    public virtual GameObject GetRandomChoice()
    {
        GameObject[] options = GetObjectOptions();
        return options[Random.Range(0, options.Length)];
    }

    public abstract GameObject GetMinUtilChoice();

    public abstract GameObject GetMaxUtilChoice();

}
