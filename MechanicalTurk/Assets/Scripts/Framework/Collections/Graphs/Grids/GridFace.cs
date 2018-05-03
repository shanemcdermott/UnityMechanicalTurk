using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Framework.Util;

namespace Framework.Collections
{
    public class GridFace : Node
    {
        protected List<Node> vertices;

        public List<Node> Vertices
        {
            get
            {
                return vertices;
            }
        }


        public Vector3 Dimensions
        {
            get
            {
                Vector3 min = GetMin();
                Vector3 max = GetMax();
                return new Vector3(max.x - min.x, max.y - min.y, max.z - min.z);
            }
        }


        public GridFace()
        {
            position = new Vector3();
            vertices = new List<Node>();
        }

        public GridFace(Vector2 position2)
        {
            position = new Vector3(position2.x, 0, position2.y);
            vertices = new List<Node>();
        }

        public GridFace(Vector3 position)
        {
            this.position = position;
            vertices = new List<Node>();
        }

        public GridFace(GridFace toCopy)
        {
            this.position = toCopy.position;
            this.vertices = toCopy.vertices;
        }

        public virtual void AddVertices(Node[] vertices)
        {
            this.vertices.AddRange(vertices);
        }

        public int AddVertex(Node vertex)
        {
            vertices.Add(vertex);
            return vertices.Count - 1;
        }

        public bool IsSquare()
        {
            return vertices.Count == 4;
        }

        public int NumVertices()
        {
            return vertices.Count;
        }

        public Node GetVertex(int index)
        {
            return vertices[index];
        }

        public bool HasVertex(Node vertex)
        {
            return vertices.Contains(vertex);
        }

        public Node GetConnectionWithVertex(Node vertex)
        {
            int i = FindConnectionWithVertex(vertex);
            if (i != -1)
            {
                return connections[i];
            }
            return null;
        }

        protected int FindConnectionWithVertex(Node vertex)
        {
            for (int i = 0; i < NumConnections(); i++)
            {
                if (connections[i] != null)
                {
                    GridFace face = (GridFace)connections[i];
                    if (face != null && face.HasVertex(vertex))
                        return i;
                }
            }

            return -1;
        }

        public virtual void GetVertexPositions(out List<Vector3> pos)
        {
            pos = new List<Vector3>();
            foreach (Node node in vertices)
            {
                pos.Add(node.GetPosition());
            }
        }

        public virtual List<Node> GetVertices()
        {
            return vertices;
        }

        protected void SwapVertices(int i, int j)
        {
            Node ni = vertices[i];
            vertices[i] = vertices[j];
            vertices[j] = ni;
        }

        public override void DrawConnections()
        {
            Vector3 pNorm = position.normalized;

            Color color = Color.cyan;
            foreach (GridFace c in connections)
            {
                if (c != null)
                {
                    Debug.DrawLine(position, c.position, color);
                }
            }
        }

        public virtual Vector3 GetMin()
        {
            return vertices[0].GetPosition();
        }

        public virtual Vector3 GetMax()
        {
            return vertices[3].GetPosition();
        }

        public virtual bool Contains(Vector2 point)
        {
            Vector3 min = GetMin();
            Vector3 max = GetMax();
            return point.x > min.x && point.x < max.x && point.y > min.z && point.y < max.z;
        }

        /// Returns a circle represented as a vector, where x and y are its center and z is its radius
        public Vector3 GetCircumCircle(Vector2 p)
        {
            if (IsSquare())
            {
                float r = MathOps.CircumRadius(p, Vertices[0].GetPositionXZ(), Vertices[3].GetPositionXZ());
                Vector2 c = MathOps.CircumCenter(p, Vertices[0].GetPositionXZ(), Vertices[3].GetPositionXZ());
                return new Vector3(c.x, c.y, r);
            }
            else
            {
                float r = MathOps.CircumRadius(p, Vertices[1].GetPositionXZ(), Vertices[2].GetPositionXZ());
                Vector2 c = MathOps.CircumCenter(p, Vertices[1].GetPositionXZ(), Vertices[2].GetPositionXZ());

                return new Vector3(c.x, c.y, r);
            }
        }

        /*
        public static GridFace Merge(GridFace A, GridFace B)
        {
            List<Node> sharedVerts = new List<Node>();
            List<Node> exclusiveVerts = new List<Node>();
            foreach(Node node in A.Vertices)
            {
                Node shared = B.GetConnectionWithVertex(node);
                if (shared != null)
                    sharedVerts.Add(shared);
            }


        }
        */
    }
}