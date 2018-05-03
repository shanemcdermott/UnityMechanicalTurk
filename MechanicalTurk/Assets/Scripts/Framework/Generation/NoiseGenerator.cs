using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Collections;

namespace Framework.Generation
{
    /*Populates a NoiseMap with random values */
    public abstract class NoiseGenerator : GenerationAlgorithm
    {
        [SerializeField] /*The NoiseMap to populate*/
        public NoiseMap noiseMap;

        /*Generated values are scaled by this amount*/
        public float scale = 1f;

        /*Width of #noiseMap*/
        public int width
        {
            get { return noiseMap.Width; }
            set
            {
                noiseMap.Width = value;
            }
        }

        /*Height of #noiseMap*/
        public int height
        {
            get { return noiseMap.Height; }
            set
            {
                noiseMap.Height = value;
            }
        }

        /**Searches for #noiseMap if one is not currently assigned*/
        public override void Setup()
        {
            if (noiseMap == null)
                noiseMap = GetComponent<NoiseMap>();
        }

        /**Returns true if all generation requirements are met. False otherwise*/
        public override bool CanGenerate()
        {
            return noiseMap != null;
        }

        /**Calls NoiseGenerator#Generate(int, int) using the width and height of #noiseMap as parameters */ 
        public override void Generate()
        {
            noiseMap.Values = GenerateHeightMap(noiseMap.Width, noiseMap.Height);
        }

        /*Populates #noiseMap with random values */
        public abstract float[,] GenerateHeightMap(int mapWidth, int mapHeight);
    }
}