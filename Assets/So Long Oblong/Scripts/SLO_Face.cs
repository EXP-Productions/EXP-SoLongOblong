using UnityEngine;
using System.Collections;

/// <summary>
/// SL o_ face.
///  * Currently restricted to tris
/// * need to put in code to handle inner, outer, center offset and line it up with joint tabs
/// </summary>
public class SLO_Face : MonoBehaviour 
{
	TriMesh m_TriMesh;

	public float m_Index;

	SLO_Join m_Join0;
	public SLO_Join Join0 {	get {  return m_Join0;	}	}
	SLO_Join m_Join1;
	public SLO_Join Join1 {	get {  return m_Join1;	}	}
	SLO_Join m_Join2;
	public SLO_Join Join2 {	get {  return m_Join2;	}	}

	//public SLO_Edge m_Edge01;
	//public SLO_Edge m_Edge12;
//	public SLO_Edge m_Edge20;


	float m_Edge0Length;
	public float Edge0Length {		get {			return m_Edge0Length;		}	}
	float m_Edge1Length;
	public float Edge1Length {		get {			return m_Edge1Length;		}	}
	float m_Edge2Length;
	public float Edge2Length {		get {			return m_Edge2Length;		}	}

	float m_Angle01;
	public float Angle01 {		get {			return m_Angle01;		}	}
	float m_Angle12;
	public float Angle12 {		get {			return m_Angle12;		}	}
	float m_Angle20;
	public float Angle20 {		get {			return m_Angle20;		}	}

	Vector3[] m_InnerVerts;

	float m_Thickness = 3f;
	public float Thickness
	{
		get {	return m_Thickness;		}
		set
		{
			m_Thickness = value;
			m_TriMesh.m_Thickness = m_Thickness;
			// change thickness of tri here
			// change thickness of tabs
		}
	}

	public bool Draw
	{
		set
		{
			m_TriMesh.Draw = value;
		}
	}

	public float m_TabTolerance = 1;

	public Vector3 Normal {		get {			return m_TriMesh.TransformedNormal;		}	}

	// Use this for initialization
	public void Init ( SLO_Join j0, SLO_Join j1, SLO_Join j2, Material mat, float thickness, int index, TriMesh.Extrusion extrusion ) 
	{
		m_Join0 = j0;
		m_Join1 = j1;
		m_Join2 = j2;

		//m_Edge01 = FindEdgeConnectedToJoin ( m_Join0, m_Join1 );
		//m_Edge12 = FindEdgeConnectedToJoin ( m_Join1, m_Join2 );
		//m_Edge20 = FindEdgeConnectedToJoin ( m_Join2, m_Join0 );

		m_Index = index;

		gameObject.name = "Face " + index;

		m_TriMesh = gameObject.AddComponent< TriMesh >() as TriMesh;
		m_TriMesh.m_Mat = mat;

		m_Thickness = thickness;

		m_TriMesh.m_Thickness = m_Thickness * .001f;	

		// TODO: If edge is tube then the diameter should come from the edge to give a flush finish
		m_InnerVerts = new Vector3[3];
		m_InnerVerts = TriMeshTest1.FindInnerVertsForFace (j0.transform.position, j1.transform.position, j2.transform.position, ( ( m_Join0.Diameter / 2f) + m_TabTolerance ) * .001f );

		// need to put in code to handle inner, outer, center offset and line it up with joint tabs
		m_TriMesh.CreateTri( m_InnerVerts[0], m_InnerVerts[1], m_InnerVerts[2], extrusion );

		UpdateMeasurements ();

		// Make selectable
		gameObject.tag = "Selectable";
		gameObject.AddComponent< MeshCollider > ();
	}

	void OnMouseDown()
	{
		MeshEdgeSolver.Instance.SetSelectedObject (gameObject);
	}

	SLO_Edge FindEdgeConnectedToJoin( SLO_Join baseJoin, SLO_Join secondaryJoin   )
	{
		foreach (SLO_Edge edge in baseJoin.ConnectedEdges)
		{
			if( baseJoin == edge.m_Join0 )
			{
				if( secondaryJoin == edge.m_Join1 )
				{
					return edge;
				}
			}
			else if( baseJoin ==  edge.m_Join1 )
			{
				if( secondaryJoin == edge.m_Join0 )
				{
					return edge;
				}
			}
		}

		return null;
	}

	void UpdateMeasurements()
	{
		Vector3 edge0 = m_InnerVerts [0] - m_InnerVerts [1];
		Vector3 edge1 = m_InnerVerts [1] - m_InnerVerts [2];
		Vector3 edge2 = m_InnerVerts [2] - m_InnerVerts [0];

		m_Edge0Length = edge0.magnitude * 1000;
		m_Edge1Length = edge1.magnitude * 1000;
		m_Edge2Length = edge2.magnitude * 1000;

		m_Angle01 = Vector3.Angle ( edge0, edge1 );
		m_Angle12 = Vector3.Angle ( edge1, edge2 );
		m_Angle20 = Vector3.Angle ( edge2, edge0 );

	}

	void Update()
	{
		// Udpate inner verts // TODO: Only needs to happen when edge diameter is upated
		m_InnerVerts = new Vector3[3];
		m_InnerVerts = TriMeshTest1.FindInnerVertsForFace (m_Join0.transform.position, m_Join1.transform.position, m_Join2.transform.position, ( ( m_Join0.Diameter / 2f) + m_TabTolerance ) * .001f );

		m_TriMesh.UpdateTri (m_InnerVerts [0], m_InnerVerts [1], m_InnerVerts [2], m_Thickness * .001f );
		UpdateMeasurements ();

	}
}
