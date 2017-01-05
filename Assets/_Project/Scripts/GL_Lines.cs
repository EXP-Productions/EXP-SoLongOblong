using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GL_Lines : MonoBehaviour 
{
    public class Line
    {
        public Vector3 m_Start;
        public Vector3 m_End;
        public Color m_Col;
        public bool m_Draw = true;

        public Line( Color col, Vector3 v1, Vector3 v2 )
        {
            m_Col = col;
            m_Start = v1;
            m_End = v2;
        }
    }

    List<Line> m_Lines = new List<Line>();

    public List<Line> Lines
    {
        get { return m_Lines; }
        set { m_Lines = value; }
    }

    Material m_Mat;

    public Transform m_ParentTransform;

	// Use this for initialization
	void Start () 
    {
        m_Mat = new Material
        (
            "Shader \"Lines/Wireframe\" { Properties { _Color (\"Main Color\", Color) = (1,1,1,1) } SubShader { Pass { " +
            "ZWrite off " +
            "ZTest LEqual " +
            "Blend SrcAlpha OneMinusSrcAlpha " +
            "Lighting Off " +
            "Offset -1, -1 " +
            "Color[_Color] }}}"
        );
	}
	
	// Update is called once per frame
	void Update () 
    {
	
        /*
         *  GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex(startVertex);
        GL.Vertex(new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0));
        GL.End();
         */
	}

    protected virtual void OnRenderObject()
    {
        GL.PushMatrix();
        m_Mat.SetPass(0);

        GL.Begin(GL.LINES);

        for (int i = 0; i < m_Lines.Count; i++)
        {
            if (m_Lines[i].m_Draw)
            {
                GL.Color(m_Lines[i].m_Col);

                if ( m_ParentTransform != null )
                {
                    GL.Vertex( m_ParentTransform.TransformPoint( m_Lines[i].m_Start ) );
                    GL.Vertex( m_ParentTransform.TransformPoint( m_Lines[i].m_End ) );
                }
                else
                {
                    GL.Vertex(m_Lines[i].m_Start);
                    GL.Vertex(m_Lines[i].m_End);
                }                
            }
        }
       
        GL.End();

        GL.PopMatrix();
    }	
}
