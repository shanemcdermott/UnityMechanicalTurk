using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class TextureGenerator {

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(float[,] heightMap) {
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, heightMap [x, y]);
			}
		}

		return TextureFromColourMap (colourMap, width, height);
	}

    public static void RoadsAsTexture(ref Dictionary<Vector2Int, bool> connectionPoints, int width, int height)
    {
        Debug.Log("Saving road splatmap...");
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (connectionPoints.ContainsKey(new Vector2Int(x, y)))
                    colorMap[y * width + x] = Color.red;
                else
                    colorMap[y * width + x] = Color.black;

            }
        }
        Texture2D roadMap = TextureFromColourMap(colorMap, width, height);

        string fileName = Application.persistentDataPath + "/roadSplat.png";
        File.WriteAllBytes(fileName, roadMap.EncodeToPNG());
        Debug.Log("Saved to " + fileName);
    }
}
