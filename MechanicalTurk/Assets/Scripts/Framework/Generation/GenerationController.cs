using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages Random Seed, Generation Sequences.
/// </summary>
public class GenerationController : MonoBehaviour
{

    /*RNG Seed to be used for all generation processes*/
    public int Seed
    {
        get { return seed; }
        set
        {
            seed = value;
            Random.InitState(seed);
        }
    }

    public GenerationSequence controlledSequence;
    public NoiseGenerator heightMapGenerator;

    public NoiseMap heightMap;


    private int seed;
    void Awake()
    {
        LookForComponents();
        ConnectToAlgorithms();
    }

    protected virtual void LookForComponents()
    {
        if (heightMapGenerator == null)
        {
            heightMapGenerator = GetComponent<NoiseGenerator>();
        }
        if (controlledSequence == null)
        {
            controlledSequence = GetComponentInChildren<GenerationSequence>();
        }
        if (heightMap == null)
        {
            heightMap = GetComponent<NoiseMap>();
        }
    }

    void Start()
    {
        Setup();
        StartGenerationSequence();
    }

    void Setup()
    {
        Random.InitState(Seed);
        controlledSequence.Setup();
        heightMapGenerator.noiseMap = heightMap;
        heightMapGenerator.Setup();
    }

    //Give child Algorithms a reference to this controller.
    protected void ConnectToAlgorithms()
    {
        controlledSequence.Controller = this;
        heightMapGenerator.Controller = this;
    }

    public void StartGenerationSequence()
    {
        GenerateHeightmap();
        MapGenerator mapGen = GetComponent<MapGenerator>();
        if(mapGen)
        {
            mapGen.GenerateMap(heightMap.Values);
        }
        if (controlledSequence.CanGenerate())
        {
            controlledSequence.Generate(true);
        }
    }

    public void GenerateHeightmap()
    {
       heightMapGenerator.Generate();
    }

    public void SetupAndGenerate()
    {
        Setup();
        StartGenerationSequence();
    }
}
