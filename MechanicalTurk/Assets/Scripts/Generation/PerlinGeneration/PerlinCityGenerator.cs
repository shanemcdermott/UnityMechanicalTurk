using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinCityGenerator : CityGenerator {

    public PolyGrid polyGrid;
    public GameObject gameNode;
	public Texture2D roadHorizontal;
    public Texture2D roadVertical;

    private Texture2D roadTexture;

    public int lowRoadNumber = 2;
    public int highRoadNumber = 4;

    public Vector2Int facesPerSide;
    
    public TerrainGenerator terrainGenerator;

    public GameObject testPlane;
    
    public List<GameObject> spawnedGameNodes;
    
    [SerializeField]
    public NoiseGenerator buildingNoiseGenerator;

    public BuildingGenerator buildingGenerator;
    private GameObject[] buildings;

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

        List<Node> vertices = polyGrid.GetVertices();
        List<GridFace> faces = polyGrid.GetFaces();
        
        //draw connections between verts
        foreach(Node node in vertices)
        {
            //if the value is true, it is horizontal, and should be red, vertical = green
            Dictionary<Vector2Int, bool> connectionPoints = node.GiveConnectionLines();
            foreach (KeyValuePair<Vector2Int,bool> kvp in connectionPoints)
            {
                if(kvp.Value == true)
                {
                    roadTexture.SetPixel(kvp.Key.x, kvp.Key.y, Color.red);
                }
                else
                {
                    roadTexture.SetPixel(kvp.Key.x, kvp.Key.y, Color.blue);
                }
                
            }
        }

        //draw connections between face verts
        for(int i = 0; i < faces.Count;i++)
        {
            Dictionary<Vector2Int, bool> connectionPoints = new Dictionary<Vector2Int, bool>();
            CombineWith(connectionPoints, faces[i].GetConnectionLine(faces[i].GetVertex(0).GetPosition(), faces[i].GetVertex(1).GetPosition()));
            CombineWith(connectionPoints, faces[i].GetConnectionLine(faces[i].GetVertex(0).GetPosition(), faces[i].GetVertex(2).GetPosition()));
            CombineWith(connectionPoints, faces[i].GetConnectionLine(faces[i].GetVertex(2).GetPosition(), faces[i].GetVertex(3).GetPosition()));
            CombineWith(connectionPoints, faces[i].GetConnectionLine(faces[i].GetVertex(1).GetPosition(), faces[i].GetVertex(3).GetPosition()));

            foreach (KeyValuePair<Vector2Int,bool> kvp in connectionPoints)
            {
                if(kvp.Value == true)
                {
                    roadTexture.SetPixel(kvp.Key.x, kvp.Key.y, Color.red);
                }
                else
                {
                    roadTexture.SetPixel(kvp.Key.x, kvp.Key.y, Color.blue);
                }
                
            }
        }

        roadTexture.Apply();

        Material planeMat = testPlane.GetComponent<Renderer>().sharedMaterial;
        planeMat.mainTexture = roadTexture;
    }

    void CombineWith(Dictionary<Vector2Int, bool> dict, Dictionary<Vector2Int, bool> dictToAdd)
    {
        foreach (KeyValuePair<Vector2Int, bool> kvp in dictToAdd)
        {
            bool value;
            if (!dict.TryGetValue(kvp.Key, out value))
            {
                dict.Add(kvp.Key, kvp.Value);
            }
        }
    }

    void ApplyRoadToTerrain()
    {
        Debug.Log("PerlinCityGenerator: Applying Road Texture to terrain");
        TerrainData tData = terrainGenerator.terrain.terrainData;
        SplatPrototype[] textures = new SplatPrototype[3];

        textures[0] = tData.splatPrototypes[0];

        textures[1] = new SplatPrototype();
        textures[1].texture = roadHorizontal;
		textures[1].tileSize = new Vector2Int(1,1);

        textures[2] = new SplatPrototype();
        textures[2].texture = roadVertical;
        textures[2].tileSize = new Vector2Int(1, 1);

        float[,,] alphamaps = new float[tData.alphamapWidth, tData.alphamapHeight, 3];
        
        for (int y = 0; y < tData.alphamapHeight; y++)
        { 
            for (int x = 0; x < tData.alphamapWidth; x++)
            {
                //check if the road is at this x/y (normalized), if so, alpha=1, else alpha=0
                Color color = roadTexture.GetPixel(x / 2, y / 2);
                
                if(color.b == 1){
                    alphamaps[y, x, 0] = 0;
                    alphamaps[y, x, 1] = 1;
                    alphamaps[y, x, 2] = 0;
                }
                else if(color.r == 1)
                {
                    alphamaps[y, x, 0] = 0;
                    alphamaps[y, x, 1] = 0;
                    alphamaps[y, x, 2] = 1;
                }
                else
                {
                    alphamaps[y, x, 0] = 1;
                    alphamaps[y, x, 1] = 0;
                    alphamaps[y, x, 2] = 0;
                }
            }
        }
        terrainGenerator.terrain.terrainData.splatPrototypes = textures;
        terrainGenerator.terrain.terrainData.SetAlphamaps(0, 0, alphamaps);
    }

    void GenerateBuildings()
    {
        GenerateBuildingHeightMap();

        //need a list of buildings with colliders (looks like they are set by biomeGenerator right now)

        //need to check each gridface? but i should have multiple buildings in some spots

        List<GridFace> faces = polyGrid.GetFaces();
        foreach(GridFace face in faces)
        {
            Vector2 bottomLeft = face.GetVertex(0).GetPositionXZ();
            Vector2 topRight = face.GetVertex(3).GetPositionXZ();

            Vector2 midpoint = MathOps.Midpoint(bottomLeft, topRight);
            Vector2Int midpointInt = new Vector2Int((int)midpoint.x, (int)midpoint.y);

            GameObject objectToSpawn = buildings[(midpointInt.x * buildingGenerator.heightMap.Width )+ midpointInt.y];
            BoxCollider collider = objectToSpawn.AddComponent<BoxCollider>();
            float faceX = topRight.x - bottomLeft.x;
            float faceZ = topRight.y - bottomLeft.y;

            if(collider.size.x < faceX && collider.size.z < faceZ)
            {
                GameObject instance = GameObject.Instantiate(objectToSpawn, transform);
                instance.transform.position = new Vector3(midpoint.x, 1, midpoint.y);
            }
        }

        //go through roadtexture faces to find spots
            //foreach gridface
                //check size (add box collider to building, check x & z)
                //check slope (xz and yz < .4)

            //foreach valid spot
                //spawn a building based on perlin
    }
    
    void GenerateBuildingHeightMap()
    {
        buildingNoiseGenerator.Setup();
        buildingNoiseGenerator.Generate();
        buildingGenerator.Setup();
        buildingGenerator.Generate();
        buildings = buildingGenerator.GetBuildingMap();
    }

    public override void Setup()
    {
        if (polyGrid == null)
        {
            polyGrid = gameObject.GetComponent<PolyGrid>();
            if (polyGrid == null)
            {
                polyGrid = gameObject.AddComponent<PolyGrid>();
                polyGrid.Dimensions = heightMap.Dimensions;
            }
        }
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
