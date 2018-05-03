using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Collections;
using Framework.Generation;


namespace Algorithms.City
{

    [System.Serializable]
    public struct ObjectSpawnParams
    {
        public GameObject[] prefabOptions;
        public float maxSteepness;
        public Vector2 minDimensions;

        public GameObject randomObject
        {
            get
            {
                return prefabOptions[Random.Range(0, prefabOptions.Length)];
            }
        }
    }

    /// <summary>
    /// Fills a square region with random objects
    /// </summary>
    public class BlockGenerator : GenerationAlgorithm
    {
        /// <summary>
        /// The collection of objects to choose from when populating.
        /// </summary>
        public List<GameObject> blockPrefabs = new List<GameObject>();

        /// <summary>
        /// An optional random point generator.
        /// If provided, pointGenerator is used to select which parts of districtNode are populated.
        /// </summary>
        public PointGenerator pointGenerator;

        [Header("Grid Settings")]
        ///Total dimensions of the city
        public Vector3 Dimensions = new Vector3(256f, 0f, 256f);
        ///Minimum size a plot of land can be
        public Vector3 MinLotSize = new Vector3(10f, 10f, 10f);

        ///Root node of the block
        public GridNode districtNode;
        /// <summary>
        /// Terrain asset- used to align spawned objects with terrain height values.
        /// </summary>
        public Terrain terrain;

        public RoadDesigner roadDesigner;
        /// <summary>
        /// Root Transform dummy object used for handling offets.
        /// </summary>
        protected GameObject cityBlock;

        /// <summary>
        /// Destroys cityBlock and district node.
        /// </summary>
        public override void Clean()
        {
            base.Clean();
            if (cityBlock != null)
            {
                DestroyImmediate(cityBlock);
            }
            districtNode = null;
        }

        /// <summary>
        /// Creates CityBlock dummy transform.
        /// If districtNode doesn't exist, CreateGrid is called.
        /// Tries to find roadDesigner
        /// </summary>
        public override void Setup()
        {
            cityBlock = new GameObject("CityBlock");
            cityBlock.transform.SetParent(transform);
            cityBlock.transform.position = transform.root.position;
            if (districtNode == null)
            {
                CreateGrid();
            }
            if (roadDesigner == null)
            {
                roadDesigner = GetComponent<RoadDesigner>();
            }
        }

        /// <summary>
        /// Creates a square GridNode and assigns to districtNode.
        /// </summary>
        public virtual void CreateGrid()
        {
            GridFaceFactory<GridNode> gFac = new GridFaceFactory<GridNode>();
            /*Also used as the center for the root node*/
            Vector3 lotSize = Dimensions * 0.5f;
            districtNode = gFac.GetSquareNode(lotSize, Dimensions);
        }

        /// <summary>
        /// Returns true if districtNode and terrain are valid.
        /// </summary>
        /// <returns>true if all generation requirements are met.</returns>
        public override bool CanGenerate()
        {
            return districtNode != null && terrain != null;
        }

        /// <summary>
        /// if pointGenerator is valid, calls PopulateBlocksSelectively.
        /// otherwise calls PopulateBlocks
        /// </summary>
        public override void Generate()
        {
            if (pointGenerator)
            {
                PopulateBlocksSelectively();
            }
            else
            {
                PopulateBlocks();
            }
        }


        /// <summary>
        /// Uniformly populates all districtNode leaves
        /// </summary>
        public virtual void PopulateBlocks()
        {

            List<GridNode> leaves;
            districtNode.GetLeaves(out leaves);
            List<GridNode> unvisited = new List<GridNode>(leaves);
            Vector2 worldOffset = districtNode.GetVertex(0).GetPositionXZ();

            foreach (GridNode node in leaves)
            {
                Vector3 v = node.GetPosition();
                //new Vector3(point.x, 0f, point.y);
                v.y = terrain.SampleHeight(v + transform.root.position);
                node.SetPosition(v);
                SpawnBlockObject(node);
            }

        }


        /// <summary>
        /// Uses pointGenerator to choose which districtNode leaves to populate.
        /// </summary>
        public virtual void PopulateBlocksSelectively()
        {
            List<GridNode> leaves;
            districtNode.GetLeaves(out leaves);
            List<GridNode> unvisited = new List<GridNode>(leaves);
            Vector2 worldOffset = districtNode.GetVertex(0).GetPositionXZ();
            List<Vector2> points;
            pointGenerator.Generate(districtNode.Dimensions.x, districtNode.Dimensions.z, MinLotSize.x, 1000, out points);
            foreach (Vector2 p in points)
            {
                Vector2 point = p + worldOffset;
                GridNode childNode;
                if (districtNode.GetChildContainingPoint(point, out childNode))
                {
                    if (unvisited.Remove(childNode))
                    {
                        Vector3 v = childNode.GetPosition();
                        //new Vector3(point.x, 0f, point.y);
                        v.y = terrain.SampleHeight(v + transform.root.position);
                        childNode.SetPosition(v);
                        SpawnBlockObject(childNode);
                    }
                }
            }
            PopulateUnvisitedNodes(unvisited);
        }


        /// <summary>
        /// Selects a GameObject to spawn from blockPrefabs and instantiates.
        /// If the spawned object has a BlockGenerator, its Generate function is called.
        /// </summary>
        /// <param name="parentNode">The node to use as the new object's root</param>
        /// <returns>Instantiated GameObject</returns>
        public virtual GameObject SpawnBlockObject(GridNode parentNode)
        {
            GameObject go = GameObject.Instantiate(ChooseDistrictToSpawn(parentNode));
            go.transform.SetParent(cityBlock.transform);
            go.transform.localPosition = parentNode.GetPosition();
            BlockGenerator blockGen = go.GetComponent<BlockGenerator>();
            if (blockGen)
            {
                blockGen.districtNode = parentNode;
                blockGen.Dimensions = parentNode.Dimensions;
                blockGen.terrain = terrain;
                blockGen.Setup();
                if (blockGen.CanGenerate())
                {
                    blockGen.Generate();
                }
            }
            return go;
        }

        /*By default, does nothing*/
        protected virtual void PopulateUnvisitedNodes(IEnumerable<GridNode> nodes)
        {

        }

        /// <summary>
        /// Randomly selects an object from blockPrefabs
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public virtual GameObject ChooseDistrictToSpawn(Node parentNode)
        {

            GameObject regionToSpawn = blockPrefabs[Random.Range(0, blockPrefabs.Count)];
            return regionToSpawn;
        }

        /// <summary>
        /// If roadDesigner exists, calls GetRoads
        /// </summary>
        /// <param name="roads"></param>
        public virtual void GetRoads(ref Dictionary<Vector2Int, bool> roads)
        {
            if (roadDesigner)
            {
                roadDesigner.GetRoads(districtNode, ref roads);
            }
        }

    }
}