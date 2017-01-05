using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ranged_Vector
{
    public Vector3 m_RawValue;


    /*
    // The range you want to normalize your vector magnitude to
    public Vector2 m_MagnitudeRange;

    // The raw input value
    private Vector3 m_RawValue;

    public Vector3 RawValue
    {
        get { return m_RawValue; }
        set
        {
            // calculate deltatime
            float deltaTime = Time.time - m_LastUpdateTime;

            Vector3 previousNormalizedValue = m_NormalizedValue;

            // Calculate delta since last update
            m_RawDelta = (value - m_RawValue) * deltaTime;
            // Set the raw value
            m_RawValue = value;
            // Calculate normalized value based on updated raw value and range
            m_NormalizedValue = m_RawValue.magnitude.ScaleTo01(m_MagnitudeRange.x, m_MagnitudeRange.y);
            // Calculate normalized delta
            m_NormalizedDelta = (m_NormalizedValue - previousNormalizedValue) * deltaTime;           
            // Set update time in case you dont update the value each frame
            m_LastUpdateTime = Time.time;
        }
    }

    // The raw delta
    public Vector3 m_RawDelta;

    // Returns the normalized value 
    public float m_NormalizedValue;

    // Returns the delta of the normalized value
    public Vector3 m_NormalizedDelta;

    // The last update time used for calculating the delta time
    float m_LastUpdateTime;

    */
}
