using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemGeneration : CityGenerator
{
    public TerrainGenerator terrainGenerator;
    public Texture2D[] tilingTextures;
    private int seed;
    private List<Vector3> roads = new List<Vector3>();
    private string axiom;
    private float angle;
    private string currString;
    private float length = 1f;
    private int iterations;
    private Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private Texture2D roadTexture;


    public override void Generate()
    {
        GenerateRoadGrid();
        PopulateRoadTexture();
        ApplyRoadToTerrain();
        //GenerateBuildings();
    }

    private void ApplyRoadToTerrain()
    {
        TerrainData tData = terrainGenerator.terrain.terrainData;
        SplatPrototype[] textures = new SplatPrototype[16];
        textures[0] = tData.splatPrototypes[0];
        for(int i = 1; i < 16; i++)
        {
            textures[i] = new SplatPrototype();
            textures[i].texture = tilingTextures[i - 1];
            textures[i].tileSize = Vector2Int.one;
        }
        float[,,] alphamaps = new float[tData.alphamapWidth, tData.alphamapHeight, 16];
        for (int x = 0; x < tData.alphamapWidth; x++) 
        {
            for (int y = 0; y < tData.alphamapHeight; y++)
            {
                for (int i = 1; i < 16; i++)
                {
                    alphamaps[x, y, i] = 0;
                }
                alphamaps[x, y, 0] = 1;
            }
        }

        foreach (Node node in polyGrid.GetVertices())
        {
            List<IConnection<Node>> connections = new List<IConnection<Node>>();
            node.GetConnections(out connections);
            int i = 0;
            foreach(IConnection<Node> edge in connections)
            {
                Node n = edge.GetToNode();
                Vector2 diff = n.GetPositionXZ() - node.GetPositionXZ();
                if (diff.x > 0)
                    i += 4;
                if (diff.x < 0)
                    i += 1;
                if (diff.y > 0)
                    i += 8;
                if (diff.y < 0)
                    i += 2;
            }
            Debug.Log("number of connections is " +node.NumConnections());
            alphamaps[(int)node.GetPosition().x, (int)node.GetPosition().y, i] = 1;
            alphamaps[(int)node.GetPosition().x, (int)node.GetPosition().y, 0] = 0;
        }

        
        terrainGenerator.terrain.terrainData.splatPrototypes = textures;
        terrainGenerator.terrain.terrainData.SetAlphamaps(0, 0, alphamaps);
    }

    private void PopulateRoadTexture()
    {
        roadTexture = new Texture2D(terrainGenerator.terrain.terrainData.heightmapWidth, terrainGenerator.terrain.terrainData.heightmapHeight);

        Color color = Color.black;

        List<Node> vertices = polyGrid.GetVertices();

        foreach (Node node in vertices)
        {
            Vector2 v2 = node.GetPositionXZ();
            roadTexture.SetPixel((int)v2.x, (int)v2.y, color);
        }

        roadTexture.Apply();
    }

    private void GenerateRoadGrid()
    {
        seed = GameObject.Find("GenController").GetComponent<GenerationController>().Seed;
        UnityEngine.Random.InitState(seed);
        //*****RULES************
        //Random character '!' : random event (-,+,delete last command, or nothing)
        rules.Add('X', "X+Y!F+");
        rules.Add('Y', "-F!X-Y");
        angle = 90f;
        iterations = 12;
        axiom = "FX";

        polyGrid.AddVertex(new Node(transform.position));
        currString = axiom;
        for (int i = 0; i < iterations; i++)
        {
            GenerateIteration();
        }
        //DrawLines();
    }

    void DrawLines()
    {
        for (int i = 0; i < roads.Count - 1; i++)
        {
            Debug.DrawLine(roads[i], roads[i + 1], Color.black, 10000f, false);
        }
    }

    void GenerateIteration()
    {
        string newString = "";

        char[] stringCharacters = currString.ToCharArray();

        for (int i = 0; i < stringCharacters.Length; i++)
        {
            char currentCharacter = stringCharacters[i];

            if (rules.ContainsKey(currentCharacter))
            {
                newString += rules[currentCharacter];
            }
            else
            {
                newString += currentCharacter.ToString();
            }
        }
        currString = newString;

        stringCharacters = currString.ToCharArray();
        int index = 0;
        for (int i = 0; i < stringCharacters.Length; i++)
        {
            char currentCharacter = stringCharacters[i];
            if (currentCharacter == 'F' || currentCharacter == 'G')
            {
                index++;
                transform.Translate(Vector3.forward * length);
                Node n = new Node(transform.position);
                polyGrid.AddConnection(polyGrid.NumVertexConnections(polyGrid.NumVertices() - 1),n);
                polyGrid.AddVertex(n);
            }
            else if (currentCharacter == 'f')//will create 'islands' of roads (vertex not added, still move)
            {
                transform.Translate(Vector3.forward * length);
            }
            else if (currentCharacter == '+')
            {
                transform.Rotate(Vector3.up * angle);
            }
            else if (currentCharacter == '-')
            {
                transform.Rotate(Vector3.up * -angle);
            }
            else if (currentCharacter == '[')
            {
                TransformInfo ti = new TransformInfo();
                ti.position = transform.position;
                ti.rotation = transform.rotation;
                transformStack.Push(ti);
            }
            else if (currentCharacter == ']')
            {
                TransformInfo ti = transformStack.Pop();
                transform.position = ti.position;
                transform.rotation = ti.rotation;
            }
            else if (currentCharacter == '!')//Insert Random Command
            {
                float num = UnityEngine.Random.value;
                if(num < 0.1f)//Turn left
                {
                    stringCharacters.SetValue('-', i + 1);
                    i--;
                }
                else if( num < 0.2f)//Turn right
                {
                    stringCharacters.SetValue('+', i + 1);
                    i--;
                }
                else if(num < 0.3f)//Delete command
                {
                    stringCharacters.SetValue('n', i + 1);
                    i--;
                }
                // Anything else
                //Do nothing


                //last segment +=  new Vector3( Random.next(),0, Random.next() );
                //random needs to be 0,1,2,3 
            }
        }
    }

    private class TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
