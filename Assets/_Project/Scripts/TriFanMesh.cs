using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Vert
{
    public int m_Index;
    public List<Tri> m_Tris = new List<Tri>();
    public Vector3 m_Pos { get { return m_T.position; } }    
    public Vector3 Normal
    {
        get
        {
            Vector3 normal = Vector3.zero;
            for (int i = 0; i < m_Tris.Count; i++)
            {
                normal += m_Tris[i].m_Normal;
            }

            normal /= m_Tris.Count;
            return normal.normalized;
        }
    }
    public Transform m_T;

    public Vert(Transform t, int index)
    {
        m_T = t;
        m_Index = index;
    }    

    public Vector3 GetExtrudedVert(float offset)
    {
        return m_Pos + (Normal * offset);
    }
}

[System.Serializable]
public class Tri
{
    public Vert m_V0, m_V1, m_V2;
    public Vector3 m_Normal;
    Vector3 m_Center;

    public Tri(Vert v0, Vert v1, Vert v2)
    {
        m_V0 = v0;
        m_V0.m_Tris.Add(this);

        m_V1 = v1;
        m_V1.m_Tris.Add(this);

        m_V2 = v2;
        m_V2.m_Tris.Add(this);

        m_Normal = Vector3.Cross(v0.m_Pos - v1.m_Pos, v0.m_Pos - v2.m_Pos).normalized * .3f;
        m_Center = (v0.m_Pos + v1.m_Pos + v2.m_Pos) / 3f;
    }

    public void Update()
    {
        m_Normal = Vector3.Cross(m_V0.m_Pos - m_V1.m_Pos, m_V0.m_Pos - m_V2.m_Pos).normalized * .3f;
        m_Center = (m_V0.m_Pos + m_V1.m_Pos + m_V2.m_Pos) / 3f;
    }

    public void Draw()
    {
        Debug.DrawLine(m_V0.m_Pos, m_V1.m_Pos);
        Debug.DrawLine(m_V1.m_Pos, m_V2.m_Pos);
        Debug.DrawLine(m_V2.m_Pos, m_V0.m_Pos);

        //Debug.DrawLine(m_Center, m_Center + m_Normal);
    }
}

public class TriFanMesh : MonoBehaviour 
{
	MeshFilter m_MeshFilter;
	MeshRenderer m_MeshRenderer;
    	
	public bool Draw
	{
		set
		{
			m_MeshRenderer.enabled = value;
		}
	}

	public Material m_Mat;
	Mesh m_Mesh;

	public float m_Thickness = .05f;

    Vector3     m_CenterVert;
	Vector3[]	m_BaseVerts = new Vector3[3];

	public Vector3[] BaseVerts {	get {	return m_BaseVerts;	}	}


	Vector3[] 	m_MeshVertArray;
	int[] 		m_TriIndecies;
    public Vector3[]   m_Verts;

    Vector3 m_Normal; 
	
	public Vector3 Normal {		get {	return m_Normal;	}	}
	public Vector3 TransformedNormal 
	{		
		get
		{	
			Vector3 side1 = transform.TransformPoint( m_BaseVerts[1] ) - transform.TransformPoint( m_BaseVerts[0] );
			Vector3 side2 = transform.TransformPoint( m_BaseVerts[2] ) - transform.TransformPoint( m_BaseVerts[0] );
			return Vector3.Cross(side1, side2).normalized;
		}
	}

	public Vector3 Edge01MidPoint{	get	{	return ( transform.TransformPoint( BaseVerts[0] ) + transform.TransformPoint( BaseVerts[1] ) ) / 2f;}	}
	public Vector3 Edge12MidPoint{	get	{	return ( transform.TransformPoint( BaseVerts[1] ) + transform.TransformPoint( BaseVerts[2] ) ) / 2f;}	}
	public Vector3 Edge20MidPoint{	get	{	return ( transform.TransformPoint( BaseVerts[2] ) + transform.TransformPoint( BaseVerts[0] ) ) / 2f;}	}


    // debug
    public Transform[] m_Transforms;
    public List<Tri> m_TriObjects = new List<Tri>();
    public List<Vert> m_VertObjects = new List<Vert>();

    private void Start()
    {
        // Create verts
        m_VertObjects.Add(new Vert(transform, m_VertObjects.Count));

        for (int i = 0; i < m_Transforms.Length; i++)        
            m_VertObjects.Add(new Vert(m_Transforms[i], m_VertObjects.Count));        

        for (int i = 1; i < m_VertObjects.Count; i++)
        {
            int nextIndex = i + 1;
            nextIndex %= m_VertObjects.Count;
            if(nextIndex==0)
                nextIndex = 1;

            m_TriObjects.Add(new Tri(m_VertObjects[0], m_VertObjects[i], m_VertObjects[nextIndex]));
        }
    }

    private void Update()
    {        
        // Update Tris
        for (int i = 0; i < m_TriObjects.Count; i++)
        {            
            int nextIndex = i + 1;
            nextIndex %= m_Transforms.Length;
            m_TriObjects[i].Update();
        }

        // Draw tris
        Vector3 baseNormal = Vector3.zero;
        for (int i = 0; i < m_TriObjects.Count; i++)
        {
            baseNormal += m_TriObjects[i].m_Normal;
            m_TriObjects[i].Draw();
        }
        
        for (int i = 0; i < m_VertObjects.Count; i++)
        {
            Debug.DrawLine(m_VertObjects[i].m_Pos, m_VertObjects[i].GetExtrudedVert(.1f));
        }

        if (Input.GetKeyDown(KeyCode.M))
            CreateFan();
    }

  
    void CreateFan()
    {
        // create mesh and filter
        m_Mesh = new Mesh();
        m_MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = m_Mat;
        
        #region Verts
        m_Verts = new Vector3[m_VertObjects.Count * 2];
        // Assign vertex positions
        for (int i = 0; i < m_VertObjects.Count; i++)
        {
            m_Verts[i] = transform.InverseTransformPoint(m_VertObjects[i].m_Pos);
            m_Verts[i + m_VertObjects.Count] = transform.InverseTransformPoint(m_VertObjects[i].GetExtrudedVert(.1f));
        }
        #endregion

        #region Tris        
        int triCount = m_TriObjects.Count*4;
        m_TriIndecies = new int[triCount * 3];

        // top tris
        int index = 0;
        for (int i = 0; i < m_TriObjects.Count; i++)
        {
            m_TriIndecies[index] = m_TriObjects[i].m_V2.m_Index;
            index++;
            m_TriIndecies[index] = m_TriObjects[i].m_V1.m_Index;
            index++;
            m_TriIndecies[index] = m_TriObjects[i].m_V0.m_Index;
            index++;
        }

        // bottom tris
        for (int i = 0; i < m_TriObjects.Count; i++)
        {
            m_TriIndecies[index] = m_TriObjects[i].m_V0.m_Index + m_TriObjects.Count + 1;
            index++;
            m_TriIndecies[index] = m_TriObjects[i].m_V1.m_Index + m_TriObjects.Count + 1;
            index++;
            m_TriIndecies[index] = m_TriObjects[i].m_V2.m_Index + m_TriObjects.Count + 1;
            index++;
        }

        // Side tris
        for (int i = 0; i < m_TriObjects.Count; i++)
        {
            m_TriIndecies[index] = m_TriObjects[i].m_V1.m_Index;
            index++;
            m_TriIndecies[index] = m_TriObjects[i].m_V2.m_Index;
            index++;
            m_TriIndecies[index] = m_TriObjects[i].m_V2.m_Index + m_TriObjects.Count + 1;
            index++;

            m_TriIndecies[index] = m_TriObjects[i].m_V2.m_Index + m_TriObjects.Count + 1;
            index++;
            m_TriIndecies[index] = m_TriObjects[i].m_V1.m_Index + m_TriObjects.Count + 1;
            index++;
            m_TriIndecies[index] = m_TriObjects[i].m_V1.m_Index;            
            index++;
        }
        #endregion

        // Create mesh
        m_MeshFilter.mesh = new Mesh();
        m_MeshFilter.mesh.vertices = m_Verts;
        m_MeshFilter.mesh.SetTriangles(m_TriIndecies, 0);
        m_MeshFilter.mesh.RecalculateBounds();
        m_MeshFilter.mesh.RecalculateNormals();
    }

    /*
	public void CreateTri ( Vector3 vert0, Vector3 vert1, Vector3 vert2, Extrusion extrusion ) 
	{
		m_BaseVerts [0] = vert0;
		m_BaseVerts [1] = vert1;
		m_BaseVerts [2] = vert2;

		m_Extrusion = extrusion;


		m_MeshFilter = gameObject.AddComponent< MeshFilter > ();
		m_MeshRenderer = gameObject.AddComponent< MeshRenderer > ();
		m_MeshRenderer.material = m_Mat;

		Vector3 side1 = vert1 - vert0;
		Vector3 side2 = vert2 - vert0;
		m_Normal = Vector3.Cross(side1, side2).normalized;

		if (m_Extrusion == Extrusion.None) 
		{
			m_MeshVertArray = new Vector3[3];
			
			UpdateThickness ();
			
			// 8 tris * 3 verts each
			m_Tris = new int[ 3 ];
			
			// Face Down
			m_Tris [0] = 2;
			m_Tris [1] = 1;
			m_Tris [2] = 0;
		}
		else
		{
			m_MeshVertArray = new Vector3[6];

			UpdateThickness ();

			// 8 tris * 3 verts each
			m_Tris = new int[ 24 ];

			// Face Down
			m_Tris [0] = 2;
			m_Tris [1] = 1;
			m_Tris [2] = 0;

			// Face Up
			m_Tris [3] = 3;
			m_Tris [4] = 4;
			m_Tris [5] = 5;

			//Face 0
			m_Tris [6] = 0;
			m_Tris [7] = 1;
			m_Tris [8] = 3;

			m_Tris [9] = 1;
			m_Tris [10] = 4;
			m_Tris [11] = 3;

			//Face 1
			m_Tris [12] = 4;
			m_Tris [13] = 1;
			m_Tris [14] = 2;
		
			m_Tris [15] = 2;
			m_Tris [16] = 5;
			m_Tris [17] = 4;

			//Face 2
			m_Tris [18] = 5;
			m_Tris [19] = 2;
			m_Tris [20] = 3;
		
			m_Tris [21] = 3;
			m_Tris [22] = 2;
			m_Tris [23] = 0;
		}


		// Create mesh
		m_MeshFilter.mesh = new Mesh ();
		m_MeshFilter.mesh.vertices = m_MeshVertArray;
		m_MeshFilter.mesh.SetTriangles (m_Tris, 0);
		m_MeshFilter.mesh.RecalculateBounds ();
		m_MeshFilter.mesh.RecalculateNormals();
		;
	}

	public void UpdateTri( Vector3 vert0, Vector3 vert1, Vector3 vert2, float thickness )
	{
		m_BaseVerts [0] = vert0;
		m_BaseVerts [1] = vert1;
		m_BaseVerts [2] = vert2;

		Vector3 side1 = vert1 - vert0;
		Vector3 side2 = vert2 - vert0;
		m_Normal = Vector3.Cross(side1, side2).normalized;

		m_Thickness = thickness;
				
		UpdateThickness ();

		m_MeshFilter.mesh.vertices = m_MeshVertArray;
	}

	void UpdateThickness()
    { 		
		m_MeshVertArray [0] = m_BaseVerts [0];
		m_MeshVertArray [1] = m_BaseVerts [1];
		m_MeshVertArray [2] = m_BaseVerts [2];
			
		m_MeshVertArray [3] = m_BaseVerts [0] + (m_Normal * m_Thickness);
		m_MeshVertArray [4] = m_BaseVerts [1] + (m_Normal * m_Thickness);
		m_MeshVertArray [5] = m_BaseVerts [2] + (m_Normal * m_Thickness);		
	}


	void OnDrawGizmos()
	{
		if (!Application.isPlaying)
			return;

		for (int i = 0; i < m_MeshVertArray.Length; i++)
		{
			Gizmos.DrawSphere( m_MeshVertArray[i], .001f );
		}
	}
    */
}
