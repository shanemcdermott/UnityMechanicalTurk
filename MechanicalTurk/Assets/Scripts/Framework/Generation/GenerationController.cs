using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages Random Seed, Generation Sequences.
/// </summary>
public class GenerationController : MonoBehaviour
{

    /*RNG Seed to be used for all generation processes*/
    public int Seed;

    public TerrainGenerator terrainGenerator;


    public NoiseMap heightMap
    {
        get { return terrainGenerator.heightMap; }
        set
        {
            terrainGenerator.heightMap = value;
        }
    }


    private int seed;
    void Awake()
    {
        LookForComponents();
    }

    protected virtual void LookForComponents()
    {
        if (terrainGenerator == null)
        {
            terrainGenerator = GetComponentInChildren<TerrainGenerator>();
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
        terrainGenerator.Setup();
    }

    public virtual void StartGenerationSequence()
    {
        GenerateHeightmap();
    }

    public void GenerateHeightmap()
    {
        terrainGenerator.Generate(true);
    }

    public void SetupAndGenerate()
    {
        Setup();
        StartGenerationSequence();
    }
}
