using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GenerationController controllerA;
    public GenerationController controllerB;

    public Camera cameraA;
    public Camera cameraB;


    public Transform[] cameraAsequence;
    public float[] cameraAsequenceDuration;

    public Transform[] cameraBsequence;
    public float[] cameraBsequenceDuration;


    public int seqIndex = 0;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(CameraView());
	}
	
	// Update is called once per frame
	void Update ()
    {
        
        cameraA.transform.position = Vector3.Lerp(cameraA.transform.position, cameraAsequence[seqIndex].position, Time.deltaTime);
        cameraA.transform.rotation = Quaternion.Lerp( cameraA.transform.rotation, cameraAsequence[seqIndex].rotation, Time.deltaTime);

        cameraB.transform.position = Vector3.Lerp(cameraB.transform.position, cameraBsequence[seqIndex].position, Time.deltaTime);
        cameraB.transform.rotation = Quaternion.Lerp(cameraB.transform.rotation, cameraBsequence[seqIndex].rotation, Time.deltaTime);
    }

    public void StartNextSequence()
    {
        StopAllCoroutines();
        StartCoroutine(CameraView());
    }

    IEnumerator CameraView()
    {
        seqIndex = 0;
        while(seqIndex < cameraAsequence.Length)
        {
            yield return new WaitForSeconds(cameraAsequenceDuration[seqIndex]);
            seqIndex++;
        }

        controllerA.Seed++;
        controllerB.Seed++;
        controllerA.cityGenerator.OnGenerationComplete.AddListener(StartNextSequence);
        controllerA.SetupAndGenerate();
        controllerB.SetupAndGenerate();

    }
}
