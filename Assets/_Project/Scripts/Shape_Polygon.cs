using UnityEngine;
using System.Collections;


[System.Serializable]
public class Shape_Polygon
{
    public Vector2[] m_NormalizedPositions;													// Relative positions of vertacies
    public Vector2[] NormalizedPositions { get { return m_NormalizedPositions; } }

    public int m_NumberOfSides = 6;
    public int NumberOfSides { set { m_NumberOfSides = value; CalculatePositions(); } } // Recalculate positions if number of sides set

    public float m_StartAngle = 360;														// Angle the positions start at
    public float StartAngle { set { m_StartAngle = value; CalculatePositions(); } }		// Recalculate positions if start angle changed

    public float m_TotalAngle = 360;														// Total angle of all sides
    public float TotalAngle { set { m_TotalAngle = value; CalculatePositions(); } }		// Recalculate positions if total angle changed

    //public float   = 0;

    public void CalculatePositions()
    {
        m_NormalizedPositions = new Vector2[m_NumberOfSides];

        float angleBetweenDivisions = m_TotalAngle / ((float)m_NumberOfSides);

        for (int i = 0; i < m_NumberOfSides; i++)
        {
            Vector2 newPos = Vector3.zero;
            float angle = (m_StartAngle * Mathf.Deg2Rad) + ((i) * angleBetweenDivisions * Mathf.Deg2Rad);
            newPos.x = Mathf.Sin(angle);
            newPos.y = Mathf.Cos(angle);

            m_NormalizedPositions[i] = newPos;
        }
    }
}

