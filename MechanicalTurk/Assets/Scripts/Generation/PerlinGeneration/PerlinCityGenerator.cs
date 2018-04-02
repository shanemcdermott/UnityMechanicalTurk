using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinCityGenerator : CityGenerator {
    public GameObject gameNode;
    private Texture2D roadTexture;

    public int lowRoadNumber = 2;
    public int highRoadNumber = 4;
    
    public TerrainGenerator terrainGenerator;

    public GameObject testPlane;
    

    public override void Generate()
    {
        GenerateRoadGrid();
        PopulateRoadTexture();
        ApplyRoadToTerrain();
        //GenerateBuildings();
    }

    void GenerateRoadGrid()
    {

        if (polyGrid.NumFaces() == 0)
        {
            polyGrid.FacesPerSide = new Vector2Int(5, 5);
            Debug.Log("PerlinCityGenerator: Populating perlin road grid");
            GridFactory.GeneratePerlinGrid(ref polyGrid, 2, 4);
            //GridFactory.PopulateSquareGrid(ref polyGrid);
        }
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
        for(int i = 0; i < faces.Count;)
        {
            List<Vector2Int> connectionPoints = GetConnectionPoints(faces[i].GetPosition(), faces[i + 1].GetPosition());

            foreach (Vector2Int v2 in connectionPoints)
            {
                roadTexture.SetPixel(v2.x, v2.y, color);
            }



            i += 2;
        }

        roadTexture.Apply();

        testPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = roadTexture;
    }

    private List<Vector2Int> GetConnectionPoints(Vector3 node, Vector3 connection)
    {
        List<Vector2Int> connectionLine = new List<Vector2Int>();

        Vector3 difference = connection - node;

        if (difference.x > 0)
        {
            Debug.Log("posdiffx");
            for (int i = (int)node.x; i <= (int)connection.x; i++)
            {
                connectionLine.Add(new Vector2Int(i, (int)node.z));
            }
        }
        else if (difference.x < 0)
        {
            Debug.Log("negdiffx");
            for (int i = (int)connection.x; i <= (int)node.x; i++)
            {
                connectionLine.Add(new Vector2Int(i, (int)connection.z));
            }
        }

        if (difference.z > 0)
        {
            Debug.Log("posdiffz");
            for (int i = (int)node.z; i <= (int)connection.z; i++)
            {
                connectionLine.Add(new Vector2Int((int)node.x, i));
            }
        }
        else if (difference.z < 0)
        {
            Debug.Log("negdiffz");
            for (int i = (int)connection.z; i <= (int)node.z; i++)
            {
                connectionLine.Add(new Vector2Int((int)connection.x, i));
            }
        }

        return connectionLine;
    }

    void ApplyRoadToTerrain()
    {
        Debug.Log("PerlinCityGenerator: Applying Road Texture to terrain");
        TerrainData tData = terrainGenerator.terrain.terrainData;
        SplatPrototype[] textures = new SplatPrototype[2];

        textures[0] = tData.splatPrototypes[0];
        textures[1] = new SplatPrototype();
        textures[1].texture = roadTexture;
        textures[1].tileSize = heightMap.Dimensions;

        float[,,] alphamaps = new float[tData.alphamapWidth, tData.alphamapHeight, 2];
        Debug.Log("Alphamap size: (w: " + tData.alphamapWidth + ",H: " + tData.alphamapHeight + ")");
        for (int y = 0; y < tData.alphamapHeight; y++)
        { 
            for (int x = 0; x < tData.alphamapWidth; x++)
            {

                //check if the road is at this x/y (normalized), if so, alpha=1, else alpha=0
                float alphaVal = roadTexture.GetPixel(x / 2, y / 2).grayscale;
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
