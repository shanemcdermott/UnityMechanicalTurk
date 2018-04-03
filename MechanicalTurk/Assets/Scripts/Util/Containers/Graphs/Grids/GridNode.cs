using System.Collections.Generic;

public class GridNode : GridFace
{
    const int NUM_CHILDREN = 4;
    protected GridNode[] children;

    public void ConnectVertices()
    {
        vertices[0].AddConnection(vertices[1]);
        vertices[0].AddConnection(vertices[2]);
        vertices[2].AddConnection(vertices[3]);
        vertices[3].AddConnection(vertices[1]);
    }

    public virtual void Subdivide()
    {
        if(children!=null)
        {
            for(int x=0;x<NUM_CHILDREN;x++)
            {
                    children[x].Subdivide();
            }
        }
        else
        {
            //Currently only works with squares
            children = new GridNode[NUM_CHILDREN];
            for(int x=0;x<NUM_CHILDREN;x++)
            {
                    children[x] = new GridNode();
                    children[x].position = MathOps.Midpoint(position, vertices[x].GetPosition());
            }
            
            Node center = new Node(position);
            Node bottomMid = Node.Split(vertices[0],vertices[1]);
            bottomMid.AddConnection(center);
            Node leftMid = Node.Split(vertices[0], vertices[2]);
            leftMid.AddConnection(center);
            Node rightMid = Node.Split(vertices[1], vertices[3]);
            rightMid.AddConnection(center);
            Node topMid = Node.Split(vertices[2], vertices[3]);
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
    }


}