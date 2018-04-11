using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PointGenerator : MonoBehaviour
{
    [SerializeField]
    public PolygonCollider2D bounds;

    public List<Vector2> samplePoints;

    public virtual Vector2 RandPoint()
    {
        
        float x = UnityEngine.Random.Range(bounds.bounds.min.x, bounds.bounds.max.x);
        float y = UnityEngine.Random.Range(bounds.bounds.min.y, bounds.bounds.max.y);
        return new Vector2(x, y);
    }

    public virtual Vector2 NextPoint()
    {
        return RandPoint();
    }


    public Vector2[] Corners()
    {
        return bounds.points;
    }

    public abstract void Init();
    public abstract void Init(Vector2[] sourcePoints);
    public abstract void Generate(out List<Vector2> results);
    public abstract void Generate(float width, float height, float minDstance, int maxPointsDesired, out List<Vector2> results);
   
}