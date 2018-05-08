using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Generation;
using Framework.Collections;


namespace Algorithms.City
{

    public class PerlinCityGenerator : CityGenerator
    {

        [Header("Debugging")]
        public GameObject testObject;
        public bool spawnTestObjectsEnabled = false;


        public TerrainGenerator terrainGenerator;

        [SerializeField]
        [Header("Buildings")]
        public NoiseGenerator buildingNoiseGenerator;
        public BuildingGenerator buildingGenerator;

        [SerializeField]
        [Header("Roads")]
        public RoadPainter roadPainter;
        public PolyGrid polyGrid;

        public int lowRoadNumber = 2;
        public int highRoadNumber = 4;

        public Vector2Int facesPerSide;

        private GameObject[] buildings;

        public override void Generate()
        {
            CleanCityGen();
            GenerateRoadGrid();
            if (spawnTestObjectsEnabled)
            {
                SpawnTestObjects();
            }
            CreateRoadsFromGrid();
            GenerateBuildingHeightMap();
            GenerateBuildings();
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
            foreach (GridFace face in polyGrid.GetFaces())
            {
                //GameObject test = GameObject.Instantiate(testObject, transform);
                //test.transform.position = face.GetPosition();
                foreach (Node vert in face.GetVertices())
                {
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

                Vector3 midPoint = Framework.Util.MathOps.Midpoint(faces[i].GetVertex(a).GetPosition(), faces[i].GetVertex(b).GetPosition());
                //TODO Check if there is a building here before drawing
                //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetPosition(), midPoint);

                List<Node> verts = faces[i].GetVertices();
                foreach (Node node in verts)
                {
                    node.GetConnectionLines(ref connectionPoints);
                }
            }

            roadPainter.DrawRoads(ref connectionPoints);
            roadPainter.ApplyAlphaBlend();
        }

        void GenerateBuildingHeightMap()
        {
            Debug.Log("PerlinCityGenerator: Generating Building heightmaps");
            buildingNoiseGenerator.Generate();

        }

        void GenerateBuildings()
        {
            Debug.Log("PerlinCityGenerator: Generating Buildings");
            List<GridFace> faces = polyGrid.GetFaces();
            foreach (GridFace face in faces)
            {
                Vector2 bottomLeft = face.GetVertex(0).GetPositionXZ();
                Vector2 midpoint = face.GetPositionXZ();

                Vector2 faceSize = new Vector2((midpoint.x - bottomLeft.x) * 2, (midpoint.y - bottomLeft.y) * 2);

                //if (CheckSlope(bottomLeft))
                //{
                GameObject district = buildingGenerator.GetBuilding(midpoint, faceSize);
                if (district != null)
                {
                    GameObject instance = GameObject.Instantiate(district, transform);

                    float height = terrainGenerator.terrain.SampleHeight(new Vector3(midpoint.x + transform.root.position.x, 0, midpoint.y + transform.root.position.z));
                    instance.transform.localPosition = new Vector3(midpoint.x, height, midpoint.y);

                    CityBlockGenerator districtBlockGenerator = instance.GetComponent<CityBlockGenerator>();
                    if (districtBlockGenerator != null)
                    {
                        districtBlockGenerator.Dimensions = new Vector3(faceSize.x, 0, faceSize.y);
                        districtBlockGenerator.terrain = terrainGenerator.terrain;
                        List<Node> verts = face.GetVertices();
                        GridNode parentNode = new GridNode(new Vector3(midpoint.x, 0, midpoint.y), ref verts);
                        districtBlockGenerator.districtNode = parentNode;
                        districtBlockGenerator.Setup();
                        districtBlockGenerator.Generate();
                    }


                    //Transform child = districtBlockGenerator.gameObject.transform.GetChild(0);
                    //child.localPosition = Vector3.zero;
                    //instance.transform.localPosition = new Vector3(bottomLeft.x, 0, bottomLeft.y);
                }
                //}


            }
        }

        bool CheckSlope(Vector2 point)
        {
            TerrainData terrainData = terrainGenerator.terrain.terrainData;
            Vector2 normalizedPoint = new Vector2(point.x / terrainData.size.x, point.y / terrainData.size.z);
            float steepness = terrainData.GetSteepness(normalizedPoint.x, normalizedPoint.y);
            if (steepness < 20)
            {
                return true;
            }

            return false;
        }

        public override void Setup()
        {
            polyGrid.Dimensions = heightMap.Dimensions;

            roadPainter.Setup();

            buildingNoiseGenerator.Setup();
            buildingGenerator.Setup();
        }
    }
}