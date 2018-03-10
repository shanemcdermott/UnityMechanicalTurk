using UnityEngine;
using System.Collections;
using System;

public class MapGenerator : GenerationAlgorithm
{

	public enum DrawMode {HeightMap, ColourMap, Mesh};
	public DrawMode drawMode;
    public MapDisplay mapDisplay;

    public Texture2D heightMapTexture;
    public Texture2D colorMapTexture;

    public int mapSize;

	[Range(0,6)]
	public int levelOfDetail;


	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public bool autoUpdate;
  

    public NoiseGenerator noiseGenerator;
    public BiomeGenerator biomeGenerator;

    public override bool CanGenerate()
    {
        return noiseGenerator != null && noiseGenerator.CanGenerate();
    }

    public override void Setup()
    {
        if (noiseGenerator == null)
        {
            noiseGenerator = GetComponent<NoiseGenerator>();
        }
        noiseGenerator.noiseMap.Resize(new Vector2Int(mapSize, mapSize));
        noiseGenerator.Setup();
        biomeGenerator = GetComponent<BiomeGenerator>();
        mapDisplay = GetComponent<MapDisplay>();
    }

    public override void Generate()
    {
        noiseGenerator.Generate();
        biomeGenerator.noiseMap = noiseGenerator.noiseMap;
        biomeGenerator.Setup();
        if(biomeGenerator.CanGenerate())
        {
            biomeGenerator.Generate();
        }
        CreateTextures();
        TerrainGenerator tgen = GetComponent<TerrainGenerator>();
        tgen.noiseMap = noiseGenerator.noiseMap;
        //DisplayResults();
    }

    public void CreateTextures()
    {
        colorMapTexture = TextureGenerator.TextureFromColourMap(biomeGenerator.colorMap, mapSize, mapSize);
        heightMapTexture = TextureGenerator.TextureFromHeightMap(noiseGenerator.noiseMap.Values);
    }

    public void DisplayResults()
    {
        switch(drawMode)
        {
            case DrawMode.HeightMap:
                mapDisplay.DrawTexture(heightMapTexture);
                break;
            case DrawMode.ColourMap:
                mapDisplay.DrawTexture(colorMapTexture);
                break;
            case DrawMode.Mesh:
                mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseGenerator.noiseMap.Values, meshHeightMultiplier, meshHeightCurve, levelOfDetail), colorMapTexture);
                break;
        }
    }

}

