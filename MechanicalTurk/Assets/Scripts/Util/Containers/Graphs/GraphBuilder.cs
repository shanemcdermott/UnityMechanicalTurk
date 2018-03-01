using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphBuilder : MonoBehaviour
{
    public DelaunayTriangulation delTri;
    public bool drawTriangles = false;


	public NoiseGenerator noiseGenerator;
    public PointGenerator pointGenerator;
    public bool drawPoints = true;


    public PolyGrid polyGrid;
    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] LOD_0_Prefabs = new GameObject[3];

    public void Start()
    {
        if (polyGrid.NumFaces() == 0)
        {
            Debug.Log("populating grid");
            GridFactory.PopulateSquareGrid(ref polyGrid);
            foreach(Node node in polyGrid.GetFaces())
            {
                GameObject go = GameObject.Instantiate(LOD_0_Prefabs[Random.Range(0,LOD_0_Prefabs.Length)]);
                go.transform.SetParent(transform);
                GameNode gn = go.GetComponent<GameNode>();
                gn.SetNode(node);
                gn.SpawnBuildings();
            }
        }
    }

    public void Init()
    {
        pointGenerator.Init(pointGenerator.Corners());
        InitializeGraph();
    }

    public void InitializeGraph()
    {
        delTri.Init();
    }

    public void NextPoint()
    {
        Vector2 v2 = pointGenerator.NextPoint();
        if (v2.Equals(Vector2.positiveInfinity)) return;
        delTri.AddPoint(v2);
    }
  
    public void FillPoints()
    {
        for (Vector2 v2 = pointGenerator.NextPoint(); !(v2.Equals(Vector2.positiveInfinity)); v2 = pointGenerator.NextPoint())
        {
            delTri.AddPoint(v2);
        }
    }

    public void UpdateMeshData()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        CreateMeshesFromFaces(mesh);
    }

    public void CreateMeshesFromFaces(Mesh mesh)
    {

        List<Vector3> verts = polyGrid.GetFaces()[0].GetVertexPositions();
        //For now, assuming 4 sides
        mesh.vertices = verts.ToArray();
        foreach (Vector3 v3 in verts)
        {
            Debug.Log(v3.ToString());
        }


        Vector3[] normals = new Vector3[verts.Count];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
        }
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;


        int[] tri = new int[6];
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;
    }
}
