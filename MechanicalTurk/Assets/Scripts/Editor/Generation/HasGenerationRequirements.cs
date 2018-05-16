using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HasGenerationRequirements
{
    /// <summary>
    /// Used for editor scripts to show missing components
    /// </summary>
    void ShowMissingGenRequirements();
}
