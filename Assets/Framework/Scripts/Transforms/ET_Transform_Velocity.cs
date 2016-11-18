using UnityEngine;
using System.Collections;

public class ET_Transform_Velocity : MonoBehaviour
{
	public Vector3	m_Velocity;
	
	public float m_VelocityNormalizationRange = 30;
	public float m_NormalizedSpeed;

	public float m_Smoothing = 12;

	public Vector3 Velocity 
	{
		get
		{
			return m_Velocity;
		}
	}

	Transform m_Tform;

	Vector3 m_CurrentPos;
	Vector3 m_PrevPos;

	void Start()
	{
		m_Tform = transform;
	}

	// Update is called once per frame
	void Update ()
	{
		m_CurrentPos = m_Tform.position;

		if( m_Smoothing == 0 )
			m_Velocity = ( m_CurrentPos - m_PrevPos ) / Time.deltaTime;
		else
			m_Velocity = Vector3.Lerp ( m_Velocity, ( m_CurrentPos - m_PrevPos ) / Time.deltaTime, Time.deltaTime * m_Smoothing );


		m_NormalizedSpeed = m_Velocity.magnitude / m_VelocityNormalizationRange;

		
		m_PrevPos = m_Tform.position;
	}

	void OnDrawGizmos()
	{
	}
}
