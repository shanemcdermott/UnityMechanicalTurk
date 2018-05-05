using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Util
{
    /// <summary>
    /// Represents a target view transform  for a camera to lerp to.
    /// </summary>
    public class CameraViewTarget : ScriptableObject
    {
        public Vector3 viewPosition;
        public Vector3 viewRotation;

        /// <summary>
        /// How long the camera should take to lerp from its previous transform to this one.
        /// </summary>
        public float duration = 5;
        /// <summary>
        /// The next view in the sequence
        /// </summary>
        public CameraViewTarget nextView;

        /// <summary>
        /// Can this view reach itself?
        /// </summary>
        /// <returns></returns>
        public bool IsCircular()
        {
            for (CameraViewTarget next = nextView; next != null; next = next.nextView)
            {
                if (next == this) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the last CameraView in the sequence.
        /// If the sequence is circular, returns the CameraView that connects to this one.
        /// </summary>
        /// <returns>
        /// The last CameraView in the sequence.
        /// </returns>
        public CameraViewTarget GetLast()
        {
            CameraViewTarget last = nextView;
            for (CameraViewTarget next = nextView; next != null; next = next.nextView)
            {
                if (next == this) return last;
                last = next;
            }

            return last;
        }

        public int GetNumRemaining()
        {
            int totalRemaining = 0;
            for (CameraViewTarget next = nextView; next != null; next = next.nextView)
            {
                if (next == this) return totalRemaining;
                totalRemaining++;
            }

            return totalRemaining;
        }
    }

}
