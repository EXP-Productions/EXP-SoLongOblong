using UnityEngine;
using System.Collections;

/// <summary>
/// OSC_FFT.
///  - Gets an fft float array over OSC. Used when you need to get mutiple OSC FFT's
/// </summary>
[AddComponentMenu("Ethno Tekh Framework/Managers/OSC FFT")]
public class FFT : MonoBehaviour
{
	public enum FFTRange
	{
		Low,
		LowAverage,
		Mid,
		MidAverage,
		High,
		HighAverage,
		Full,
	}
	
	Vector2 m_LowRange = new Vector2( 0 , 5 );
	Vector2 m_MidRange = new Vector2( 6 , 15 );
	Vector2 m_HighRange = new Vector2( 16 , 25 );

    float m_LowAverage;
    public float LowAverage { get { return m_LowAverage.ScaleTo01(m_AverageNormRange.x, m_AverageNormRange.y); } }
    float m_MidAverage;
    public float MidAverage { get { return m_MidAverage.ScaleTo01(m_AverageNormRange.x, m_AverageNormRange.y); } }
    float m_HighAverage;
    public float HighAverage { get { return m_HighAverage.ScaleTo01(m_AverageNormRange.x, m_AverageNormRange.y); } }

    public Vector2 m_AverageNormRange = new Vector2(0, 1);
	
	protected float[] 	m_RawSamples;           			
	public float[] 		m_SmoothedSamples;     	
	static float[] 		m_SmoothedOutput;  
	public int 			m_SampleCount = 32;   		
	private const int 	m_Frequency = 48000;		
		
	public float m_Scaler = 1;
	
	public float 	m_SmoothDownRate = 8;	
	public bool 	m_DrawGizmos = false;	
	public bool 	m_Debug = false;
	
	public bool 	m_ActiveInput = false;

    float           m_Volume;
    public float    Volume { get { return m_Volume; } }
    public float    m_VolumeScaler = 1;
	
	protected virtual void Start ()
	{
		m_RawSamples = 		new float[ m_SampleCount ];
		m_SmoothedSamples = new float[ m_SampleCount ];
			
		m_RawSamples = 		new float[m_SampleCount];
		m_SmoothedOutput = 	new float[m_SampleCount];

		//Audio_Manager.Instance.RegisterFFT( this );
	}
	
	protected virtual void Update()
	{
       




		float lowAverage = 0;
		for( int i = (int)m_LowRange.x; i < (int)m_LowRange.y; i++ )
		{
			lowAverage += m_RawSamples[ i ];
		}

		lowAverage = Mathf.Max( lowAverage, 0.001f );

		lowAverage = lowAverage / (m_LowRange.y - m_LowRange.x);
		m_LowAverage = Mathf.Lerp( m_LowAverage, lowAverage, Time.deltaTime * 20 );
		m_LowAverage = Mathf.Clamp01( m_LowAverage );
		
		float midAverage = 0;
		for( int i = (int)m_MidRange.x; i < (int)m_MidRange.y; i++ )
		{
			midAverage += m_RawSamples[ i ];
		}
		midAverage = Mathf.Max( midAverage, 0.001f );
		midAverage = midAverage / (m_MidRange.y - m_MidRange.x);
		m_MidAverage = Mathf.Lerp( m_MidAverage, midAverage, Time.deltaTime * 20 );
		m_MidAverage = Mathf.Clamp01( m_MidAverage );
		
		float highAverage = 0;
		for( int i = (int)m_HighRange.x; i < (int)m_HighRange.y; i++ )
		{
			highAverage += m_RawSamples[ i ];
		}
		highAverage = Mathf.Max( highAverage, 0.001f );
		highAverage = highAverage / (m_HighRange.y - m_HighRange.x);
		m_HighAverage = Mathf.Lerp( m_HighAverage, highAverage, Time.deltaTime * 20 );
		m_HighAverage = Mathf.Clamp01( m_HighAverage );
		
		if( m_SmoothDownRate != 0 )
		{				
			for(int i = 0; i < m_RawSamples.Length; i++)
			{
				if( m_RawSamples[i] < m_SmoothedSamples[i] )
					m_SmoothedSamples[i] = Mathf.Lerp(m_SmoothedSamples[i], m_RawSamples[i], m_SmoothDownRate * Time.deltaTime);
				else
					m_SmoothedSamples[i] = m_RawSamples[i];
				
			}
				
			for(int i = 0; i < m_RawSamples.Length; i++)
			{
				if( m_RawSamples[i] < m_SmoothedOutput[i] )
					m_SmoothedOutput[i] = Mathf.Lerp(m_SmoothedOutput[i], m_RawSamples[i], m_SmoothDownRate * Time.deltaTime);
				else
					m_SmoothedOutput[i] = m_RawSamples[i];
			}
		}
		else
		{
			for(int i = 0; i < m_RawSamples.Length; i++)
			{
				m_SmoothedSamples[i] = m_RawSamples[i];
			}
		}

        m_Volume = 0;
        // Calculate volume
        for (int i = 0; i < m_SmoothedOutput.Length; i++)
        {
            m_Volume += m_SmoothedOutput[i];
        }

        m_Volume /= m_RawSamples.Length;
        m_Volume *= m_VolumeScaler;

        if (float.IsNaN(m_Volume))
            m_Volume = 0;

        m_Volume = Mathf.Clamp01(m_Volume);
		
		m_ActiveInput = false;
		for(int i = 0; i < m_SmoothedSamples.Length; i++)
		{
			if( m_SmoothedSamples[i] != 0 )
			{
				m_ActiveInput = true;
				break;
			}
			
		}
	}
	
	
	public float GetSample( int index )
	{
		return m_SmoothedSamples[ index ] * m_Scaler;
	}
	
	public float GetRawSample( int index )
	{
		return m_RawSamples[ index ] * m_Scaler;
	}
	
	public float GetSampleFromNormalizedValue( float val )
	{
		if( m_SmoothedSamples == null ) return 0;
		if( m_SmoothedSamples.Length == 0 ) return 0;
		
		int sampleIndex = (int)(val * m_SampleCount);
		sampleIndex = Mathf.Clamp( sampleIndex, 0, m_SampleCount - 1 );
		float finalVal = m_SmoothedSamples[ sampleIndex ] * m_Scaler;
		
		if( m_Debug &&  finalVal == 0 ) print( val + "  " + finalVal );
		return finalVal;
	}
	
	public float GetSampleFromNormalizedValue( float val, FFTRange range )
	{
		if( m_SmoothedSamples == null ) return 0;
		if( m_SmoothedSamples.Length == 0 ) return 0;
							
		
		if( range == FFTRange.LowAverage ) return m_LowAverage;
		else if( range == FFTRange.MidAverage ) return m_MidAverage;
		else if( range == FFTRange.HighAverage ) return m_HighAverage;
		
		
		Vector2 indexRange = new Vector2( 0, m_RawSamples.Length );		
		
		if( range == FFTRange.Low )			indexRange = m_LowRange;
		else if( range == FFTRange.Mid )	indexRange = m_MidRange;
		else if( range == FFTRange.High )	indexRange = m_HighRange;


        int sampleIndex = (int)val.ScaleFrom01(indexRange.x, indexRange.y);
		
		sampleIndex = Mathf.Clamp( sampleIndex, 0, m_SampleCount - 1 );
		
		float finalVal = m_SmoothedSamples[ sampleIndex ] * m_Scaler;
		
		if( m_Debug &&  finalVal == 0 ) print( val + "  " + finalVal );
		return finalVal;
	}
	
	public float GetOutputSampleFromNormalizedValue( float val )
	{
		return m_SmoothedOutput[ (int) (val * m_SampleCount) ] * m_Scaler;
	}
	
	void OnDrawGizmos()
	{
		if(m_RawSamples == null)
			return;
		
		if( !m_DrawGizmos ) return; 
		
		for(int i = 0; i < m_RawSamples.Length; i++)
			Gizmos.DrawWireCube( transform.position + new Vector3(i * .1f, 10,0), new Vector3(.1f, m_SmoothedSamples[i] * m_Scaler , .1f));
		
		for(int i = 0; i < m_RawSamples.Length; i++)
			Gizmos.DrawWireCube(transform.position + new Vector3(i * .1f, 12,0), new Vector3(.1f, m_RawSamples[i] * m_Scaler, .1f));
		
		for(int i = 0; i < m_RawSamples.Length; i++)
			Gizmos.DrawWireCube(transform.position + new Vector3(i * .1f, 14,0), new Vector3(.1f,  m_RawSamples[i] * m_Scaler, .1f));
		
	}
}
