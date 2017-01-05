using UnityEngine;
using System.Collections;

public class GL_Polygon : MonoBehaviour
{
	public Shape_Polygon 	m_Polygon;
	public Vector3[] 		m_VertexPositions;

	public Material 		m_Mat;
	public Color 			m_Col = Color.white;	

	public float 			m_NormalizedInnerRadius = 0;


	void Awake()
	{
		if( m_Polygon == null )
			m_Polygon = new Shape_Polygon();
	}
	
	void Start()
	{
		m_Mat = new Material
        (
			"Shader \"Lines/Wireframe\" { Properties { _Color (\"Main Color\", Color) = (1,1,1,1) } SubShader { Pass { " + 
			"ZWrite off " + 
			"ZTest LEqual " +
			"Blend SrcAlpha OneMinusSrcAlpha " + 
			"Lighting Off " +
			"Offset -1, -1 " +
			"Color[_Color] }}}"
        );
		
		
		m_Polygon.CalculatePositions();
		CalculateVertexWorldPositions();
	}

	void Update()
	{
		m_Polygon.CalculatePositions();
		CalculateVertexWorldPositions();
	}

	void CalculateVertexWorldPositions()
	{
		if( m_NormalizedInnerRadius != 0 )
		{
			m_VertexPositions = new Vector3[ m_Polygon.m_NumberOfSides * 2 ];
			
			for( int i = 0; i < m_Polygon.m_NumberOfSides; i++ )
			{
				m_VertexPositions[ i ] = 	 							transform.TransformPoint( (Vector3)m_Polygon.NormalizedPositions[ i ]  );
				m_VertexPositions[ i + m_Polygon.m_NumberOfSides ] = 	transform.TransformPoint( (Vector3)m_Polygon.NormalizedPositions[ i ] * m_NormalizedInnerRadius  ) ;
			}
		}
		else
		{
			m_VertexPositions = new Vector3[ m_Polygon.m_NumberOfSides ];
			
			for( int i = 0; i < m_Polygon.m_NumberOfSides; i++ )
			{
				m_VertexPositions[ i ] = 	 							transform.TransformPoint( (Vector3)m_Polygon.NormalizedPositions[ i ]  );
			}
		}
	}

	void OnDrawGizmos() 
	{
		if( Application.isPlaying )
		{
			for( int i = 0; i < m_VertexPositions.Length; i++ )
			{
			//	Gizmos.DrawWireSphere( m_VertexPositions[ i ], .1f );
			}
		}
	}

	protected virtual void OnRenderObject()
	{   
		GL.PushMatrix();
		m_Mat.SetPass(0);
		GL.Color( m_Col );

		DrawPoly( m_Polygon, m_NormalizedInnerRadius );

	    GL.PopMatrix();
	}	

	void DrawPoly( Shape_Polygon polygon, float radiusInner )
	{
		//GL.Begin(GL.QUADS); // Triangle

		if( radiusInner == 0)
		{
			GL.Begin(GL.TRIANGLES); // Triangle
			for( var i = 0; i < polygon.m_NumberOfSides; i++ )
			{
				int index2 = i + 1;
				if( i == polygon.m_NumberOfSides - 1 )
					index2 = 0;

				GL.Vertex3 ( transform.position.x, transform.position.y, transform.position.z );    // Center Vert
				//GL.TexCoord2 (pos.x, 1-pos.y); 		// Center UV

				GL.Vertex3 ( m_VertexPositions[ i ].x, m_VertexPositions[ i ].y, m_VertexPositions[ i ].z );    // Center Vert
				//GL.TexCoord2 (pos.x, 1-pos.y); 		// Center UV

				GL.Vertex3 ( m_VertexPositions[ index2 ].x, m_VertexPositions[ index2 ].y, m_VertexPositions[ index2 ].z );    // Center Vert
				//GL.TexCoord2 (pos.x, 1-pos.y); 		// Center UV
			} 
			GL.End();	
		}
		else
		{
			GL.Begin(GL.QUADS); // Triangle
			for( var i = 0; i < polygon.m_NumberOfSides; i++ )
			{
				int index2 = i + 1;
				if( i == polygon.m_NumberOfSides - 1 )
					index2 = 0;
				
				GL.Vertex3 ( m_VertexPositions[ i ].x, m_VertexPositions[ i ].y, m_VertexPositions[ i ].z );    // Center Vert
				//GL.TexCoord2 (pos.x, 1-pos.y); 		// Center UV
				
				GL.Vertex3 ( m_VertexPositions[ index2 ].x, m_VertexPositions[ index2 ].y, m_VertexPositions[ index2 ].z );    // Center Vert
				//GL.TexCoord2 (pos.x, 1-pos.y); 		// Center UV

				GL.Vertex3 ( m_VertexPositions[ index2 + m_Polygon.m_NumberOfSides ].x, m_VertexPositions[ index2 + m_Polygon.m_NumberOfSides ].y, m_VertexPositions[ index2 + m_Polygon.m_NumberOfSides ].z );    // Center Vert
				//GL.TexCoord2 (pos.x, 1-pos.y); 		// Center UV

				GL.Vertex3 ( m_VertexPositions[ i + m_Polygon.m_NumberOfSides ].x, m_VertexPositions[ i + m_Polygon.m_NumberOfSides ].y, m_VertexPositions[ i + m_Polygon.m_NumberOfSides ].z );    // Center Vert
			//	GL.TexCoord2 (pos.x, 1-pos.y); 		// Center UV
			} 
			GL.End();	
		}
	}
}



