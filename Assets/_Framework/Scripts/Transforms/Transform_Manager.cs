using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Ethno Tekh Framework/Transform/Transform Manager")]
public class Transform_Manager : MonoBehaviour
{
	public enum PositionType
	{
		Local,
		World,
	}
	
	public enum ControlType
	{
		Offset,
		Base,
	}
	
	public PositionType m_PositionType = PositionType.Local;
	public PositionType PosType { set{ m_PositionType = value; } }
	
	
	// Base values
	Vector3 m_BaseWorldPosition;
	public Vector3 BaseWorldPos { get{ return m_BaseWorldPosition; } set{ m_BaseWorldPosition = value; } }
	
	public Vector3 m_BaseLocalPosition;
	public Vector3 BaseLocalPosition { get{ return m_BaseLocalPosition; } set{ m_BaseLocalPosition = value; } }
	
	Quaternion m_BaseWorldRotation;
	public Quaternion BaseWorldRotation { get{ return m_BaseWorldRotation; }  set{ m_BaseWorldRotation = value; }}
	
	Quaternion m_BaseLocalRotation;
	public Quaternion BaseLocalRotation { get{ return m_BaseLocalRotation; } set{ m_BaseLocalRotation = value; } }
	
	public Vector3 m_BaseLocalScale;
	public Vector3 BaseLocalScale{ get{ return m_BaseLocalScale; } set{ m_BaseLocalScale = value; } }
		
	
	// Offset values
	public Vector3 m_OffsetPosition;
	public Vector3 OffsetPosition { set{ m_OffsetPosition = value; } get{return m_OffsetPosition;}}
		
	Quaternion m_OffsetRotation;
	public Quaternion OffsetRotation { set{ m_OffsetRotation = value; } get{return m_OffsetRotation;}}
	
	public Vector3 m_OffsetScale;
	public Vector3 OffsetScale { set{ m_OffsetScale = value; } get{return m_OffsetScale;}}
	
	
	// Target values
	public Vector3 WorldTargetPos
	{
		get { return m_BaseWorldPosition + m_OffsetPosition; } 
		set { m_OffsetPosition = value -  m_BaseWorldPosition; }
	}
	
	public Vector3 LocalTargetPos
	{
		get { return m_BaseLocalPosition + m_OffsetPosition ; }
		set { m_OffsetPosition = value -  m_BaseLocalPosition; }
	}
	
	Quaternion WorldTargetRotation { get { return m_BaseWorldRotation * m_OffsetRotation; } }
	public Quaternion LocalTargetRotation
	{ 
		get { return m_BaseLocalRotation * m_OffsetRotation; }
		set { m_OffsetRotation = value * Quaternion.Inverse( m_BaseLocalRotation ); }
	}
	 
	public Vector3 TargetScale
	{
		get { return m_BaseLocalScale + m_OffsetScale; }
		set { m_OffsetScale = value - m_BaseLocalScale; }
	}
	
	
	// Smoothing values
	public bool m_UseSmoothing { get{ if( m_Smoothing == 0 ) return false; return true; }   set{ m_UseSmoothing = value; }}
	public float m_Smoothing = 0;
	
	public bool m_Update = true;
	public bool m_UpdateScale = true;
	public bool m_UpdateRotation = true;
	
	public Transform m_LookAt;
	
	void Start ()
	{
		SetBase();		
	}
	
	public void SetBase()
	{
		m_BaseWorldPosition = transform.position;
		m_BaseLocalPosition = transform.localPosition;
		m_BaseWorldRotation = transform.rotation;
		m_BaseLocalRotation = transform.localRotation;
		m_BaseLocalScale = transform.localScale;		
	}	
	
	public void Update ()
	{
		if(!m_Update)	return;			
				
		if( m_PositionType == PositionType.World )
		{
			// Lerp or move to new targets depending on smoothing
			if(m_UseSmoothing) transform.position = Vector3.Lerp( transform.position, WorldTargetPos, m_Smoothing * Time.deltaTime );
			else transform.position = WorldTargetPos;

			if( m_UpdateRotation )
			{
				if(m_UseSmoothing) transform.rotation = Quaternion.Slerp( transform.rotation, WorldTargetRotation, m_Smoothing * Time.deltaTime );
				else transform.rotation = WorldTargetRotation;
			}

			if( m_UpdateScale )
			{
				if(m_UseSmoothing) transform.localScale = Vector3.Lerp( transform.localScale, TargetScale, m_Smoothing * Time.deltaTime );
				else transform.localScale = TargetScale;
			}
		}
		else
		{
			Vector3 localOffsetDirection = transform.TransformDirection( m_OffsetPosition );
			
			if(m_UseSmoothing) transform.localPosition = Vector3.Lerp ( transform.localPosition, m_BaseLocalPosition +  localOffsetDirection , m_Smoothing * Time.deltaTime );
			else transform.localPosition =  m_BaseLocalPosition + localOffsetDirection;

			if( m_UpdateRotation )
			{
				if(m_UseSmoothing) transform.localRotation = Quaternion.Slerp( transform.localRotation, LocalTargetRotation, m_Smoothing * Time.deltaTime );
				else transform.localRotation = LocalTargetRotation;
			}

			if( m_UpdateScale )
			{
				if(m_UseSmoothing) transform.localScale = Vector3.Lerp( transform.localScale, TargetScale, m_Smoothing * Time.deltaTime );
				else transform.localScale = TargetScale;
			}
		}
		
		// Reset offsets
		m_OffsetPosition = Vector3.zero;
		m_OffsetRotation = Quaternion.identity;
		m_OffsetScale = Vector3.zero;
		
		if( m_LookAt != null ) transform.LookAt( m_LookAt.position );
	}

	public void IncrementXRotation( float increment )
	{
		Vector3 localRot = BaseLocalRotation.eulerAngles;
		localRot.x += increment;
		BaseLocalRotation = Quaternion.Euler( localRot );
	}

	public void IncrementYRotation( float increment )
	{
		Vector3 localRot = BaseLocalRotation.eulerAngles;
		localRot.y += increment;
		BaseLocalRotation = Quaternion.Euler( localRot );
	}
	public void IncrementZRotation( float increment )
	{
		Vector3 localRot = BaseLocalRotation.eulerAngles;
		localRot.z += increment;
		BaseLocalRotation = Quaternion.Euler( localRot );
	}

	
	public void ResetPosition()
	{
		transform.position = BaseWorldPos;
		transform.rotation = BaseWorldRotation;
		transform.localScale = BaseLocalScale;
	}
	
	/*
	public void Save( string externalSavePrefix )
	{
		string saveStringPrefix = externalSavePrefix + name;
		
		PlayerPrefs.SetInt( saveStringPrefix + " Position Type", (int)m_PositionType );
		
		PlayerPrefsX.SetVector3( saveStringPrefix + " Base World Position", m_BaseWorldPosition );
		PlayerPrefsX.SetVector3( saveStringPrefix + " Base Local Position", m_BaseLocalPosition );
		PlayerPrefsX.SetQuaternion( saveStringPrefix + " Base Rotation", m_BaseWorldRotation );
		PlayerPrefsX.SetVector3( saveStringPrefix + " Base Scale", m_BaseLocalScale );
	}
	
	
	public void Load( string externalSavePrefix )
	{
		string saveStringPrefix = externalSavePrefix + name;
		
		m_PositionType = (PositionType)PlayerPrefs.GetInt( saveStringPrefix + " Position Type");
		
		m_BaseWorldPosition = PlayerPrefsX.GetVector3( saveStringPrefix + " Base World Position" );
		transform.position = WorldTargetPos;
		
		m_BaseLocalPosition = PlayerPrefsX.GetVector3( saveStringPrefix + " Base Local Position" );		
		
		m_BaseWorldRotation = PlayerPrefsX.GetQuaternion( saveStringPrefix + " Base Rotation");
		transform.rotation = m_BaseWorldRotation;
		
		m_BaseLocalScale = PlayerPrefsX.GetVector3( saveStringPrefix + " Base Scale" );	
		transform.localScale = TargetScale;		
	}
	*/
}
