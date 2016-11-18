using UnityEngine;
using System.Collections;

public static class MeshExtensions 
{
	public static Mesh ScaleVerts( this Mesh mesh, float scaler )
	{
		for (int i = 0; i < mesh.vertexCount; i++) 
		{
			mesh.vertices[i] = mesh.vertices[i] * scaler;
		}

		return mesh;
	}

	/*
	public static Mesh ResetTransform( Gameobject go, Vector3 lookAt )
	{
		// Create temporary array of verts using the current TransformPoint( vert )

		// Rotate transform to facing

		// Set mesh using original 
	}
	*/
}
