using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinCityGenerator : CityGenerator {

    public PolyGrid polyGrid;

    public int lowRoadNumber = 2;
    public int highRoadNumber = 4;

    public Vector2Int facesPerSide;
    
    public TerrainGenerator terrainGenerator;
    
    [SerializeField]
    public NoiseGenerator buildingNoiseGenerator;

    public BuildingGenerator buildingGenerator;

    public RoadPainter roadPainter;

    public GameObject testObject;
    private GameObject[] buildings;

    public override void Generate()
    {
        CleanCityGen();
        GenerateRoadGrid();
        //SpawnTestObjects();
        CreateRoadsFromGrid();
        GenerateBuildingHeightMap();
        //GenerateBuildings();
    }

    void CleanCityGen()
    {
        Debug.Log("PerlinCityGenerator: Cleaning city");
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        if (polyGrid.NumFaces() != 0)
        {
            polyGrid.Clean();
        }
    }

    void GenerateRoadGrid()
    {
        Debug.Log("PerlinCityGenerator: Populating perlin road grid");
        polyGrid.FacesPerSide = new Vector2Int(facesPerSide.x, facesPerSide.y);
        GridFactory.GeneratePerlinGrid(ref polyGrid, lowRoadNumber, highRoadNumber);
    }

    private void SpawnTestObjects()
    {
        Debug.Log("PerlinCityGenerator: Spawning test objects");
        foreach (GridFace face in polyGrid.GetFaces()){
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
        Debug.Log("PerlinCityGenerator: Generating Buildings");
        List<GridFace> faces = polyGrid.GetFaces();
        foreach(GridFace face in faces)
        {
            Vector2 bottomLeft = face.GetVertex(0).GetPositionXZ();
            Vector2 midpoint = face.GetPositionXZ();

            Vector2 faceSize = new Vector2(midpoint.x - bottomLeft.x, midpoint.y - bottomLeft.y);
            
            if (CheckSlope(midpoint))
            {
                GameObject objectToSpawn = buildingGenerator.GetBuilding(midpoint, faceSize);
                if (objectToSpawn != null)
                {
                    GameObject instance = GameObject.Instantiate(objectToSpawn, transform);
                    float height = terrainGenerator.terrain.SampleHeight(new Vector3(midpoint.x, 0, midpoint.y));
                    instance.transform.position = new Vector3(midpoint.x, height, midpoint.y);
                }
            }
        }
    }

    bool CheckSlope(Vector2 point)
    {
        TerrainData terrainData = terrainGenerator.terrain.terrainData;
        Vector2 normalizedPoint = new Vector2(point.x / terrainData.size.x, point.y / terrainData.size.z);
        float steepness = terrainData.GetSteepness(normalizedPoint.x, normalizedPoint.y);
        if(steepness < 20)
        {
            return true;
        }

        return false;
    }


    void GenerateBuildingHeightMap()
    {
        Debug.Log("PerlinCityGenerator: Generating Building heightmaps");
        buildingNoiseGenerator.Generate();
        buildingGenerator.Generate();
    }

    public override void Setup()
    {
        polyGrid.Dimensions = heightMap.Dimensions;

        roadPainter.Setup();

        buildingNoiseGenerator.Setup();
        buildingGenerator.Setup();
    }
}
