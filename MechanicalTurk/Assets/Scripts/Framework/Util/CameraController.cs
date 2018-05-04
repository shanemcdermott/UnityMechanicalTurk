using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Generation;

namespace Framework.Util
{
    public class CameraController : MonoBehaviour
    {
        
        public Camera[] cameras;
        public CameraView[] viewSequences;

        protected Transform[] sequenceTransformRoots;

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
            Array.Resize(ref sequenceTransformRoots, last + 1);

            string camobjName = "Camera" + last;

            GameObject goRoot = new GameObject(camobjName + "_ViewRoot");
            goRoot.transform.SetParent(transform);
            sequenceTransformRoots[last] = goRoot.transform;

            GameObject go = new GameObject(camobjName);
            go.transform.SetParent(goRoot.transform);

            cameras[last] = go.AddComponent<Camera>();
            cameras[last].depth = last;
        }

        public virtual void AddCameraView(int viewCameraId,
                                            float viewDuration,
                                            Vector3 viewLocation,
                                            Vector3 viewRotation)
        {


            string camViewName = "Camera" + viewCameraId + "_View" + (viewSequences[viewCameraId] == null ? 0 : viewSequences[viewCameraId].GetNumRemaining() + 1);

            GameObject go = new GameObject(camViewName);
            go.transform.SetParent(sequenceTransformRoots[viewCameraId]);



            if (viewSequences[viewCameraId] == null)
            {
                viewSequences[viewCameraId] = go.AddComponent<CameraView>();
                viewSequences[viewCameraId].duration = viewDuration;
                viewSequences[viewCameraId].transform.position = viewLocation;
                viewSequences[viewCameraId].transform.rotation = Quaternion.Euler(viewRotation);
            }
            else
            {

                CameraView lastView = viewSequences[viewCameraId].GetLast();

                if (lastView == null)
                {
                    lastView = go.AddComponent<CameraView>();
                    lastView.duration = viewDuration;
                    lastView.transform.position = viewLocation;
                    lastView.transform.rotation = Quaternion.Euler(viewRotation);
                    viewSequences[viewCameraId].nextView = lastView;
                }
                else
                {
                    lastView.nextView = go.AddComponent<CameraView>();
                    lastView.nextView.duration = viewDuration;
                    lastView.nextView.transform.position = viewLocation;
                    lastView.nextView.transform.rotation = Quaternion.Euler(viewRotation);
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
                viewSequences[i].BeginView(cameras[i]);
            }
        }

        public virtual void MakeSequencesFinite()
        {
            for (int i = 0; i < viewSequences.Length; i++)
            {
                CameraView lastView = viewSequences[i].GetLast();
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
                CameraView lastView = viewSequences[i].GetLast();
                if (lastView.nextView == null)
                {
                    lastView.nextView = viewSequences[i];
                }
            }
        }
    }
}