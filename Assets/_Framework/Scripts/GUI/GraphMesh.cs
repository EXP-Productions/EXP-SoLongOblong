using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( CanvasRenderer ))]
[RequireComponent(typeof(RectTransform))]
public class GraphMesh : MonoBehaviour 
{	
	public int 	m_Resolution = 30;
	float[]		m_NormalizedGraphValues;
	
	//MeshFilter 	m_MeshFilter;	
	Mesh 		m_Mesh;
	
	Vector3[] 	m_Verts;
	int[] 		m_Tris;
	Vector2[] 	m_UVs;

    List<UIVertex> pVertexList;
	
	public Material m_Mat;
	
	bool m_Initialized = false;

    CanvasRenderer pCanvasRenderer;
    RectTransform pRectTransform;

    void Start ()
	{
		if(m_Initialized) return;

        pCanvasRenderer =   gameObject.GetComponent<CanvasRenderer>();
        pCanvasRenderer.SetMaterial( m_Mat , null);
        pRectTransform =    gameObject.GetComponent<RectTransform>();
		
		//m_MeshFilter = gameObject.AddComponent< MeshFilter >();
		//gameObject.AddComponent< MeshRenderer >();

		m_NormalizedGraphValues = new float[ m_Resolution ];
		for( int i = 0; i < m_NormalizedGraphValues.Length; i++ )
		{
			m_NormalizedGraphValues[ i ] = 1;
		}
		
		//if( m_Mat != null )		renderer.material = m_Mat;			
		
		m_Mesh = new Mesh();			
		
		CreateMesh();	
		
		m_Initialized = true;
	}

	public void AddValue( float val )
	{		
		if( m_NormalizedGraphValues == null || m_NormalizedGraphValues.Length < m_Resolution )
			m_NormalizedGraphValues = new float[ m_Resolution ];
		
		for( int p = m_NormalizedGraphValues.Length-1; p > 0; p-- )
			m_NormalizedGraphValues[ p ] = m_NormalizedGraphValues[ p-1 ];
		
		m_NormalizedGraphValues[ 0 ] = val;

		UpdateVerts();

        pCanvasRenderer.Clear();
        pCanvasRenderer.SetVertices( pVertexList );
	}

	void Update()
	{
		AddValue( Random.Range( 0f, 1f ) );
	}
	
	void CreateMesh()
	{		
		// Set mesh properties
		m_Verts = new Vector3[ m_Resolution * 2 ]; 
		int triCount = ( m_Resolution - 1 ) * 2;
		m_Tris = new int[ triCount * 3 ];
		m_UVs = new Vector2[ m_Resolution * 2 ];

        // UI verts for ui canvas rendering
       // m_UIVerts = new UIVertex[m_Resolution * 2];
		
		float distanceBetweenVerts = 1 / ( (float)m_Resolution - 1 );		
		
		//Create verts and add them to the vert array		
		for( int i = 0; i < m_Verts.Length/2; i++ )
		{
			float xPos = i * distanceBetweenVerts;
			float yPos = 0;

			Vector3 vert = new Vector3( xPos, yPos, 0 );

            // Add UI vert
           // m_UIVerts[i].position = new Vector3(xPos, yPos, 0);

			m_Verts[i] = vert;			
			m_UVs[i] = new Vector2( xPos, yPos );			
		}

		//Create verts and add them to the vert array		
		for( int i = m_Verts.Length/2; i < m_Verts.Length; i++ )
		{
			float xPos = ( i - (m_Verts.Length/2) ) * distanceBetweenVerts;
			float yPos = m_NormalizedGraphValues[ i - (m_Verts.Length/2) ];
			
			Vector3 vert = new Vector3( xPos, yPos, 0 );
			
			m_Verts[i] = vert;			
			m_UVs[i] = new Vector2( xPos, yPos );			
		}

		//print( "Tris: " + m_Tris.Length );
		// Assign triangles
		for( int i = 0; i < (triCount/2); i++ )
		{			
			m_Tris[ (i * 3) ] = i;			
			m_Tris[ (i * 3) + 1 ] = i + 1;			
			m_Tris[ (i * 3) + 2 ] = i + (m_Verts.Length/2);	
		}

		// Assign triangles
		for( int i = triCount/2; i < triCount; i++ )
		{			
			m_Tris[ (i * 3) ] = 	( i - (triCount/2) ) + 1;			
			m_Tris[ (i * 3) + 1 ] = (m_Verts.Length/2) + ( i - (triCount/2) ) + 1;			
			m_Tris[ (i * 3) + 2 ] = (m_Verts.Length/2) + ( i - (triCount/2) );	
		}
		
		m_Mesh.vertices = m_Verts;
		m_Mesh.triangles = m_Tris;
		m_Mesh.uv = m_UVs;
			
		m_Mesh.RecalculateNormals();
		m_Mesh.RecalculateBounds();


        UIVertex uiVert;
        int[] indices = m_Mesh.GetIndices(0);
        pVertexList = new List<UIVertex>();

        for (int i = 0; i < indices.Length; i += 3)
        {
            for (int p = 0; p < 3; p++)
            {
                uiVert = new UIVertex();
                uiVert.position = m_Mesh.vertices[indices[i + p]];
                uiVert.normal = m_Mesh.normals[indices[i + p]];
                uiVert.uv0 = m_Mesh.uv[indices[i + p]];
                //do the same for tangent, uv1, and color if you need to.
                pVertexList.Add(uiVert);
            }            
        }

        //This just adds the last vertex twice to fit the quad format.
        pVertexList.Add( pVertexList[ pVertexList.Count - 1 ] );
	}   
	
	void UpdateVerts()
	{		
		if(!m_Initialized) Start();
		
		for( int i = m_Verts.Length/2; i < m_Verts.Length; i++ )
		{
			m_Verts[i].y = m_NormalizedGraphValues[ i - (m_Verts.Length/2) ];					
		}
		
		m_Mesh.vertices = m_Verts;
	}
}
