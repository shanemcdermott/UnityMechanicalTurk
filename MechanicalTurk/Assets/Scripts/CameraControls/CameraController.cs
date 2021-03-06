﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CameraControls
{
    [System.Serializable]
    public class ViewTargetEvent : UnityEvent<Camera, CameraViewTarget>
    { }

    public class CameraController : MonoBehaviour
    {
 
        public Camera[] cameras;
        public CameraViewTarget[] viewSequences;

        /// <summary>
        /// Stores active coroutine for each camera/viewSequence
        /// </summary>
        private Dictionary<Camera, IEnumerator> viewLerpCoroutines;

        /// <summary>
        /// Unity Event dispatcher that triggers when LerpView has finished
        /// </summary>
        public ViewTargetEvent OnViewSequenceComplete;

        /// <summary>
        /// Enables screen capturing.
        /// </summary>
        public bool captureEnabled = false;
        /// <summary>
        /// filename prefix for screen capture
        /// </summary>
        public string capturePrefix = "Capture_";
        /// <summary>
        /// Number of screen captures that have been recorded so far.
        /// </summary>
        protected int captureCount = 0;
        private List<Texture2D> captures;


        private void Awake()
        {
            if (OnViewSequenceComplete == null)
            {
                OnViewSequenceComplete = new ViewTargetEvent();
            }

            viewLerpCoroutines = new Dictionary<Camera, IEnumerator>();
            captures = new List<Texture2D>();
        }

        // Use this for initialization
        void Start()
        {
            StartSequences();
        }


        /// <summary>
        /// Adds numCamerasToAdd cameras.
        /// </summary>
        /// <param name="numCamerasToAdd"></param>
        public void AddCameras(int numCamerasToAdd)
        {
            for(int i = 0; i < numCamerasToAdd; i++)
            {
                AddCamera();
            }
        }


        /// <summary>
        /// Creates and attaches a child game object with a camera.
        /// </summary>
        public void AddCamera()
        {
            int last = 0;
            if(cameras != null)
            {
                last = cameras.Length;
            }

            Array.Resize(ref cameras, last + 1);
            Array.Resize(ref viewSequences, last + 1);

            string camobjName = "Camera" + last;

            GameObject go = new GameObject(camobjName);
            go.transform.SetParent(transform);

            cameras[last] = go.AddComponent<Camera>();
            cameras[last].depth = last;
            UpdateCameraRatios();
            AlignCamerasWithStartView();
        }

        public void RemoveCamera(int cameraId)
        {
            if(cameraId >= 0 && cameraId < cameras.Length)
            {
                DestroyImmediate(cameras[cameraId].gameObject);
                cameras[cameraId] = cameras[cameras.Length - 1];
                viewSequences[cameraId] = viewSequences[viewSequences.Length - 1];
                Array.Resize(ref cameras, cameras.Length - 1);
                Array.Resize(ref viewSequences, viewSequences.Length - 1);

                UpdateCameraRatios();
            }
        }

        public virtual void AlignCameraWithTarget(int viewCameraId, Vector3 cameraOffset, Vector3 targetPosition)
        {
            cameras[viewCameraId].transform.position = targetPosition + cameraOffset;
            cameras[viewCameraId].transform.LookAt(targetPosition);
        }

        public virtual void UpdateCameraRatios()
        {
            float ratio = 1f / cameras.Length;
            for(int i =0; i < cameras.Length; i++)
            {
                cameras[i].rect = new Rect(new Vector2(i * ratio, 0), new Vector2(ratio, 1f));
            }
        }

        public virtual void AddCameraView(int viewCameraId, CameraViewTarget viewTarget)
        {
            
            if (viewSequences[viewCameraId] == null)
            {
                viewSequences[viewCameraId] = viewTarget;
            }
            else
            {

                CameraViewTarget lastView = viewSequences[viewCameraId].GetLast();

                if (lastView == null)
                {
                    viewSequences[viewCameraId].nextView = viewTarget;
                }
                else
                {
                    lastView.nextView = viewTarget;
                }
            }
        }

        public void AlignCamerasWithStartView()
        {
            for(int i = 0; i < cameras.Length; i++)
            {
                if(viewSequences[i])
                {
                    cameras[i].gameObject.transform.position = viewSequences[i].viewPosition;
                    cameras[i].gameObject.transform.rotation = Quaternion.Euler(viewSequences[i].viewRotation);
                }
            }
        }


        public virtual void StartSequences()
        {
            MakeSequencesCircular();
            RestartSequences();
        }

        public virtual void RestartSequences()
        {
            for (int i = 0; i < viewSequences.Length; i++)
            {
                BeginView(cameras[i], viewSequences[i]);
            }
        }

        public virtual void MakeSequencesFinite()
        {
            for (int i = 0; i < viewSequences.Length; i++)
            {
                CameraViewTarget lastView = viewSequences[i].GetLast();
                if (lastView.nextView != null)
                {
                    lastView.nextView = null;
                }
            }
        }

        public virtual void MakeSequencesCircular()
        {
            for (int i = 0; i < viewSequences.Length; i++)
            {
                CameraViewTarget lastView = viewSequences[i].GetLast();
                if (lastView.nextView == null)
                {
                    lastView.nextView = viewSequences[i];
                }
            }
        }

        /// <summary>
        /// Starts the LerpView coroutine
        /// </summary>
        /// <param name="targetCamera"></param>
        public void BeginView(Camera targetCamera, CameraViewTarget viewTarget)
        {
            viewLerpCoroutines.Add(targetCamera, LerpView(targetCamera, viewTarget));
            StartCoroutine(viewLerpCoroutines[targetCamera]);
        }


        /// <summary>
        /// Lerps between camera's current position and transform position.
        /// </summary>
        /// <returns></returns>
        IEnumerator LerpView(Camera targetCamera, CameraViewTarget rootViewTarget)
        {
            for (CameraViewTarget viewTarget = rootViewTarget; viewTarget != null; viewTarget = viewTarget.nextView)
            {
                for (float timeElapsed = 0f; timeElapsed < viewTarget.duration; timeElapsed += Time.deltaTime)
                {
                    targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, viewTarget.viewPosition, Time.deltaTime);
                    targetCamera.transform.rotation = Quaternion.Lerp(targetCamera.transform.rotation, Quaternion.Euler(viewTarget.viewRotation), Time.deltaTime);
                    yield return null;
                }

                if (captureEnabled)
                {
                    CaptureView();
                }
            }
            LerpSequenceComplete(targetCamera, rootViewTarget);
        }

        /// <summary>
        /// Called when duration has been reached.
        /// If nextView is valid, BeginView is called again.
        /// </summary>
        protected virtual void LerpSequenceComplete(Camera targetCamera, CameraViewTarget viewTarget)
        {
            StopCoroutine(viewLerpCoroutines[targetCamera]);
            viewLerpCoroutines.Remove(targetCamera);
            OnViewSequenceComplete.Invoke(targetCamera, viewTarget);
        }


        public virtual string GetCaptureFileName()
        {
            return Application.dataPath + "/../"+capturePrefix +(captureCount++).ToString() + ".png";
        }

        public virtual void CaptureView()
        {
            ScreenCapture.CaptureScreenshot(GetCaptureFileName());
            
        }

        
        public virtual void ExportCapures()
        {
            if (captures.Count == 0) return;

            //int width = captures[0].width;
            //int height = captures[0].height;
            //int stride = width / 8;
            //FileStream stream = new FileStream(GetCaptureFileName(), FileMode.Create);
            //GifBitmapEncoder encoder = new GifBitmapEncoder();
            for (int i = 0; i < captures.Count; i++)
            {
                byte[] bytes = captures[i].EncodeToPNG();
                //File.WriteAllBytes(GetCaptureFileName(), bytes);
                //BitmapSource image = BitmapSource.Create(width, height, 96, 96, System.Windows.Media.PixelFormats.Indexed1, BitmapPalettes.WebPalette, bytes, stride);
                //encoder.Frames.Add(BitmapFrame.Create(image));
            }
            //encoder.Save(stream);
        }
        
        public virtual void DestroyCaptures()
        {
            foreach(Texture2D tex in captures)
            {
                UnityEngine.Object.Destroy(tex);
            }
            captures.Clear();
        }

        /// <summary>
        /// Stops all coroutines and removes all event listeners
        /// </summary>
        private void OnDisable()
        {
            OnViewSequenceComplete.RemoveAllListeners();
            StopAllCoroutines();
        }
    }
}