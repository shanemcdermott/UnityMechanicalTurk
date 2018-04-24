using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shane McDermott 2018
abstract public class PointGenerator : MonoBehaviour
{
    [SerializeField]
    public Rect bounds = new Rect(0, 0, 100, 100);

    public List<Vector2> samplePoints;

    public float width
    {
        get { return bounds.width; }
        set
        {
            bounds.width = value;
        }
    }
    public float height
    {
        get { return bounds.height; }
        set { bounds.height = value; }
    }

    public void SetBounds(float width, float height)
    {
        bounds = new Rect(0, 0, width, height);
    }

    public virtual Vector2 RandPoint()
    {
        float x = UnityEngine.Random.Range(0, bounds.width);
        float y = UnityEngine.Random.Range(0, bounds.height);
        return new Vector2(x, y);
    }

    public virtual Vector2 NextPoint()
    {
        return RandPoint();
    }


    public Vector2[] Corners()
    {
        Vector2 bottomLeft = new Vector2(bounds.xMin, bounds.yMin);
        Vector2 topLeft = new Vector2(bounds.xMin, bounds.yMax);
        Vector2 bottomRight = new Vector2(bounds.xMax, bounds.yMin);
        Vector2 topRight = new Vector2(bounds.xMax, bounds.yMax);
        return new Vector2[] { topLeft, bottomLeft, bottomRight, topRight };
    }

    public abstract void Init();
    public abstract void Init(Vector2[] sourcePoints);
    public abstract void Generate(out List<Vector2> results);
    public abstract void Generate(float width, float height, float minDstance, int maxPointsDesired, out List<Vector2> results);

}