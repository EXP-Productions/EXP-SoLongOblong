using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class TriMeshTest1 : MonoBehaviour 
{
	public float m_Radius = .1f;
	public float m_Length = .1f;

	public Transform m_V1;
	public Transform m_V2;
	public Transform m_V3;

	Vector3 m_MidPoint;

	Vector3 m_CrossV1Left;
	Vector3 m_CrossV1Right;

	Vector3 m_Normal;

	public GameObject[] m_Cylinders;

	Vector3[] m_InnerTri1;
	Vector3[] m_InnerTri2;
	Vector3[] m_InnerTri3;


	Vector3 v1LeftTollerance;
	Vector3 v1RightTollerance;

	public float m_Angle1;
	public float m_Angle2;
	public float m_Angle3;

	public float offset01;

	public GameObject[] m_Edges;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_MidPoint = ( m_V1.position + m_V2.position + m_V3.position ) / 3f;

		Vector3 side1 = m_V2.position - m_V1.position;
		Vector3 side2 = m_V3.position - m_V1.position;
		m_Normal = Vector3.Cross(side1, side2).normalized;

		//m_CrossV1Left = Vector3.Cross( 	m_Normal, 	m_V2.position - m_V1.position ).normalized;
		//m_CrossV1Right = Vector3.Cross( m_V3.position - m_V1.position, m_Normal).normalized;


		for (int i = 0; i < 6; i++) 
		{
			m_Cylinders[i].transform.localScale = new Vector3( m_Radius * 2, m_Radius * 2, m_Length );
		}

		m_Cylinders [0].transform.position = m_V1.position;
		m_Cylinders [0].transform.LookAt (m_V2.position);
		m_Cylinders [1].transform.position = m_V1.position;
		m_Cylinders [1].transform.LookAt (m_V3.position);

		m_Cylinders [2].transform.position = m_V2.position;
		m_Cylinders [2].transform.LookAt (m_V1.position);
		m_Cylinders [3].transform.position = m_V2.position;
		m_Cylinders [3].transform.LookAt (m_V3.position);

		m_Cylinders [4].transform.position = m_V3.position;
		m_Cylinders [4].transform.LookAt (m_V1.position);
		m_Cylinders [5].transform.position = m_V3.position;
		m_Cylinders [5].transform.LookAt (m_V2.position);


		m_InnerTri1 = FindInnerVerts ( m_V1.position, m_V2.position, m_V3.position, m_Radius, m_Length );
		m_InnerTri2 = FindInnerVerts ( m_V2.position, m_V3.position, m_V1.position, m_Radius, m_Length );
		m_InnerTri3 = FindInnerVerts ( m_V3.position, m_V1.position, m_V2.position, m_Radius, m_Length );

		//offset01 = Vector3.Distance (m_V1.position, m_InnerTri1 [3]);
		offset01 = ( 90 / m_Angle1 ) * ( m_Radius * 1.2f );

		print (offset01 / m_Radius);

		float length = (m_InnerTri1 [3] - m_InnerTri2 [4]).magnitude;
		m_Edges [0].transform.position = m_V1.position + (( m_V2.position - m_V1.position ).normalized * offset01 );
		m_Edges [0].transform.localScale = new Vector3 (m_Radius / 2f, m_Radius / 2f, 1);
		m_Edges [0].transform.LookAt (m_InnerTri2[4]); 

		m_Angle1 = Vector3.Angle (m_V1.position - m_V2.position, m_V1.position - m_V3.position);
		m_Angle2 = Vector3.Angle (m_V2.position - m_V1.position, m_V2.position - m_V3.position);
		m_Angle3 = Vector3.Angle (m_V3.position - m_V1.position, m_V3.position - m_V2.position);

	}

	public float m_MinAngle = 20f;

	void OnDrawGizmos()
	{
		//if (!Application.isPlaying)
		//	return;

		Gizmos.color = new Color (1, 1, 1, .2f);
		Gizmos.DrawSphere( m_V1.position, m_Radius );
		Gizmos.DrawSphere( m_V2.position, m_Radius );
		Gizmos.DrawSphere( m_V3.position, m_Radius );

		Gizmos.DrawLine (m_V1.position, m_V2.position);
		Gizmos.DrawLine (m_V2.position, m_V3.position);
		Gizmos.DrawLine (m_V3.position, m_V1.position);


		if( m_Angle1 > m_MinAngle )		Gizmos.color = Color.yellow;
		else 							Gizmos.color = Color.red;

		Gizmos.DrawLine (m_InnerTri1[0], m_InnerTri1[1]);
		Gizmos.DrawLine (m_InnerTri1[1], m_InnerTri1[2]);
		Gizmos.DrawLine (m_InnerTri1[2], m_InnerTri1[0]);


		if( m_Angle2 > m_MinAngle )		Gizmos.color = Color.yellow;
		else 							Gizmos.color = Color.red;

		Gizmos.DrawLine (m_InnerTri2[0], m_InnerTri2[1]);
		Gizmos.DrawLine (m_InnerTri2[1], m_InnerTri2[2]);
		Gizmos.DrawLine (m_InnerTri2[2], m_InnerTri2[0]);

		if( m_Angle3 > m_MinAngle )		Gizmos.color = Color.yellow;
		else 							Gizmos.color = Color.red;

		Gizmos.DrawLine (m_InnerTri3[0], m_InnerTri3[1]);
		Gizmos.DrawLine (m_InnerTri3[1], m_InnerTri3[2]);
		Gizmos.DrawLine (m_InnerTri3[2], m_InnerTri3[0]);


		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (m_InnerTri1[3], m_InnerTri2[4]);
		Gizmos.DrawLine (m_InnerTri2[3], m_InnerTri3[4]);
		Gizmos.DrawLine (m_InnerTri3[3], m_InnerTri1[4]);


	}

	public static Vector3[] FindInnerVerts( Vector3 v1, Vector3 v2, Vector3 v3, float radius, float length )
	{
		// v1 v2, v3, 1-2 edge tolerance, 1-3 edge tolerance. Edge tollerance is how fare the edge needs to be out given the radius of the edge to not interesect adjascent edges
		Vector3[] verts = new Vector3[5];
		Vector3 normal = Vector3.Cross( v2 - v1, v3 - v1 ).normalized;

		Vector3 crossLeft = Vector3.Cross( 	normal, v2 - v1 ).normalized;
		Vector3 crossRight = Vector3.Cross( v3 - v1, normal ).normalized;

		Vector3 innerV1;
		LineLineIntersection ( out innerV1,
                            	v1 + ( crossLeft *  radius ),
                            	(v2 - v1),
                                v1 + ( crossRight *  radius ),
                                (v3 - v1) );

		verts [0] = innerV1;
		verts [1] = v1 + (( v2 - v1 ).normalized * length ) + ( crossLeft * radius );
		verts [2] = v1 + (( v3 - v1 ).normalized * length ) + ( crossRight * radius );

		verts [3]  = v1 + ( ( v2 - v1 ).normalized * ( innerV1 - v1 ).magnitude );
		verts [4]  = v1 + ( ( v3 - v1 ).normalized * ( innerV1 - v1 ).magnitude );


		return verts;

	}

	public static Vector3[] FindInnerVerts( Vector3 v1, Vector3 v2, Vector3 v3, float radius, float length12,  float length13  )
	{
		// v1 v2, v3, 1-2 edge tolerance, 1-3 edge tolerance. Edge tollerance is how fare the edge needs to be out given the radius of the edge to not interesect adjascent edges
		Vector3[] verts = new Vector3[5];
		Vector3 normal = Vector3.Cross( v2 - v1, v3 - v1 ).normalized;
		
		Vector3 crossLeft = Vector3.Cross( 	normal, v2 - v1 ).normalized;
		Vector3 crossRight = Vector3.Cross( v3 - v1, normal ).normalized;
		
		Vector3 innerV1;
		LineLineIntersection ( out innerV1,
		                      v1 + ( crossLeft *  radius ),
		                      (v2 - v1),
		                      v1 + ( crossRight *  radius ),
		                      (v3 - v1) );
		
		verts [0] = innerV1;
		verts [1] = v1 + (( v2 - v1 ).normalized * length12 ) + ( crossLeft * radius );
		verts [2] = v1 + (( v3 - v1 ).normalized * length13 ) + ( crossRight * radius );
		
		verts [3]  = v1 + ( ( v2 - v1 ).normalized * ( innerV1 - v1 ).magnitude );
		verts [4]  = v1 + ( ( v3 - v1 ).normalized * ( innerV1 - v1 ).magnitude );
		
		
		return verts;
		
	}

	public static float FindOffSetGivenAdjascentEdges( Vector3 v1, Vector3 v2, Vector3 adjascentv3, float radius )
	{
		// pos1, pos2
		Vector3 normal = Vector3.Cross( v2 - v1, adjascentv3 - v1 ).normalized;
		
		Vector3 crossLeft = Vector3.Cross( 	normal, v2 - v1 ).normalized;
		Vector3 crossRight = Vector3.Cross( adjascentv3 - v1, normal ).normalized;
		
		Vector3 innerV1;
		LineLineIntersection ( out innerV1,
		                      v1 + ( crossLeft *  radius ),
		                      (v2 - v1),
		                      v1 + ( crossRight *  radius ),
		                      (adjascentv3 - v1) );		
			
		return ( v1 + ( ( v2 - v1 ).normalized * ( innerV1 - v1 ).magnitude )).magnitude;
	}

	public static Vector3[] FindInnerVertsForFace( Vector3 v1, Vector3 v2, Vector3 v3, float radius )
	{
		// v1 v2, v3
		Vector3[] verts = new Vector3[3];		
		verts [0] = FindInsetVert (v1, v2, v3, radius);
		verts [1] = FindInsetVert (v3, v1, v2, radius);
		verts [2] = FindInsetVert (v2, v3, v1, radius);

		return verts;	
	}

	public static Vector3 FindInsetVert( Vector3 v1, Vector3 v2, Vector3 v3, float radius )
	{
		// v1 v2, v3, l tolerance, r tolerance
		Vector3 normal = Vector3.Cross( v2 - v1, v3 - v1 ).normalized;
		
		Vector3 crossLeft = Vector3.Cross( 	normal, v2 - v1 ).normalized;
		Vector3 crossRight = Vector3.Cross( v3 - v1, normal ).normalized;
		
		Vector3 innerV1;
		LineLineIntersection ( out innerV1,
		                      v1 + ( crossLeft *  radius ),
		                      (v2 - v1),
		                      v1 + ( crossRight *  radius ),
		                      (v3 - v1) );		
		
		return innerV1;		
	}


	//Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
	//Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
	//same plane, use ClosestPointsOnTwoLines() instead.
	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){
		
		intersection = Vector3.zero;
		
		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
		Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
		
		float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
		
		//Lines are not coplanar. Take into account rounding errors.
		if((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f)){
			
			return false;
		}
		
		//Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
		float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
		
		if((s >= 0.0f) && (s <= 1.0f)){
			
			intersection = linePoint1 + (lineVec1 * s);
			return true;
		}
		
		else{
			return false;       
		}
	}
}
