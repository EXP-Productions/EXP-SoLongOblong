using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
public class RealtimeMesh : MonoBehaviour 
{
	protected MeshFilter m_MeshFilter;
	
    protected Mesh m_Mesh;  
	public Mesh FullMesh{ get{ return m_Mesh; } }
		
    public Vector3[] m_CurrentVerts;
	public Vector3[] m_OriginalVerts;

	public Vector3[] OriginalVerts {
		get {
			return m_OriginalVerts;
		}
	}
	
	protected  Vector3[] m_CurrentNormals;
	protected  Vector3[] m_OriginalNormals;
	
	protected int[] m_CurrentTris;
	protected int[] m_OriginalTris;

	public int[] OriginalTris {
		get {
			return m_OriginalTris;
		}
	}
	
	public  Vector2[] m_CurrentUVs;
	protected  Vector2[] m_OriginalUVs;
	
	public  Color[] m_Colors;
    
		
	public bool m_UpdateNormals = true; 	
	public bool m_UpdateBounds; 
	
	public bool m_InvertNormals = false;
	
	
	public bool m_ManualStart = true;
	

	void Awake()
	{
		m_MeshFilter = (MeshFilter)GetComponent( typeof(MeshFilter) );
	}

    protected virtual void Start()
    {
		if( m_MeshFilter.mesh != null )
			m_Mesh = m_MeshFilter.mesh;		
		else
			m_Mesh = new Mesh();

		if( !m_ManualStart )
			InitializeMesh( m_MeshFilter.mesh );
    }

	public void InitializeMesh( Mesh mesh )
	{
        print("Initing mesh");
		if( m_Mesh != null && m_Mesh != mesh ) m_Mesh.Clear();

		m_Mesh = mesh;
		m_MeshFilter.mesh = mesh;
		m_OriginalVerts = new Vector3[ m_Mesh.vertices.Length ];
		m_Mesh.vertices.CopyTo( m_OriginalVerts, 0 );
		
		m_OriginalNormals = new Vector3[ m_Mesh.normals.Length ];
		m_Mesh.normals.CopyTo( m_OriginalNormals, 0 );
		
		m_OriginalTris = new int[ m_Mesh.triangles.Length ];
		m_Mesh.triangles.CopyTo( m_OriginalTris, 0 );
		
		m_OriginalUVs = new Vector2[ m_Mesh.uv.Length ];
		m_Mesh.uv.CopyTo( m_OriginalUVs, 0 );
		
		m_CurrentVerts = new Vector3[ m_Mesh.vertices.Length ];
		m_Mesh.vertices.CopyTo( m_CurrentVerts, 0 );
		
		m_CurrentNormals = new Vector3[ m_Mesh.normals.Length ];
		m_Mesh.normals.CopyTo( m_CurrentNormals, 0 );
		
		m_CurrentTris = new int[ m_Mesh.triangles.Length ];
		m_Mesh.triangles.CopyTo( m_CurrentTris, 0 );
		
		m_CurrentUVs = new Vector2[ m_Mesh.uv.Length ];
		m_Mesh.uv.CopyTo( m_CurrentUVs, 0 );
		
		//m_Colors.CopyTo( m_Mesh.colors, 0 );
		m_Mesh.colors = m_Colors;
		
		if( m_InvertNormals )
			InvertNormals();
		
		m_Mesh.RecalculateNormals();
		m_Mesh.RecalculateBounds();

		UpdateMesh();
	}
	
	void InvertNormals()
	{
		for( int i = 0; i < m_Mesh.normals.Length; i++ )
		{
			m_Mesh.normals[i] = -m_Mesh.normals[i];
		}
	}

	// Late update so it is called after any mesh modification
    public void UpdateMesh()
    {
        m_Mesh.vertices = m_CurrentVerts;
		m_Mesh.uv = m_CurrentUVs;	
		m_Mesh.triangles = m_CurrentTris;
		m_Mesh.colors = m_Colors;
		
		if( m_UpdateNormals )  m_Mesh.RecalculateNormals();
		 m_Mesh.RecalculateBounds();
    }
	
	public void SetVertCount( int count )
	{
		m_CurrentVerts = new Vector3[ count ];
		m_OriginalVerts = new Vector3[ count ];
		
		m_CurrentNormals = new Vector3[ count ];
		m_OriginalNormals = new Vector3[ count ];
		
		m_CurrentUVs = new Vector2[ count ];
		m_OriginalUVs = new Vector2[ count ];
		
		m_Colors = new Color[ count ];
	}
	
	public void ClearTris()
	{
		for( int i = 0; i < m_CurrentTris.Length; i++ )
			m_CurrentTris[i] = 0;
	}
	
	public void SetTriCount( int count )
	{
		m_OriginalTris = new int[ count * 3 ];
		m_CurrentTris = new int[ count * 3 ];
		print ("Set tri count to: " + m_CurrentTris.Length );
	}
    
    public void SetVertPosition( Vector3 vertPosition, int index )
    {
		//print (  index + "  " + m_CurrentVerts.Length);
        m_CurrentVerts[ index ] = vertPosition;        
    }
	
	 public void SetVertCol( int vertIndex, Color col )
    {
		//print (  index + "  " + m_CurrentVerts.Length);
        m_Colors[vertIndex] = col;        
    }
	
	public void SetTriIndexes( int triIndex, int vert1, int vert2, int vert3 )
    {
		int index = (triIndex * 3);
		//print (" Tri count: " + m_CurrentTris.Length + "   index: " +  index);
		
		m_CurrentTris[ index ] = vert1;
		m_CurrentTris[ index + 1 ] = vert2;
		m_CurrentTris[ index + 2 ] = vert3;
    }
	
	
	public void  Subdivide()
	{
		MeshHelper.Subdivide4( m_Mesh );
		m_MeshFilter.mesh = m_Mesh;
		Start();
	}
	
	protected void CenterPivot()
	{
		Vector3 midPoint = MidPoint();
		
		if( transform.position == midPoint )
		{
			print("Already centered");
			return;
		}
				
		
		for( int i = 0; i < m_CurrentVerts.Length; i++ )
			m_CurrentVerts[i] -= midPoint;
		
		UpdateMesh();
		
		transform.position = midPoint;
		
		
	}


	
	public Vector2 GetNormalizedVertPosOnXY( int index )
	{
		Vector2 normPos = Vector2.zero;

         normPos.x = m_CurrentVerts[ index ].x.ScaleTo01 ( m_Mesh.bounds.min.x, m_Mesh.bounds.max.x );
         normPos.y = m_CurrentVerts[index].y.ScaleTo01( m_Mesh.bounds.min.y, m_Mesh.bounds.max.y);
		
		return normPos;
	}
	
	public Vector2 GetNormalizedVertPosOnXZ( int index )
	{
		Vector2 normPos = Vector2.zero;

        normPos.x = m_CurrentVerts[index].x.ScaleTo01(m_Mesh.bounds.min.x, m_Mesh.bounds.max.x);
		normPos.y = m_CurrentVerts[ index ].z.ScaleTo01(  m_Mesh.bounds.min.z, m_Mesh.bounds.max.z );
		
		return normPos;
	}
	
	protected Vector3 MidPoint()
	{
		Vector3 midPoint = Vector3.zero;
		
		for( int i = 0; i < m_CurrentVerts.Length; i++ )	midPoint += transform.TransformPoint( m_CurrentVerts[i] );
		
		midPoint /= m_CurrentVerts.Length;
		
		return midPoint;
			
	}
	
	void Reset()
	{
		
	}
}
