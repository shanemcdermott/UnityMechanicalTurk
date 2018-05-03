using System.Collections.Generic;
using UnityEngine;

namespace Framework.Collections
{

    public class GridNode : GridFace
    {
        public GridNode[] Children
        {
            get { return children; }
            set
            {
                children = value;
            }
        }

        protected GridNode[] children;

        public GridNode()
        {
            position = new Vector3();
            vertices = new List<Node>();
        }

        public GridNode(Vector3 center, ref List<Node> verts)
        {
            this.position = center;
            this.vertices = verts;
        }


        /*How many children this node has*/
        public int Height()
        {
            if (children == null) return 0;

            return 1 + children[0].Height();
        }

        public void ConnectVertices()
        {
            if (IsSquare())
            {
                vertices[0].AddConnection(vertices[1]);
                vertices[0].AddConnection(vertices[2]);
                vertices[2].AddConnection(vertices[3]);
                vertices[3].AddConnection(vertices[1]);
            }
        }

        public virtual void Subdivide()
        {
            Subdivide(0.5f);
        }

        public virtual void Subdivide(float blend)
        {
            if (!SubdivideChildren(blend))
            {
                SubdivideSelf(blend);
            }
        }


        /// <summary>
        /// Attempts to subidivide children. If Children are null, returns false.
        /// </summary>
        /// <param name="blend">Midpoint lerp weight</param>
        /// <returns>tTrue if successful.</returns>
        protected virtual bool SubdivideChildren(float blend)
        {
            if (children == null) return false;

            for (int x = 0; x < vertices.Count; x++)
            {
                children[x].Subdivide(blend);
            }

            return true;
        }

        protected virtual void SubdivideSelf(float weight)
        {
            //Currently only works with squares
            if (IsSquare())
            {
                const int NUM_CHILDREN = 4;
                children = new GridNode[NUM_CHILDREN];
                for (int x = 0; x < NUM_CHILDREN; x++)
                {
                    children[x] = new GridNode();
                    children[x].position = Vector3.Lerp(position, vertices[x].GetPosition(), weight);
                }

                Node center = new Node(position);
                Node bottomMid = Node.Split(vertices[0], vertices[1], weight);
                bottomMid.AddConnection(center);
                Node leftMid = Node.Split(vertices[0], vertices[2], weight);
                leftMid.AddConnection(center);
                Node rightMid = Node.Split(vertices[1], vertices[3], weight);
                rightMid.AddConnection(center);
                Node topMid = Node.Split(vertices[2], vertices[3], weight);
                topMid.AddConnection(center);
                int i = 0;
                children[i].AddVertices(new Node[NUM_CHILDREN]{
                vertices[i],
                bottomMid,
                leftMid,
                center
            });
                children[i].ConnectVertices();
                i++;
                children[i].AddVertices(new Node[NUM_CHILDREN]{
                bottomMid,
                vertices[i],
                center,
                rightMid
            });
                children[i].ConnectVertices();
                i++;
                children[i].AddVertices(new Node[NUM_CHILDREN]{
                leftMid,
                center,
                vertices[i],
                topMid
            });
                children[i].ConnectVertices();
                i++;
                children[i].AddVertices(new Node[NUM_CHILDREN]{
                center,
                rightMid,
                topMid,
                vertices[i]
            });
                children[i].ConnectVertices();
            }
            ConnectChildren();
        }

        public void ConnectChildren()
        {
            if (IsSquare())
            {
                children[0].AddConnection(children[1]);
                children[0].AddConnection(children[2]);
                children[3].AddConnection(children[1]);
                children[3].AddConnection(children[2]);
            }
        }

        public bool IsLeaf()
        {
            return children == null;
        }

        public void GetChildrenAtDepth(int depth, out List<GridNode> outChildren)
        {
            outChildren = new List<GridNode>();
            GetNodesAtDepth(depth, ref outChildren);
        }

        private void GetNodesAtDepth(int depth, ref List<GridNode> leafNodes)
        {
            int currentDepth = Height();

            if (currentDepth == depth)
            {
                leafNodes.Add(this);
            }
            else if (currentDepth < depth)
            {
                foreach (GridNode child in children)
                {
                    child.GetNodesAtDepth(depth, ref leafNodes);
                }
            }
        }

        public void GetLeaves(out Dictionary<Vector3, GridNode> leaves)
        {
            leaves = new Dictionary<Vector3, GridNode>();

        }
        private void GetChildLeaves(ref Dictionary<Vector3, GridNode> leafNodes)
        {
            if (IsLeaf())
            {
                leafNodes.Add(GetPosition(), this);
            }
            else
            {
                foreach (GridNode child in children)
                {
                    child.GetChildLeaves(ref leafNodes);
                }
            }
        }

        public void GetLeaves(out List<GridNode> leafNodes)
        {
            leafNodes = new List<GridNode>();
            GetChildLeaves(ref leafNodes);
        }

        private void GetChildLeaves(ref List<GridNode> leafNodes)
        {
            if (IsLeaf())
            {
                leafNodes.Add(this);
            }
            else
            {
                foreach (GridNode child in children)
                {
                    child.GetChildLeaves(ref leafNodes);
                }
            }
        }


        public virtual void GetChildVertices(out List<Node> verts)
        {
            verts = new List<Node>(vertices);
            if (!IsLeaf())
            {
                List<GridNode> childLeaves = new List<GridNode>();
                GetChildLeaves(ref childLeaves);
                foreach (GridNode child in childLeaves)
                {
                    verts.AddRange(child.GetVertices());
                }
            }
        }

        public virtual List<Node> GetChildVertices()
        {
            List<Node> verts = new List<Node>(vertices);
            if (!IsLeaf())
            {
                List<GridNode> childLeaves = new List<GridNode>();
                GetChildLeaves(ref childLeaves);
                foreach (GridNode child in childLeaves)
                {
                    verts.AddRange(child.GetVertices());
                }
            }

            return verts;
        }

        public virtual bool GetChildContainingPoint(Vector2 point, out GridNode child)
        {
            if (this.Contains(point))
            {
                if (IsLeaf())
                {
                    child = this;
                    return true;
                }
                foreach (GridNode childNode in children)
                {
                    if (childNode.GetChildContainingPoint(point, out child))
                    {
                        return true;
                    }
                }
            }

            child = null;
            return false;
        }

    }
}