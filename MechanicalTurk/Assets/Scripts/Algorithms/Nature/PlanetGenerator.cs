using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Collections;
using Framework.Util;
using Framework.Generation;

namespace Algorithms.Nature
{

    public class PlanetGenerator : GenerationAlgorithm
    {

        public NoiseGenerator heightMapGenerator;
        public BiomeGenerator biomeGenerator;

        public float heightScale = 1f;

        public NoiseMap heightMap
        {
            get
            {
                if (_heightMap == null)
                {
                    _heightMap = GetComponent<NoiseMap>();
                }
                return _heightMap;
            }
            set
            {
                _heightMap = value;
                if (biomeGenerator)
                {
                    biomeGenerator.heightMap = value;
                }
                if (heightMapGenerator)
                {
                    heightMapGenerator.noiseMap = value;
                }
            }
        }
        private NoiseMap _heightMap;

        public Texture2D biomeTexture;
        public Material planetMaterial;

        public override bool CanGenerate()
        {
            return heightMapGenerator != null &&
                heightMapGenerator.CanGenerate() &&
                biomeGenerator != null &&
                //biomeGenerator.CanGenerate() &&
                heightMap != null;
        }

        public override void Setup()
        {
            if (heightMap == null)
            {
                heightMap = GetComponent<NoiseMap>();
            }
            if (heightMapGenerator == null)
            {
                heightMapGenerator = GetComponent<NoiseGenerator>();
            }
            if (biomeGenerator == null)
            {
                biomeGenerator = GetComponent<BiomeGenerator>();
            }
        }

        public override void Generate()
        {
            heightMapGenerator.OnGenerationComplete.AddListener(GenerateBiomes);
            heightMapGenerator.Setup();
            heightMapGenerator.Generate(true);
        }

        public virtual void GenerateBiomes()
        {
            biomeGenerator.OnGenerationComplete.AddListener(ApplyBiomeMap);
            biomeGenerator.Setup();
            biomeGenerator.Generate(true);
        }


        public virtual void ApplyBiomeMap()
        {
            if (biomeTexture != null)
            {
                biomeTexture.Resize(heightMap.Width, heightMap.Height);
                biomeTexture.SetPixels(biomeGenerator.GetColorMap());
                biomeTexture.Apply();
            }
            else
            {
                biomeTexture = TextureFactory.TextureFromColourMap(biomeGenerator.GetColorMap(), heightMap.Width, heightMap.Height);
                biomeTexture.name = "BiomeTexture";
            }
            if (biomeTexture)
            {
                planetMaterial.mainTexture = biomeTexture;
            }
        }

    }
}
