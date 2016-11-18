using UnityEngine;

/// <summary>
/// Bounds 3D
/// - Provides a definition for a rectangular 3D bounding volumes
/// - Lets you get a normalized postion within a volume, clamp to a volume...
/// </summary>

[System.Serializable]
public class Bounds3D
{
	public float m_Left;
	public float m_Right;
	public float m_Top;
	public float m_Bottom;
	public float m_Near;
	public float m_Far;

    public float m_LeftDelta;
    public float m_RightDelta;
    public float m_TopDelta;
    public float m_BottomDelta;
    public float m_NearDelta;
    public float m_FarDelta;
    public float m_VolumeDelta;

    float m_LeftPrev;
    float m_RightPrev;
    float m_TopPrev;
    float m_BottomPrev;
    float m_NearPrev;
    float m_FarPrev;
    float m_VolumePrev;

	Vector3 m_Center;
	
	public float Width{ 		get{ return Mathf.Abs( m_Right - m_Left ); 	} }
	public float Height{ 		get{ return Mathf.Abs( m_Top - m_Bottom ); 	} }
	public float Depth{ 		get{ return Mathf.Abs( m_Near - m_Far ); 	} }
	public float Volume{		get{ return Width * Height * Depth;	} }	
	public Vector3 Dimensions{	get{ return new Vector3( Width, Height, Depth ); }  }	

	public Vector3 Center
	{
		get
		{
			m_Center.x = Mathf.Lerp( m_Left, m_Right, .5f );
			m_Center.y = Mathf.Lerp( m_Bottom, m_Top, .5f );
			m_Center.z = Mathf.Lerp( m_Near, m_Far, .5f );
			
			return m_Center;
		}		
	}

    public void UpdateDeltas( bool UseFixedDelta)
    {
        float time = Time.deltaTime;
        if (UseFixedDelta)
            time = Time.fixedDeltaTime;

        m_LeftDelta = (m_LeftPrev - m_Left) / time;
        m_RightDelta = (m_RightPrev - m_Right) / time;
        m_TopDelta = (m_TopPrev - m_Top) / time;
        m_BottomDelta = (m_BottomPrev - m_Bottom) / time;
        m_NearDelta = (m_NearPrev - m_Near) / time;
        m_FarDelta = (m_FarPrev - m_Far) / time;
        m_VolumeDelta = (m_VolumePrev - Volume) / time;

        m_LeftPrev = m_Left;
        m_RightPrev = m_Right;
        m_TopPrev = m_Top;
        m_BottomPrev = m_Bottom;
        m_NearPrev = m_Near;
        m_FarPrev = m_Far;
        m_VolumePrev = Volume;
    }

	public bool Contains( Vector3 pos )
	{
		//Vector3 normPos = GetNormalizedPosFromWorld( pos, 0 );

		//Debug.Log( pos );

		if( pos.x > m_Right || pos.x < m_Left )
			return false;

		if( pos.y > m_Top || pos.y < m_Bottom )
			return false;

		if( pos.z > m_Far || pos.z < m_Near )
			return false;

		return true;
	}
	
	public Vector3 GetWorldFromNormalized( Vector3 normalizedInput )
	{
		Vector3 pos = Vector3.zero;

		pos.x = normalizedInput.x.ScaleFrom01( m_Left, m_Right );
		pos.y = normalizedInput.y.ScaleFrom01( m_Bottom,  m_Top );
		pos.z = normalizedInput.z.ScaleFrom01( m_Near,  m_Far );
		
		return pos;
	}
	
	
	public Vector3 GetNormalizedPosFromWorld( Vector3 worldPos, float radius )
	{
		Vector3 pos = Vector3.zero;

		pos.x = worldPos.x.ScaleTo01( m_Left + radius, 	m_Right - radius );
		pos.y = worldPos.y.ScaleTo01( m_Bottom + radius,  	m_Top - radius );
		pos.z = worldPos.z.ScaleTo01( m_Near + radius,  	m_Far - radius );
		
		return pos;
	}
	
	public void FindBounds( Vector3[] positions )
	{		
		ResetBounds( positions[ 0 ] );
		
		for( int i = 0; i < positions.Length; i++ )
		{
			ExtendToInclude( positions[ i ] );			
		}
	}
	
	public Vector3 GetNormalizedSize( Vector3 normalizeRange )
	{
		Vector3 normSize = Vector3.zero;
		normSize.x = Width / normalizeRange.x;
		normSize.y = Height / normalizeRange.y;
		normSize.z = Depth / normalizeRange.z;
		
		return normSize;
	}

	public void ClampObjectToBounds( GameObject go, float radius )
	{
		Vector3 pos = go.transform.position;

		pos.x = Mathf.Clamp( pos.x, m_Left + radius, m_Right - radius );
		pos.y = Mathf.Clamp( pos.y, m_Bottom + radius, m_Top - radius );
		pos.z = Mathf.Clamp( pos.z, m_Near + radius, m_Far - radius );

		go.transform.position = pos;
	}
	
	public void ExtendToInclude( Vector3 pos )
	{
		if( pos.x < m_Left )		m_Left = pos.x;
		if( pos.x > m_Right )		m_Right = pos.x;
		
		if( pos.y < m_Bottom )		m_Bottom = pos.y;
		if( pos.y > m_Top )			m_Top = pos.y;
		
		if( pos.z < m_Near )		m_Near = pos.z;
		if( pos.z > m_Far )			m_Far = pos.z;
	}
	
	public void ResetBounds( Vector3 pos )
	{
		m_Left = 	pos.x;
		m_Right = 	pos.x;
		
		m_Top = 	pos.y;
		m_Bottom = 	pos.y;
		
		m_Near = 	pos.z;
		m_Far = 	pos.z;
	}	

	public void DrawGizmo( bool wire )
	{
        if( wire )	Gizmos.DrawWireCube( Center, new Vector3( Width, Height, Depth ) );
        else  Gizmos.DrawCube(Center, new Vector3(Width, Height, Depth));
	}
}