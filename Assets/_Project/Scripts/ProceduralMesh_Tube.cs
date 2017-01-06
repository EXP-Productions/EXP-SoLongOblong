using UnityEngine;
using System.Collections;

public class ProceduralMesh_Tube : MonoBehaviour 
{
	public Material m_Mat;
	Vector3[] vertices;
	bool m_Initialized = false;

	void Start()
	{
		if( !m_Initialized )
			Init ( 24, false );
	}

	void Update()
	{
		UpdateVerts ();
		UpdateMeshVerts ();
	}

	public float m_OutsideRadius = .1f;

	public float OutsideRadius
	{
		get
		{
			return m_OutsideRadius;
		}

		set
		{
			m_OutsideRadius = value;
			UpdateVerts();
			UpdateMeshVerts();
		}
	}

	public float 	m_InsideRadius = 0;

	public float InsideRadius
	{
		get
		{
			return m_InsideRadius;
		}
		
		set
		{
			m_InsideRadius = value;
			UpdateVerts();
			UpdateMeshVerts();
		}
	}
	float 			_2pi = Mathf.PI * 2f;
	int 			sideCounter;

	void UpdateVerts ()
	{
		int vert = 0;

		m_InsideRadius = Mathf.Clamp (InsideRadius, 0, OutsideRadius * .99f);

		
		// Bottom cap
		sideCounter = 0;
		while (vert < nbVerticesCap) {
			sideCounter = sideCounter == nbSides ? 0 : sideCounter;
			
			float r1 = (float)(sideCounter++) / nbSides * _2pi;
			float cos = Mathf.Cos (r1);
			float sin = Mathf.Sin (r1);
			vertices [vert] = new Vector3 (cos * (InsideRadius), 0f, sin * (InsideRadius));
			vertices [vert + 1] = new Vector3 (cos * (OutsideRadius), 0f, sin * (OutsideRadius));
			vert += 2;
		}
		
		// Top cap
		sideCounter = 0;
		while (vert < nbVerticesCap * 2) {
			sideCounter = sideCounter == nbSides ? 0 : sideCounter;
			
			float r1 = (float)(sideCounter++) / nbSides * _2pi;
			float cos = Mathf.Cos (r1);
			float sin = Mathf.Sin (r1);
			vertices [vert] = new Vector3 (cos * (InsideRadius), height, sin * (InsideRadius));
			vertices [vert + 1] = new Vector3 (cos * (OutsideRadius), height, sin * (OutsideRadius));
			vert += 2;
		}
		
		// Sides (out)
		sideCounter = 0;
		while (vert < nbVerticesCap * 2 + nbVerticesSides) {
			sideCounter = sideCounter == nbSides ? 0 : sideCounter;
			
			float r1 = (float)(sideCounter++) / nbSides * _2pi;
			float cos = Mathf.Cos (r1);
			float sin = Mathf.Sin (r1);
			
			vertices [vert] = new Vector3 (cos * (OutsideRadius), height, sin * (OutsideRadius));
			vertices [vert + 1] = new Vector3 (cos * (OutsideRadius), 0, sin * (OutsideRadius));
			vert += 2;
		}
		
		// Sides (in)
		sideCounter = 0;
		while (vert < vertices.Length) {
			sideCounter = sideCounter == nbSides ? 0 : sideCounter;
			
			float r1 = (float)(sideCounter++) / nbSides * _2pi;
			float cos = Mathf.Cos (r1);
			float sin = Mathf.Sin (r1);
			
			vertices [vert] = new Vector3 (cos * (InsideRadius), height, sin * (InsideRadius));
			vertices [vert + 1] = new Vector3 (cos * (InsideRadius), 0, sin * (InsideRadius));
			vert += 2;
		}
	}

	float height = 1f;
	int nbSides = 24;
	
	
	int nbVerticesCap ;
	int nbVerticesSides ;

	MeshFilter filter;
	Mesh mesh;
	public MeshRenderer m_Renderer;
	MeshCollider collider;


	public void Init( )
	{
		Init (24, true);
	}

	void Init( int numberOfSides, bool addMeshCollider )
	{
		filter = gameObject.AddComponent<MeshFilter> ();
		m_Renderer = gameObject.AddComponent<MeshRenderer> ();
		m_Renderer.material = m_Mat;

		mesh = filter.mesh;
		mesh.Clear ();
	
		height = 1f;
		nbSides = numberOfSides;
	
	
		nbVerticesCap = nbSides * 2 + 2;
		nbVerticesSides = nbSides * 2 + 2;
		#region Vertices
	
		// bottom + top + sides
		vertices = new Vector3[nbVerticesCap * 2 + nbVerticesSides * 2];
		UpdateVerts();
		#endregion
	
		#region Normales
	
		// bottom + top + sides
		Vector3[] normales = new Vector3[vertices.Length];
		int vert = 0;
	
		// Bottom cap
		while (vert < nbVerticesCap) {
			normales [vert++] = Vector3.down;
		}
	
		// Top cap
		while (vert < nbVerticesCap * 2) {
			normales [vert++] = Vector3.up;
		}
	
		// Sides (out)
		sideCounter = 0;
		while (vert < nbVerticesCap * 2 + nbVerticesSides) {
			sideCounter = sideCounter == nbSides ? 0 : sideCounter;
		
			float r1 = (float)(sideCounter++) / nbSides * _2pi;
		
			normales [vert] = new Vector3 (Mathf.Cos (r1), 0f, Mathf.Sin (r1));
			normales [vert + 1] = normales [vert];
			vert += 2;
		}
	
		// Sides (in)
		sideCounter = 0;
		while (vert < vertices.Length) {
			sideCounter = sideCounter == nbSides ? 0 : sideCounter;
		
			float r1 = (float)(sideCounter++) / nbSides * _2pi;
		
			normales [vert] = -(new Vector3 (Mathf.Cos (r1), 0f, Mathf.Sin (r1)));
			normales [vert + 1] = normales [vert];
			vert += 2;
		}
		#endregion
	
		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];
	
		vert = 0;
		// Bottom cap
		sideCounter = 0;
		while (vert < nbVerticesCap) {
			float t = (float)(sideCounter++) / nbSides;
			uvs [vert++] = new Vector2 (0f, t);
			uvs [vert++] = new Vector2 (1f, t);
		}
	
		// Top cap
		sideCounter = 0;
		while (vert < nbVerticesCap * 2) {
			float t = (float)(sideCounter++) / nbSides;
			uvs [vert++] = new Vector2 (0f, t);
			uvs [vert++] = new Vector2 (1f, t);
		}
	
		// Sides (out)
		sideCounter = 0;
		while (vert < nbVerticesCap * 2 + nbVerticesSides) {
			float t = (float)(sideCounter++) / nbSides;
			uvs [vert++] = new Vector2 (t, 0f);
			uvs [vert++] = new Vector2 (t, 1f);
		}
	
		// Sides (in)
		sideCounter = 0;
		while (vert < vertices.Length) {
			float t = (float)(sideCounter++) / nbSides;
			uvs [vert++] = new Vector2 (t, 0f);
			uvs [vert++] = new Vector2 (t, 1f);
		}
		#endregion
	
		#region Triangles
		int nbFace = nbSides * 4;
		int nbTriangles = nbFace * 2;
		int nbIndexes = nbTriangles * 3;
		int[] triangles = new int[nbIndexes];
	
		// Bottom cap
		int i = 0;
		sideCounter = 0;
		while (sideCounter < nbSides) {
			int current = sideCounter * 2;
			int next = sideCounter * 2 + 2;
		
			triangles [i++] = next + 1;
			triangles [i++] = next;
			triangles [i++] = current;
		
			triangles [i++] = current + 1;
			triangles [i++] = next + 1;
			triangles [i++] = current;
		
			sideCounter++;
		}
	
		// Top cap
		while (sideCounter < nbSides * 2) {
			int current = sideCounter * 2 + 2;
			int next = sideCounter * 2 + 4;
		
			triangles [i++] = current;
			triangles [i++] = next;
			triangles [i++] = next + 1;
		
			triangles [i++] = current;
			triangles [i++] = next + 1;
			triangles [i++] = current + 1;
		
			sideCounter++;
		}
	
		// Sides (out)
		while (sideCounter < nbSides * 3) {
			int current = sideCounter * 2 + 4;
			int next = sideCounter * 2 + 6;
		
			triangles [i++] = current;
			triangles [i++] = next;
			triangles [i++] = next + 1;
		
			triangles [i++] = current;
			triangles [i++] = next + 1;
			triangles [i++] = current + 1;
		
			sideCounter++;
		}
	
	
		// Sides (in)
		while (sideCounter < nbSides * 4) {
			int current = sideCounter * 2 + 6;
			int next = sideCounter * 2 + 8;
		
			triangles [i++] = next + 1;
			triangles [i++] = next;
			triangles [i++] = current;
		
			triangles [i++] = current + 1;
			triangles [i++] = next + 1;
			triangles [i++] = current;
		
			sideCounter++;
		}
		#endregion
	
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
	
		mesh.RecalculateBounds ();
		;


		if (addMeshCollider) 
		{
			collider = gameObject.AddComponent< MeshCollider > ();
			collider.convex = true;
		}

		m_Initialized = true;
	}
    

	void UpdateMeshVerts()
	{
		mesh.vertices = vertices;		
		mesh.RecalculateBounds ();
		;
	}
}
