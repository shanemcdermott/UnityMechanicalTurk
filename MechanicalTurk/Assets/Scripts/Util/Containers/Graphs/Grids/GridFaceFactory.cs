using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFaceFactory<T> where T : GridFace, new()
{

	public T GetNewGridFace(Vector3 pos, Node[] verts)
	{
		T res = new T();
		res.SetPosition(pos);
		res.AddVertices(verts);
		return res;
	}


}
