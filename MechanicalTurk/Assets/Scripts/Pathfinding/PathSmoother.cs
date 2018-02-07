using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSmoother : MonoBehaviour
{

    /// <summary>
    /// Takes an input path made up of nodes and returns a smoothed output path.
    /// </summary>
    /// <param name="inputPath">
    /// The path that will be smoothed. Length should be > 2.
    /// </param>
    /// <returns>
    /// The smoothed version of the input path.
    /// </returns>
    public List<Vector3> SmoothPath(List<Vector3> inputPath)
    {
        //If the input path isn't at least 3 nodes in length, we can't smooth it.
        if (inputPath.Count <= 2) return inputPath;

        //Create the output path
        List<Vector3> outputPath = new List<Vector3>();
        outputPath.Add(inputPath[0]);

        string[] masks = new string[1];
        masks[0] = "Blocking";
        int mask = LayerMask.GetMask(masks);
        //Keep track of where we are in the input path.
        //We start at the third node, because we assume two adjacent nodes will pass the ray cast.
        for(int inputIndex = 2; inputIndex < inputPath.Count -1; inputIndex++)
        {
            Vector3 direction = outputPath[outputPath.Count-1] - inputPath[inputIndex];
            float distance = Vector3.Distance(inputPath[inputIndex], outputPath[outputPath.Count-1]);
            
            //Perform a raycast from the current point to the previous output point. 
            if(Physics.Raycast(inputPath[inputIndex],direction, distance,mask))
            {
                //There's not a clear line between these points, so add the previous node to the output.
                outputPath.Add(inputPath[inputIndex-1]);
            }
            
        }

        //Add the end node of the inputPath to the smoothed outputPath and return it.
        outputPath.Add(inputPath[inputPath.Count - 1]);
        return outputPath;

    }
}
