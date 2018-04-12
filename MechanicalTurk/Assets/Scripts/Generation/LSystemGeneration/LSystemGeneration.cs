﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemGeneration : CityGenerator
{
    public TerrainGenerator terrainGenerator;
    public Texture2D[] tilingTextures;
    private int seed;
    private string axiom;
    private float angle;
    private string currString;
    public int length;
    public int iterations;
    private List<Vector3> roads = new List<Vector3>();
    private Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
    private Dictionary<Vector2Int, Vector2Int[]> roadGrid = new Dictionary<Vector2Int, Vector2Int[]>();
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private Vector2Int prev, curr;
    private Texture2D roadTexture;


    public override void Generate()
    {
        
        GenerateRoadGrid();
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
        List<Vector2Int> vertices = new List<Vector2Int>(roadGrid.Keys);
        foreach( Vector2Int node in vertices)
        {
            Vector2Int[] connections = new Vector2Int[4] { new Vector2Int(int.MaxValue,int.MaxValue),
                                                           new Vector2Int(int.MaxValue,int.MaxValue),
                                                           new Vector2Int(int.MaxValue,int.MaxValue),
                                                           new Vector2Int(int.MaxValue,int.MaxValue)};
            if (roadGrid.TryGetValue(node, out connections))
            {
                int i = 0;
                int count = 0;
                foreach (Vector2Int edge in connections)
                {
                    if (edge == new Vector2Int(int.MaxValue,int.MaxValue))
                    {
                        //Debug.Log("Connection node of node (" + node.x + ", "+node.y + ") is zero!");

                        continue;
                    }
                    count++;
                    Vector2Int diff = edge - node;
                    if (diff.x > 0)
                        i += 4;
                    if (diff.x < 0)
                        i += 1;
                    if (diff.y > 0)
                        i += 8;
                    if (diff.y < 0)
                        i += 2;
                }
                Debug.Log("This iterator should be between 1-15: " + i);
                Debug.Log("Connection Data for Vertex: x:" + (int)node.x + "  y: " + (int)node.y+
                    " 1st connection: x:"+(int)connections[0].x+" y: "+(int)connections[0].y+
                    " 2nd connection: x:" + (int)connections[1].x + " y: " + (int)connections[1].y +
                    " 3rd connection: x:" + (int)connections[2].x + " y: " + (int)connections[2].y +
                    " 4th connection: x:" + (int)connections[3].x + " y: " + (int)connections[3].y);
                if (checkAlphaMap(node, new Vector2Int(tData.alphamapWidth, tData.alphamapHeight)))
                {
                    Debug.Log("node.y is " + node.y);
                    Debug.Log("node.x is " + node.x);
                    Debug.Log("i is " + i);
                    alphamaps[node.y * 2    , node.x * 2    , i] = 1;
                    alphamaps[node.y * 2 + 1, node.x * 2    , i] = 1;
                    alphamaps[node.y * 2    , node.x * 2 + 1, i] = 1;
                    alphamaps[node.y * 2 + 1, node.x * 2 + 1, i] = 1;
                    alphamaps[node.y * 2    , node.x * 2    , 0] = 0;
                    alphamaps[node.y * 2 + 1, node.x * 2    , 0] = 0;
                    alphamaps[node.y * 2    , node.x * 2 + 1, 0] = 0;
                    alphamaps[node.y * 2 + 1, node.x * 2 + 1, 0] = 0;
                    DrawConnections(alphamaps, node, connections);
                }
            }
            else
            {
                Debug.Log("Unable to set alphamap");
            }
        }

        
        terrainGenerator.terrain.terrainData.splatPrototypes = textures;
        terrainGenerator.terrain.terrainData.SetAlphamaps(0, 0, alphamaps);
    }

    private void DrawConnections(float[,,] alphamaps, Vector2Int node, Vector2Int[] connections)
    {
        foreach(Vector2Int edge in connections)
        {
            Vector2 v = new Vector2(edge.x - node.x, edge.y - node.y);
            v.Normalize();
            //NorthSouth road - 9
            //EastWest road - 5
            int bitFlag = v.x != 0 ? 5 : 9;
            for(int i =0; i < (int)Math.Ceiling((double)length/2f+1); i++)//Draw half the edge, other node will draw the rest if on the terrain
            {
                Vector2Int offset = new Vector2Int((int)v.x*i, (int)v.y);
                if (checkAlphaMap(node + offset, new Vector2(terrainGenerator.terrain.terrainData.alphamapWidth, terrainGenerator.terrain.terrainData.alphamapHeight)))
                {
                    alphamaps[(node.y + offset.y)* 2    , (node.x + offset.x ) * 2    , bitFlag] = 1;
                    alphamaps[(node.y + offset.y)* 2 + 1, (node.x + offset.x ) * 2    , bitFlag] = 1;
                    alphamaps[(node.y + offset.y)* 2    , (node.x + offset.x ) * 2 + 1, bitFlag] = 1;
                    alphamaps[(node.y + offset.y)* 2 + 1, (node.x + offset.x ) * 2 + 1, bitFlag] = 1;
                    alphamaps[(node.y + offset.y)* 2    , (node.x + offset.x ) * 2    , 0] = 0;
                    alphamaps[(node.y + offset.y)* 2 + 1, (node.x + offset.x ) * 2    , 0] = 0;
                    alphamaps[(node.y + offset.y)* 2    , (node.x + offset.x ) * 2 + 1, 0] = 0;
                    alphamaps[(node.y + offset.y)* 2 + 1, (node.x + offset.x ) * 2 + 1, 0] = 0;
                }
            }
        }
    }

    private bool checkAlphaMap(Vector2Int node, Vector2 alphaMapDimensions)
    {
        if (node.x * 2 + 1 >= alphaMapDimensions.x || node.y * 2 + 1 >= alphaMapDimensions.y)
            return false;
        else
            return true;
    }

    private void GenerateRoadGrid()
    {
        seed = GameObject.Find("GenController").GetComponent<GenerationController>().Seed;
        UnityEngine.Random.InitState(seed);
        transform.position = new Vector3(terrainGenerator.terrain.terrainData.alphamapWidth / 4, 0f, terrainGenerator.terrain.terrainData.alphamapHeight / 4);
        rules.Clear();
        //*****RULES************
        //Random character '!' : random event (-,+,delete last command, or nothing)
        rules.Add('X', "X+Y!F+");
        rules.Add('Y', "-F!X-Y");
        angle = 90f;
        axiom = "FX";

        roads.Clear();
        roadGrid.Clear();
        roadGrid.Add(new Vector2Int((int)transform.position.x,(int)transform.position.z), new Vector2Int[4]);
        roads.Add(transform.position);

        currString = axiom;
        for(int i =0; i < iterations; i++)
        {
            GenerateIteration();
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
                prev = new Vector2Int((int)transform.position.x, (int)transform.position.z);
                transform.Translate(Vector3.forward * length);
                curr = new Vector2Int((int)transform.position.x, (int)transform.position.z);
                if (!roadGrid.ContainsKey(curr))//dictionary does not contain new node
                {
                    roadGrid.Add(curr, new Vector2Int[4]);
                    roads.Add(transform.position);
                }
                //add connection
                /*Connections [0] - North
                                [1] - East
                                [2] - South
                                [3] - West*/
                Vector2Int[] CurrConnections = new Vector2Int[4];
                Vector2Int[] PrevConnections = new Vector2Int[4];
                if (roadGrid.TryGetValue(curr, out CurrConnections) && roadGrid.TryGetValue(prev, out PrevConnections))
                {
                    Vector2Int diff = curr - prev;
                    if (diff.x > 0)//node curr is east of node prev
                    {
                        CurrConnections[3] = prev;
                        PrevConnections[1] = curr;
                    }  
                    if (diff.x < 0)//node curr is west of node prev
                    {
                        CurrConnections[1] = prev;
                        PrevConnections[3] = curr;
                    }
                    if (diff.y > 0)//node curr is north of node prev
                    {
                        CurrConnections[2] = prev;
                        PrevConnections[0] = curr;
                    }
                    if (diff.y < 0)//node curr is south of node prev
                    {
                        CurrConnections[0] = prev;
                        PrevConnections[2] = curr;
                    }
                }
                else
                {
                    Debug.Log("Unable to add connection");
                }
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
                    stringCharacters.SetValue('-', i);
                    i--;
                }
                else if( num < 0.2f)//Turn right
                {
                    stringCharacters.SetValue('+', i);
                    i--;
                }
                else if(num < 0.3f)//Do nothing
                {
                    stringCharacters.SetValue('n', i);
                }
                //Need to change ^ to delete command and then do nothing
            }
        }
    }

    private class TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
