using UnityEngine;
using System.Collections;

public class TriMesh : MonoBehaviour 
{
	public enum Extrusion
	{
		In,
		Center,
		Out,
		None,
	}

	MeshFilter m_MeshFilter;
	MeshRenderer m_MeshRenderer;

	Extrusion m_Extrusion = Extrusion.Center;

	public bool Draw
	{
		set
		{
			m_MeshRenderer.enabled = value;
		}
	}

	public Material m_Mat;
	Mesh m_Mesh;

	public float m_Thickness = .05f;

	Vector3[]	m_BaseVerts = new Vector3[3];

	public Vector3[] BaseVerts {	get {	return m_BaseVerts;	}	}

	Vector3[] 	m_Verts;
	int[] 		m_Tris;

	Vector3 m_Normal; 
	
	public Vector3 Normal {		get {	return m_Normal;	}	}
	public Vector3 TransformedNormal 
	{		
		get
		{	
			Vector3 side1 = transform.TransformPoint( m_BaseVerts[1] ) - transform.TransformPoint( m_BaseVerts[0] );
			Vector3 side2 = transform.TransformPoint( m_BaseVerts[2] ) - transform.TransformPoint( m_BaseVerts[0] );
			return Vector3.Cross(side1, side2).normalized;
		}
	}

	public Vector3 Edge01MidPoint{	get	{	return ( transform.TransformPoint( BaseVerts[0] ) + transform.TransformPoint( BaseVerts[1] ) ) / 2f;}	}
	public Vector3 Edge12MidPoint{	get	{	return ( transform.TransformPoint( BaseVerts[1] ) + transform.TransformPoint( BaseVerts[2] ) ) / 2f;}	}
	public Vector3 Edge20MidPoint{	get	{	return ( transform.TransformPoint( BaseVerts[2] ) + transform.TransformPoint( BaseVerts[0] ) ) / 2f;}	}


	void Start () 
	{

	}


	public void CreateTri ( Vector3 vert0, Vector3 vert1, Vector3 vert2, Extrusion extrusion ) 
	{
		m_BaseVerts [0] = vert0;
		m_BaseVerts [1] = vert1;
		m_BaseVerts [2] = vert2;

		m_Extrusion = extrusion;


		m_MeshFilter = gameObject.AddComponent< MeshFilter > ();
		m_MeshRenderer = gameObject.AddComponent< MeshRenderer > ();
		m_MeshRenderer.material = m_Mat;

		Vector3 side1 = vert1 - vert0;
		Vector3 side2 = vert2 - vert0;
		m_Normal = Vector3.Cross(side1, side2).normalized;

		if (m_Extrusion == Extrusion.None) 
		{
			m_Verts = new Vector3[3];
			
			UpdateThickness ();
			
			// 8 tris * 3 verts each
			m_Tris = new int[ 3 ];
			
			// Face Down
			m_Tris [0] = 2;
			m_Tris [1] = 1;
			m_Tris [2] = 0;
		}
		else
		{
			m_Verts = new Vector3[6];

			UpdateThickness ();

			// 8 tris * 3 verts each
			m_Tris = new int[ 24 ];

			// Face Down
			m_Tris [0] = 2;
			m_Tris [1] = 1;
			m_Tris [2] = 0;

			// Face Up
			m_Tris [3] = 3;
			m_Tris [4] = 4;
			m_Tris [5] = 5;

			//Face 0
			m_Tris [6] = 0;
			m_Tris [7] = 1;
			m_Tris [8] = 3;

			m_Tris [9] = 1;
			m_Tris [10] = 4;
			m_Tris [11] = 3;

			//Face 1
			m_Tris [12] = 4;
			m_Tris [13] = 1;
			m_Tris [14] = 2;
		
			m_Tris [15] = 2;
			m_Tris [16] = 5;
			m_Tris [17] = 4;

			//Face 2
			m_Tris [18] = 5;
			m_Tris [19] = 2;
			m_Tris [20] = 3;
		
			m_Tris [21] = 3;
			m_Tris [22] = 2;
			m_Tris [23] = 0;
		}


		// Create mesh
		m_MeshFilter.mesh = new Mesh ();
		m_MeshFilter.mesh.vertices = m_Verts;
		m_MeshFilter.mesh.SetTriangles (m_Tris, 0);
		m_MeshFilter.mesh.RecalculateBounds ();
		m_MeshFilter.mesh.RecalculateNormals();
		m_MeshFilter.mesh.Optimize ();
	}

	public void UpdateTri( Vector3 vert0, Vector3 vert1, Vector3 vert2, float thickness )
	{
		m_BaseVerts [0] = vert0;
		m_BaseVerts [1] = vert1;
		m_BaseVerts [2] = vert2;

		Vector3 side1 = vert1 - vert0;
		Vector3 side2 = vert2 - vert0;
		m_Normal = Vector3.Cross(side1, side2).normalized;

		m_Thickness = thickness;
				
		UpdateThickness ();

		m_MeshFilter.mesh.vertices = m_Verts;
	}

	void UpdateThickness()
	{
		if ( m_Extrusion == Extrusion.Center )
		{
			m_Verts [0] = m_BaseVerts [0] - (m_Normal * m_Thickness * .5f);
			m_Verts [1] = m_BaseVerts [1] - (m_Normal * m_Thickness * .5f);
			m_Verts [2] = m_BaseVerts [2] - (m_Normal * m_Thickness * .5f);
			
			// Find m_Normal of tri
			
			
			m_Verts [3] = m_BaseVerts [0] + (m_Normal * m_Thickness * .5f);
			m_Verts [4] = m_BaseVerts [1] + (m_Normal * m_Thickness * .5f);
			m_Verts [5] = m_BaseVerts [2] + (m_Normal * m_Thickness * .5f);
		}
		else if ( m_Extrusion == Extrusion.Out )
		{
			m_Verts [0] = m_BaseVerts [0];
			m_Verts [1] = m_BaseVerts [1];
			m_Verts [2] = m_BaseVerts [2];
			
			// Find m_Normal of tri
			
			
			m_Verts [3] = m_BaseVerts [0] + (m_Normal * m_Thickness);
			m_Verts [4] = m_BaseVerts [1] + (m_Normal * m_Thickness);
			m_Verts [5] = m_BaseVerts [2] + (m_Normal * m_Thickness);
		}
		else if ( m_Extrusion == Extrusion.In )
		{
			m_Verts [0] = m_BaseVerts [0];
			m_Verts [1] = m_BaseVerts [1];
			m_Verts [2] = m_BaseVerts [2];
			
			// Find m_Normal of tri
			
			
			m_Verts [3] = m_BaseVerts [0] - (m_Normal * m_Thickness);
			m_Verts [4] = m_BaseVerts [1] - (m_Normal * m_Thickness);
			m_Verts [5] = m_BaseVerts [2] - (m_Normal * m_Thickness);
		}
		else if ( m_Extrusion == Extrusion.None )
		{
			m_Verts [0] = m_BaseVerts [0];
			m_Verts [1] = m_BaseVerts [1];
			m_Verts [2] = m_BaseVerts [2];
		}

	}


	void OnDrawGizmos()
	{
		if (!Application.isPlaying)
			return;

		for (int i = 0; i < m_Verts.Length; i++)
		{
			Gizmos.DrawSphere( m_Verts[i], .001f );
		}

		//Vector3 center = ( transform.TransformPoint( m_BaseVerts [0] ) + transform.TransformPoint( m_BaseVerts [1] ) + transform.TransformPoint( m_BaseVerts [2] ) ) / 3f;
		//Gizmos.DrawLine (center, center + TransformedNormal);
	}
}
