using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ranged_Float 
{
    // The range you want to normalize your value to
    public Vector2 m_Range = new Vector2( 0, 1 );

    // Maximum absolute velocity
    public float m_MaxAbsVelocity = 1;

    // The raw input value/ Only update once per frame otherwise the delta will get screwy
    private float m_RawValue;

    bool m_Registered = false;

    public Ranged_Float()
    {
    }

    public Ranged_Float( string name, UnityEngine.UI.Text textGUI )
    {
        m_Name = name;
        m_Label = textGUI;
    }

    public Ranged_Float(string name, GUI_RangedFloatGraph rfGUI )
    {
        m_Name = name;

        if( rfGUI != null )
            rfGUI.Initialize(this);
    }

    public float RawValue
    {
        get { return m_RawValue; }
        set 
        {
            if( !m_Registered )
            {
                Ranged_Float_Manager.Instance.AddRangedFloat(this);
                m_Registered = true;
            }

            // calculate deltatime
            float deltaTime = Time.time - m_LastUpdateTime;
            // Calculate delta since last update
            m_RawVelocity = (value - m_RawValue) / deltaTime;
            // Set the raw value
            m_RawValue = value;
            // Calculate normalized value based on updated raw value and range
            m_NormalizedValue = m_RawValue.ScaleTo01(m_Range.x, m_Range.y);

            float prevNormVel = m_NormalizedVelocity;
            // Calculate normalized velocity. May need to have its own velocity range. Check code it's here just needs a think
            //m_NormalizedVelocity = m_RawVelocity.ScaleTo01Abs(m_Range.x, m_Range.y);
            m_NormalizedVelocity = m_RawVelocity.Scale(-m_MaxAbsVelocity, m_MaxAbsVelocity, -1, 1);

            // Not sure this is right
            m_NormalizedDelta = ( m_NormalizedVelocity - prevNormVel ) * deltaTime;

            // Set update time in case you dont update the value each frame
            m_LastUpdateTime = Time.time;

            if( m_AdjustRange )
            {
                if (m_RawValue < m_Range.x) m_Range.x = m_RawValue;
                else if (m_RawValue > m_Range.y) m_Range.y = m_RawValue;

                if (Mathf.Abs(m_RawVelocity) > m_MaxAbsVelocity) m_MaxAbsVelocity = Mathf.Abs( m_RawVelocity );
            }
            
            if( m_Label != null )
            {
                m_Label.text = m_Name + ": " + m_NormalizedValue.ToDoubleDecimalString();
            }
        }
    }
    
    // Returns the normalized value 
    public float m_NormalizedValue;

    // The raw delta
    float m_RawVelocity;
    
    // Returns the delta of the normalized value. Outputs as -1 to 1
    public float m_NormalizedVelocity;

    public float m_NormalizedDelta;

    // The last update time used for calculating the delta time
    float m_LastUpdateTime;

    // Flags weather to adjust the range if the value sits outside of the established range
    public bool m_AdjustRange = false;

    // Name for gui
    public string m_Name;

    // GUI text
    public UnityEngine.UI.Text m_Label;
}


