using UnityEngine;
using System.Collections;

public class ET_Transform_Mover : MonoBehaviour 
{
    public float m_Speed;
    public float m_Angle;

    public Vector2 m_Range;
   

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        m_Angle += m_Speed * Time.deltaTime;
        transform.SetWorldY(Mathf.Sin(m_Angle ).Scale( -1f, 1f, m_Range.x, m_Range.y ) );
        transform.SetWorldX(Mathf.Cos(m_Angle).Scale(-1f, 1f, m_Range.x, m_Range.y));
       // transform.SetWorldZ(Mathf.Cos(m_Angle).Scale(-1f, 1f, m_Range.x, m_Range.y));
	}
}
