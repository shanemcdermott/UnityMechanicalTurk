using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinCityGenerator : CityGenerator {
    public GameObject gameNode;
	public Texture2D tilingTexture;

    private Texture2D roadTexture;

    public int lowRoadNumber = 2;
    public int highRoadNumber = 4;

    public Vector2Int facesPerSide;
    
    public TerrainGenerator terrainGenerator;

    public GameObject testPlane;
    
    public List<GameObject> spawnedGameNodes;

    public override void Generate()
    {
        GenerateRoadGrid();
        PopulateRoadTexture();
        ApplyRoadToTerrain();
        //GenerateBuildings();
    }

    void GenerateRoadGrid()
    {
        for(int i = transform.childCount-1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
        
        if (polyGrid.NumFaces() != 0)
        {
            polyGrid.Clean();
        }
        
        polyGrid.FacesPerSide = new Vector2Int(facesPerSide.x, facesPerSide.y);
        Debug.Log("PerlinCityGenerator: Populating perlin road grid");
        GridFactory.GeneratePerlinGrid(ref polyGrid, lowRoadNumber, highRoadNumber);
        
    }

    void PopulateRoadTexture()
    {
        Debug.Log("PerlinCityGenerator: Populating Road Texture");
        roadTexture = new Texture2D(terrainGenerator.terrain.terrainData.heightmapWidth, terrainGenerator.terrain.terrainData.heightmapHeight);

        Color color = Color.black;

        List<Node> vertices = polyGrid.GetVertices();
        List<GridFace> faces = polyGrid.GetFaces();
        
        //draw connections between verts
        foreach(Node node in vertices)
        {
            List<Vector2Int> connectionPoints = node.GiveConnectionLines();
            foreach (Vector2Int v2 in connectionPoints)
            {
                roadTexture.SetPixel(v2.x, v2.y, color);
            }
        }

        //draw connections between face verts
        for(int i = 0; i < faces.Count;i++)
        {
            List<Vector2Int> connectionPoints = new List<Vector2Int>();
            connectionPoints.AddRange(faces[i].GetConnectionLine(faces[i].GetVertex(0).GetPosition(), faces[i].GetVertex(1).GetPosition()));
            connectionPoints.AddRange(faces[i].GetConnectionLine(faces[i].GetVertex(0).GetPosition(), faces[i].GetVertex(2).GetPosition()));
            connectionPoints.AddRange(faces[i].GetConnectionLine(faces[i].GetVertex(2).GetPosition(), faces[i].GetVertex(3).GetPosition()));
            connectionPoints.AddRange(faces[i].GetConnectionLine(faces[i].GetVertex(1).GetPosition(), faces[i].GetVertex(3).GetPosition()));

            foreach (Vector2Int v2 in connectionPoints)
            {
                roadTexture.SetPixel(v2.x, v2.y, color);
            }
        }

        roadTexture.Apply();

        testPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = roadTexture;
    }

    void ApplyRoadToTerrain()
    {
        Debug.Log("PerlinCityGenerator: Applying Road Texture to terrain");
        TerrainData tData = terrainGenerator.terrain.terrainData;
        SplatPrototype[] textures = new SplatPrototype[2];

        textures[0] = tData.splatPrototypes[0];
        textures[1] = new SplatPrototype();
        textures[1].texture = tilingTexture;
		textures[1].tileSize = new Vector2Int(1,1);

        float[,,] alphamaps = new float[tData.alphamapWidth, tData.alphamapHeight, 2];
        Debug.Log("Alphamap size: (w: " + tData.alphamapWidth + ",H: " + tData.alphamapHeight + ")");
        for (int y = 0; y < tData.alphamapHeight; y++)
        { 
            for (int x = 0; x < tData.alphamapWidth; x++)
            {
                //check if the road is at this x/y (normalized), if so, alpha=1, else alpha=0
                float alphaVal = roadTexture.GetPixel(x / 2, y / 2).grayscale;
                if(alphaVal != 0){
                    alphaVal = 1;
                }
                alphamaps[y, x, 0] = alphaVal;
                alphamaps[y, x, 1] = 1 - alphaVal;
            }
        }
        terrainGenerator.terrain.terrainData.splatPrototypes = textures;
        terrainGenerator.terrain.terrainData.SetAlphamaps(0, 0, alphamaps);
    }

    void GenerateBuildings()
    {

    }

    public override void Setup()
    {
        polyGrid.Dimensions = heightMap.Dimensions;
        polyGrid.FacesPerSide = new Vector2Int(2, 2);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
