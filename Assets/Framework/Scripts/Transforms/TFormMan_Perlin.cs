using UnityEngine;
using System.Collections;

[AddComponentMenu("Ethno Tekh Framework/Transform Manager/Perlin")]
[RequireComponent (typeof ( Transform_Manager ))]
[RequireComponent (typeof ( ET_NormalizedValue ))]
public class TFormMan_Perlin : MonoBehaviour
{
	Transform_Manager 		m_TransformManager;
	ET_NormalizedValue 	m_NormalizedObjVal;
		
	public Vector3 m_PerlinPositioning = Vector3.one;
	public float m_PerlinIncrement = .1f;
	public float m_PerlinIndex;
	
	public bool m_RandomizeAtStart = false; 
	
	PerlinNoise m_PerlNoise;
	public int m_OctNum = 2;
	public bool m_UsePosition = false;
	
	
	
	// Use this for initialization
	void Start ()
	{
		m_TransformManager = ( Transform_Manager )gameObject.GetComponent< Transform_Manager >();	
		
		m_NormalizedObjVal = gameObject.GetComponent< ET_NormalizedValue >();
		if( m_NormalizedObjVal == null )
			m_NormalizedObjVal = gameObject.AddComponent< ET_NormalizedValue >();
		
		if( m_RandomizeAtStart ) m_NormalizedObjVal.RandomizeValue();
		m_PerlinIndex = m_NormalizedObjVal.m_NormalizedObjectValue;		
		
		
		m_PerlNoise = new PerlinNoise( Random.Range( 0, 1000) );
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update the perlin index to lookup the perlin value
		m_PerlinIndex += m_PerlinIncrement * Time.deltaTime;
		
		Vector3 pos = transform.position;
		Vector3 perl;
		
		//m_PerlNoise
		perl.x = m_PerlNoise.FractalNoise3D( m_PerlinIndex, m_PerlinIndex + 5, m_PerlinIndex + 10, m_OctNum, 1, 1 );
		perl.y = m_PerlNoise.FractalNoise3D( m_PerlinIndex + 5, m_PerlinIndex + 10, m_PerlinIndex , m_OctNum, 1, 1 );
		perl.z = m_PerlNoise.FractalNoise3D( m_PerlinIndex + 10, m_PerlinIndex + 5, m_PerlinIndex, m_OctNum, 1, 1 );
		
		//perl.x = m_PerlNoise.FractalNoise3D( m_PerlinIndex, m_PerlinIndex, m_PerlinIndex, m_OctNum, m_Freq, m_Amp );
		//perl.y = 0;
		//perl.z = 0;
		//print ( perl );
		//print ( perl );
		// Set the transform offset based on the perlin value

		perl.Scale( m_PerlinPositioning );
		m_TransformManager.m_OffsetPosition += perl;	
	}
}
