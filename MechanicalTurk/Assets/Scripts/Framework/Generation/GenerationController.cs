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

    protected virtual void Setup()
    {
        Random.InitState(Seed);
        LookForComponents();
        ConnectToAlgorithms();
        controlledSequence.Setup();
        heightMapGenerator.noiseMap = heightMap;
        heightMapGenerator.Setup();
    }

    //Give child Algorithms a reference to this controller.
    protected virtual void ConnectToAlgorithms()
    {
        controlledSequence.Controller = this;
        heightMapGenerator.Controller = this;
    }

    public virtual void StartGenerationSequence()
    {
        GenerateHeightmap();

        if (controlledSequence.CanGenerate())
        {
            controlledSequence.Generate(true);
        }
    }

    public void GenerateHeightmap()
    {
       heightMapGenerator.OnGenerationComplete.AddListener(GenerateBiomes);
       heightMapGenerator.Generate(true);
    }

    public void GenerateBiomes()
    {
        MapGenerator mapGen = GetComponent<MapGenerator>();
        if (mapGen)
        {
           Color[] colors = mapGen.GetColorMap(heightMap);
           Texture2D tex = TextureGenerator.TextureFromColourMap (colors, heightMap.Width, heightMap.Height);
           TerrainGenerator terrainGenerator = GetComponent<TerrainGenerator>();
           terrainGenerator.terrainTexture = tex;
           terrainGenerator.GenerateTerrain();
        }
    }

    public void SetupAndGenerate()
    {
        Setup();
        StartGenerationSequence();
    }
}
