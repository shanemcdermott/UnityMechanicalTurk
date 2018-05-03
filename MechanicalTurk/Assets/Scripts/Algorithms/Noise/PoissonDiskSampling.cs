using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Generation;
using Framework.Collections;

namespace Algorithms.Noise
{

    //Algorithm from http://devmag.org.za/2009/05/03/poisson-disk-sampling/
    public class PoissonDiskSampling : PointGenerator
    {
        //Minimum distance between points
        public float minDistance = 5;
        //How many new points each new point should try to spawn
        public int newPointsCount = 30;
        //Target total number of points to generate
        public int maxTotalPoints = 1000;

        [SerializeField]
        private float cellSize;
        public Grid2D grid;

        private Vector2 sourcePoint;
        //How many times the current @sourcePoint has been sampled
        private int sampleCount = int.MaxValue;

        private RandomQueue<Vector2> processList;

        public override void Setup()
        {
            cellSize = minDistance / Mathf.Sqrt(2);
            grid = new Grid2D(Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize));
            processList = new RandomQueue<Vector2>();
            samplePoints = new List<Vector2>();
            SetBounds(width, height);
        }

        public override void Setup(Vector2[] sourcePoints)
        {
            Setup();
            foreach (Vector2 v in sourcePoints)
            {
                AddPoint(v);
            }
        }

        public void NextSourcePoint()
        {
            sampleCount = 1;
            sourcePoint = processList.PopRandom();
        }

        public Vector2 MakeRandomPoint()
        {
            return base.NextPoint();
        }

        public override Vector2 NextPoint()
        {
            if (sampleCount >= newPointsCount)
            {
                if (processList.Empty())
                    return Vector2.positiveInfinity;

                NextSourcePoint();
            }
            for (; sampleCount < newPointsCount; sampleCount++)
            {
                Vector2 newPoint = GenerateRandomPointAround(sourcePoint, minDistance);
                if (bounds.Contains(newPoint) && !IsInNeighborhood(newPoint))
                {
                    AddPoint(newPoint);
                    return newPoint;
                }
            }
            return NextPoint();
        }

        private bool GenerateNextPoint()
        {
            return !(NextPoint().Equals(Vector2.positiveInfinity));
        }

        public void GeneratePoisson()
        {
            Setup();
            AddPoint(MakeRandomPoint());
            NextSourcePoint();
            while (GenerateNextPoint())
            {
            }
            Debug.Log("Generated " + samplePoints.Count + " points with Poisson Disk Sampling.");
        }

        private bool IsInNeighborhood(Vector2 newPoint)
        {

            IntPoint gridPoint = PointToGridCoord(newPoint);
            Vector2[,] square;
            int squareSize = 5;
            grid.SquareAroundPoint(gridPoint, squareSize, out square);
            for (int x = 0; x < squareSize; x++)
            {
                for (int y = 0; y < squareSize; y++)
                {
                    if (square[x, y] != Vector2.positiveInfinity && Vector2.Distance(square[x, y], newPoint) < minDistance)
                        return true;
                }
            }

            return false;
        }

        public void AddPoint(Vector2 InPoint)
        {
            processList.Push(InPoint);
            samplePoints.Add(InPoint);
            grid.SetPoint(PointToGridCoord(InPoint), InPoint);
        }

        public Vector2 GenerateRandomPointAround(Vector2 point, float minDist)
        {
            float r1 = UnityEngine.Random.value;
            float r2 = UnityEngine.Random.value;

            float radius = minDist * (r1 + 1);
            float angle = 2 * Mathf.PI * r2;

            float newX = point.x + radius * Mathf.Cos(angle);
            float newY = point.y + radius * Mathf.Sin(angle);

            return new Vector2(newX, newY);
        }

        public IntPoint PointToGridCoord(Vector2 point)
        {
            float gridX = (int)(point.x / cellSize);
            float gridY = (int)(point.y / cellSize);
            return new IntPoint(gridX, gridY);
        }

        public override void Generate(out List<Vector2> results)
        {
            GeneratePoisson();
            results = samplePoints;
        }

        public override void Generate(float width, float height, float minDstance, int maxPointsDesired, out List<Vector2> results)
        {
            this.width = width;
            this.height = height;
            this.minDistance = minDstance;
            this.maxTotalPoints = maxPointsDesired;
            GeneratePoisson();
            results = samplePoints;
        }

        public override void Generate()
        {
            GeneratePoisson();
        }

        public override bool CanGenerate()
        {
            return true;
        }
    }
}