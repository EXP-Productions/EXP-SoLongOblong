using UnityEngine;
using System.Collections;

/// <summary>
/// OSC_FFT.
///  - Gets an fft float array over OSC. Used when you need to get mutiple OSC FFT's
/// </summary>
[AddComponentMenu("Ethno Tekh Framework/Managers/OSC FFT")]
public class DataRacket_FFT : FFT
{
	public string m_OSCAddress;
	OSCListener m_OSCFFT;
	OSCListener m_OSCLoud;
	OSCListener m_OSCBright;
	OSCListener m_OSCNoise;
	OSCListener m_OSCHz;
	OSCListener m_OSCPitch;
	OSCListener m_OSCAttack;
	
	public bool m_Loud;
	public bool m_Bright;
	public bool m_Noise;
	public bool m_Hz;
	public bool m_Pitch;
	public bool m_Attack;
	
	//public bool m_ActiveInput = false;
	     
	//public float[] 		m_ChangeAmounts;  
	
	
	protected override void Start ()
	{
		base.Start();		
	
		m_OSCFFT = new OSCListener( m_OSCAddress, false );
		
//		m_ChangeAmounts = new float[ m_SampleCount ];
	}
	
	public void SetOSCAddress( string address )
	{
		m_OSCAddress = address;
		m_OSCFFT.m_Address = address;
	}
	
	protected override void Update()
	{				
		if( m_OSCFFT.Updated )
		{			
			for(int i = 0; i < m_RawSamples.Length; i++)
			{
				///float change = m_OSCFFT.GetDataAsFloat( i ) - m_RawSamples[i];
				//if( change > 0 )
				//	m_ChangeAmounts[i] = change;
				//else
				//	m_ChangeAmounts[i] = 0;

				m_RawSamples[i] = m_OSCFFT.GetDataAsFloat( i );
			}
		}
		else
		{
			for(int i = 0; i < m_RawSamples.Length; i++)
			{
				m_RawSamples[i] = Mathf.Lerp( m_RawSamples[i], 0, Time.deltaTime * 4 );
			}
		}
		
		m_ActiveInput = false;
		for(int i = 0; i < m_SmoothedSamples.Length; i++)
		{
			if( m_SmoothedSamples[i] != 0 )
			{
				m_ActiveInput = true;
				break;
			}
			
		}
		
		base.Update();
	}
	
	
	
}
