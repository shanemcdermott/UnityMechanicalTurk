using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies a road texture to terrain. 
/// </summary>
public class RoadPainter : MonoBehaviour
{
    
    //roadMap to alphaMap scale
    public Vector2 mapScale = new Vector2(0.5f, 0.5f);

    [Header("Roads")]
    public Texture2D roadHorizontal;
    public Texture2D roadVertical;
    public Vector2Int roadTileSize = new Vector2Int(1, 1);


    public TerrainData terrainData;
    private int NumTerrainTextures = 3;
    private float[,,] alphamaps;

    public int alphaMapWidth
    {
        get { return terrainData.alphamapWidth; }
    }
    public int alphaMapHeight
    {
        get { return terrainData.alphamapHeight; }
    }



    public void Setup()
    {
        if(terrainData == null)
        {
            terrainData = GetComponent<Terrain>().terrainData;
        }

        SetupSplatPrototypes();
        ResetAlphaBlend();
    }

    public void ResetAlphaBlend()
    {
        alphamaps = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, NumTerrainTextures];
        for (int x = 0; x < terrainData.alphamapWidth; x++)
        {
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                alphamaps[x, y, 0] = 1;
                alphamaps[x, y, 1] = 0;
                alphamaps[x, y, 2] = 0;
            }
        }
    }

    public bool DoesTerrainNeedSplatPrototypes()
    {
        return terrainData.splatPrototypes.Length != NumTerrainTextures;
    }

    /// <summary>
    /// Creates SplatPrototypes for the terrain's roads.
    /// </summary>
    public void SetupSplatPrototypes()
    {
       
        if(DoesTerrainNeedSplatPrototypes())
        {
            SplatPrototype[] textures = new SplatPrototype[NumTerrainTextures];

            textures[0] = terrainData.splatPrototypes[0];

            textures[1] = new SplatPrototype();
            textures[1].texture = roadHorizontal;
            textures[1].tileSize = roadTileSize;

            textures[2] = new SplatPrototype();
            textures[2].texture = roadVertical;
            textures[2].tileSize = roadTileSize;
            terrainData.splatPrototypes = textures;
        }
    }

    public void DrawTextureAt(Vector2Int worldPoint, int TextureID)
    {
        if (worldPoint.x * 2 + 1 >= alphaMapWidth ||
            worldPoint.y * 2 +1 >= alphaMapHeight)
        {
            return;
        }

        for(int z = 0; z < NumTerrainTextures; z++)
        {
            alphamaps[worldPoint.y*2, worldPoint.x * 2 , z] = (z == TextureID) ? 1f : 0f;
            alphamaps[worldPoint.y*2, worldPoint.x * 2 + 1, z] = (z == TextureID) ? 1f : 0f;
            alphamaps[worldPoint.y*2 + 1, worldPoint.x * 2, z] = (z == TextureID) ? 1f : 0f;
            alphamaps[worldPoint.y*2 + 1, worldPoint.x * 2 + 1, z] = (z == TextureID) ? 1f : 0f;
        }
    }

    public void DrawLine(Node from, Node to)
    {
        Vector2 fromPos = from.GetPositionXZ();
        Vector2Int fromPosInt = new Vector2Int((int)fromPos.x, (int)fromPos.y);

        Vector2 toPos = to.GetPositionXZ();
        Vector2Int toPosInt = new Vector2Int((int)toPos.x, (int)toPos.y);

        if(fromPosInt.x - toPosInt.x == 0)
        {
            DrawLine(fromPosInt, toPosInt, 1);
        }
        else
        {
            DrawLine(fromPosInt, toPosInt, 2);
        }
    }

    public void DrawLine(Vector2Int from, Vector2Int to, int textureIndex)
    {
        from = new Vector2Int((int)(from.x * mapScale.x),(int)(from.y * mapScale.y));
        to = new Vector2Int((int)(to.x * mapScale.x), (int)(to.y * mapScale.y));

        for (int x = from.x; x <= to.x; x++)
        {
            for(int y = from.y; y <= to.y; y++)
            {
                for (int z = 0; z < NumTerrainTextures; z++)
                {
                    if (z == textureIndex)
                    {
                        alphamaps[x, y, textureIndex] = 1;
                    }
                    else
                    {
                        alphamaps[x, y, textureIndex] = 0;
                    }
                }
            }
        }
    }

    public Vector2Int ScaleAlphaToTerrain(int x, int y)
    {
        return new Vector2Int((int)(x * mapScale.x),(int)( y * mapScale.y));
    }

    public void DrawRoads(ref Dictionary<Vector2Int, bool> roadPoints)
    {

        
        foreach (KeyValuePair<Vector2Int, bool> roadPoint in roadPoints)
        {
            DrawTextureAt(roadPoint.Key, roadPoint.Value ? 2 : 1);
        }
        
        /*
        for(int x = 0; x < terrainData.alphamapWidth; x++)
        {
            for(int y = 0; y < terrainData.alphamapHeight; y++)
            {
                bool result = true;

                if(roadPoints.TryGetValue(ScaleAlphaToTerrain(x,y), out result))
                {
                    if(result)
                    {
                        alphamaps[y, x, 0] = 0;
                        alphamaps[y, x, 1] = 0;
                        alphamaps[y, x, 2] = 1;
                    }                
                    else             
                    {                
                        alphamaps[y, x, 0] = 0;
                        alphamaps[y, x, 1] = 1;
                        alphamaps[y, x, 2] = 0;
                    }
                }
            }
        }
        */
    }

    public void DrawRoadsFromTexture(Texture2D srcTexture)
    {
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                //check if the road is at this x/y (normalized), if so, alpha=1, else alpha=0
                Color color = srcTexture.GetPixel((int)(x *mapScale.x), (int)(y * mapScale.y));

                if (color.b == 1)
                {
                    alphamaps[y, x, 0] = 0;
                    alphamaps[y, x, 1] = 1;
                    alphamaps[y, x, 2] = 0;
                }
                else if (color.r == 1)
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
    }


    public void ApplyAlphaBlend()
    {
        terrainData.SetAlphamaps(0, 0, alphamaps);
        alphamaps = null;
    }
}
