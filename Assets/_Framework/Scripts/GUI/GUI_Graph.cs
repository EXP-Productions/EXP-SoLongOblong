using UnityEngine;
using System.Collections;

public class GUI_Graph : MonoBehaviour 
{
    RectTransform m_ThisRect;
    public RectTransform m_Bar;

    RectTransform[] m_Bars;

    public int m_BarCount;
    float m_Width;
    public float m_BarSize;

    public float[] m_Values;
     
	// Use this for initialization
	void Start () 
    {
        m_ThisRect = gameObject.GetComponent<RectTransform>() as RectTransform;
        m_Width = m_ThisRect.rect.width;
        m_BarSize = m_Width / m_BarCount;

        m_Bars = new RectTransform[m_BarCount];

        m_Values = new float[m_BarCount];

        for (int i = 0; i < m_BarCount; i++)
        {
            RectTransform newBar = Instantiate(m_Bar) as RectTransform;
            newBar.transform.parent = transform;

            newBar.SetLocalX(m_BarSize * i);
            newBar.SetLocalY(0);
            newBar.SetScaleX(m_BarSize);
            newBar.SetScaleY(m_ThisRect.rect.height);

            m_Bars[i] = newBar;

            
        }
	}
	
	// Update is called once per frame
	void Update () 
    {

        for (int i = 0; i < m_BarCount; i++)
        {
            m_Bars[i].SetScaleY(m_ThisRect.rect.height * m_Values[i]);
        }
	}

    public void AddValue(float value)
    {
        for (int i = 1; i < m_Values.Length; i++)
        {
            m_Values[i - 1] = m_Values[i];
        }
        m_Values[m_Values.Length - 1] = value;
    }
}
