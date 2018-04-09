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

    public RoadPainter roadPainter;

    public GameObject testObject;
    private GameObject[] buildings;

    public override void Generate()
    {
        GenerateRoadGrid();
        //SpawnTestObjects();
        CreateRoadsFromGrid();
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

    private void SpawnTestObjects()
    {
        foreach(GridFace face in polyGrid.GetFaces()){
            foreach(Node vert in face.GetVertices()){
                GameObject test = GameObject.Instantiate(testObject, transform);
                test.transform.position = vert.GetPosition();
            }
            
            
        }
    }

    public virtual void CreateRoadsFromGrid()
    {
        Debug.Log("PerlinCityGenerator: Creating roads from grid");
        List<Node> vertices = polyGrid.GetVertices();
        List<GridFace> faces = polyGrid.GetFaces();

        Dictionary<Vector2Int, bool> connectionPoints = new Dictionary<Vector2Int, bool>();
        //draw connections between verts
        foreach (Node node in vertices)
        {
            node.GetConnectionLines(ref connectionPoints);
        }

        //draw connections between face verts
        for (int i = 0; i < faces.Count; i++)
        {

            int a = Random.value > 0.5f ? 0 : 3;
            int b = Random.value > 0.5f ? 1 : 2; 

            Vector3 midPoint = MathOps.Midpoint(faces[i].GetVertex(a).GetPosition(), faces[i].GetVertex(b).GetPosition());
            faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetPosition(), midPoint);
            List<Node> verts = faces[i].GetVertices();
            foreach (Node node in verts)
            {
                node.GetConnectionLines(ref connectionPoints);
            }

        }


        roadPainter.DrawRoads(ref connectionPoints);
        roadPainter.ApplyAlphaBlend();
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
        roadPainter.Setup();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
