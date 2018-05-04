using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Util
{
    /// <summary>
    /// Represents a target view transform  for a camera to lerp to.
    /// </summary>
    public class CameraView : MonoBehaviour
    {
        /// <summary>
        /// The camera whose transform is being controlled.
        /// </summary>
        public Camera targetCamera;

        /// <summary>
        /// The next view in the sequence
        /// </summary>
        public CameraView nextView;

        /// <summary>
        /// How long the camera should take to lerp from its previous transform to this one.
        /// </summary>
        public float duration = 5;

        /// <summary>
        /// Unity Event dispatcher that triggers when LerpView has finished
        /// </summary>
        public UnityEvent OnLerpViewComplete;

        void Awake()
        {
            if(OnLerpViewComplete == null)
            {
                OnLerpViewComplete = new UnityEvent();
            }
        }

        /// <summary>
        /// Starts the LerpView coroutine
        /// </summary>
        /// <param name="targetCamera"></param>
        public void BeginView(Camera targetCamera)
        {
            this.targetCamera = targetCamera;
            StartCoroutine(LerpView());
        }

        /// <summary>
        /// Called when duration has been reached.
        /// If nextView is valid, its BeginView function is called.
        /// </summary>
        protected virtual void LerpViewComplete()
        {
            OnLerpViewComplete.Invoke();
            if(nextView != null)
            {
                nextView.BeginView(targetCamera);
            }

            OnLerpViewComplete.RemoveAllListeners();
            targetCamera = null;
            StopAllCoroutines();
        }

        /// <summary>
        /// Stops all coroutines and removes all event listeners
        /// </summary>
        private void OnDisable()
        {
            OnLerpViewComplete.RemoveAllListeners();
            StopAllCoroutines();
        }

        /// <summary>
        /// Lerps between camera's current position and transform position.
        /// </summary>
        /// <returns></returns>
        IEnumerator LerpView()
        {
            for(float timeElapsed = 0f;  timeElapsed < duration; timeElapsed += Time.deltaTime)
            {
                targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, transform.position, Time.deltaTime);
                targetCamera.transform.rotation = Quaternion.Lerp(targetCamera.transform.rotation, transform.rotation, Time.deltaTime);
                yield return null;
            }

            LerpViewComplete();
        }


        /// <summary>
        /// Can this view reach itself?
        /// </summary>
        /// <returns></returns>
        public bool IsCircular()
        {
            for(CameraView next = nextView; next != null; next = next.nextView)
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
        public CameraView GetLast()
        {
            CameraView last = nextView;
            for (CameraView next = nextView; next != null; next = next.nextView)
            {
                if (next == this) return last;
                last = next;
            }

            return last;
        }

        public float GetTotalDuration()
        {
            float totalDuration = duration;
            for (CameraView next = nextView; next != null; next = next.nextView)
            {
                if (next == this) return totalDuration;
                totalDuration += next.duration;
            }

            return totalDuration;
        }

        public int GetNumRemaining()
        {
            int totalRemaining = 0;
            for (CameraView next = nextView; next != null; next = next.nextView)
            {
                if (next == this) return totalRemaining;
                totalRemaining++;
            }

            return totalRemaining;
        }
    }
}