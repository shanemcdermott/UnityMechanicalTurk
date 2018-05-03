using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Collections;

namespace Framework.Generation
{

    /// <summary>
    /// Manages Random Seed, Generation Sequences.
    /// </summary>
    public class GenerationController : MonoBehaviour
    {
        public List<GameObject> buildings = new List<GameObject>();
        /*RNG Seed to be used for all generation processes*/
        public int Seed;

        public TerrainGenerator terrainGenerator;
        public CityGenerator cityGenerator;

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
            if (cityGenerator == null)
            {
                cityGenerator = GetComponentInChildren<CityGenerator>();
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
            terrainGenerator.OnGenerationComplete.RemoveAllListeners();
            terrainGenerator.OnGenerationComplete.AddListener(GenerateCity);
            GenerateHeightmap();
        }

        public void GenerateHeightmap()
        {
            terrainGenerator.Generate(true);
        }

        public void GenerateCity()
        {
            cityGenerator.heightMap = terrainGenerator.heightMap;
            cityGenerator.Setup();
            if (cityGenerator.CanGenerate())
            {
                cityGenerator.Generate(true);
            }
            else
            {
                Debug.Log("City Generator is unable to generate at this time!");
            }
        }

        public void SetupAndGenerate()
        {
            Setup();
            StartGenerationSequence();
        }
    }
}