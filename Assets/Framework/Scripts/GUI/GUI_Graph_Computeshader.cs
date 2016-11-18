using UnityEngine;
using System.Collections;

public class GUI_Graph_Computeshader : MonoBehaviour
{
    public ComputeShader shader;
    RenderTexture tex;

    UnityEngine.UI.RawImage rawImage;

    public Vector2 m_Resolution = new Vector2( 256f, 256f );

    ComputeBuffer buffer;
    public float[] m_Values;

    public bool m_Test = false;
    float m_RandTestOffset = 0;

    void Start()
    {
        rawImage = gameObject.GetComponent<UnityEngine.UI.RawImage>() as UnityEngine.UI.RawImage;

        tex = new RenderTexture((int)m_Resolution.x, (int)m_Resolution.y, 0, RenderTextureFormat.ARGB32);
        tex.enableRandomWrite = true;
        tex.Create();

        m_Values = new float[(int)m_Resolution.x];
        for (int i = 0; i < m_Values.Length; i++)
        {
            m_Values[i] = 0;
        }

        m_RandTestOffset = Random.RandomRange(0f, 100f);

        buffer = new ComputeBuffer((int)m_Resolution.x, sizeof(float));
        buffer.SetData(m_Values);
        shader.SetBuffer(0, "buffer", buffer);
        shader.SetTexture(0, "tex", tex);
        shader.SetInt("texWidth", (int)m_Resolution.x);
        shader.SetInt("texHeight", (int)m_Resolution.y);
        shader.Dispatch(0, tex.width / 8, tex.height / 8, 1);

        rawImage.texture = tex;
    }

    public void AddValue( float value )
    {
        for (int i = 1; i < m_Values.Length; i++)
        {
            m_Values[i - 1] = m_Values[i];
        }
        m_Values[m_Values.Length - 1] = value;        
    }

    void Update()
    {
        if( m_Test )
            AddValue( Mathf.PerlinNoise(Time.time + m_RandTestOffset, Time.time + m_RandTestOffset) );

        buffer.SetData( m_Values );
        shader.SetTexture( 0, "tex", tex );       
        shader.Dispatch( 0, tex.width / 8, tex.height / 8, 1 );

        rawImage.texture = tex;
    }


    void OnDestroy()
    {
        buffer.Dispose();
        tex.Release();
    }
}