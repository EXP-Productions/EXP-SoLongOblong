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
        
        Debug.DrawLine(m_V0.m_Pos, m_V1.m_Pos, Color.yellow);
        Debug.DrawLine(m_V1.m_Pos, m_V2.m_Pos, Color.yellow);
        Debug.DrawLine(m_V2.m_Pos, m_V0.m_Pos, Color.yellow);

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

    Vector3 m_CenterVert;
    Vector3[] m_BaseVerts = new Vector3[3];

    public Vector3[] BaseVerts { get { return m_BaseVerts; } }


    Vector3[] m_MeshVertArray;
    int[] m_TriIndecies;
    public Vector3[] m_Verts;

    Vector3 m_Normal;
    public Vector3 Normal { get { return m_Normal; } }
   

    // debug
    public Transform[] m_Transforms;
    public List<Tri> m_TriObjects = new List<Tri>();
    public List<Vert> m_VertObjects = new List<Vert>();

    bool m_Created = false;

    private void Start()
    {
        if (m_Transforms != null && !m_Created)
            CreateVerts();
    }

    public void Init(Transform[] transforms)
    {
        m_Transforms = transforms;
        CreateVerts();
       // CreateFan();
    }

    void CreateVerts()
    {
        print("creating verts");
        // Create verts
        m_VertObjects.Add(new Vert(transform, m_VertObjects.Count));
        for (int i = 0; i < m_Transforms.Length; i++)
            m_VertObjects.Add(new Vert(m_Transforms[i], m_VertObjects.Count));

        // create tri objects
        for (int i = 1; i < m_VertObjects.Count; i++)
        {
            int nextIndex = i + 1;
            nextIndex %= m_VertObjects.Count;
            if (nextIndex == 0)
                nextIndex = 1;

            m_TriObjects.Add(new Tri(m_VertObjects[0], m_VertObjects[i], m_VertObjects[nextIndex]));
        }
    }

    private void Update()
    {
        /*
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
         */
        for (int i = 0; i < m_VertObjects.Count; i++)
        {
            Debug.DrawLine(m_VertObjects[i].m_Pos, m_VertObjects[i].GetExtrudedVert(.1f), Color.red);
        }
       

       // if (Input.GetKeyDown(KeyCode.M))
       //     CreateFan();

        //if (m_Created)
        //    UpdateFan();
    }


    void CreateFan()
    {
        m_Created = true;

        // create mesh and filter
        m_Mesh = new Mesh();
        m_MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        if(m_Mat != null)renderer.material = m_Mat;
        
        SetVertPositions();        

        #region Tris        
        int triCount = m_TriObjects.Count * 4;
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

    void SetVertPositions()
    {
        m_Verts = new Vector3[m_VertObjects.Count * 2];
        for (int i = 0; i < m_VertObjects.Count; i++)
        {
            m_Verts[i] = transform.InverseTransformPoint(m_VertObjects[i].m_Pos);
            m_Verts[i + m_VertObjects.Count] = transform.InverseTransformPoint(m_VertObjects[i].GetExtrudedVert(.1f));
        }
    }

    void UpdateFan()
    {     
        // Assign vertex positions
        SetVertPositions();    

        m_MeshFilter.mesh.vertices = m_Verts;
        m_MeshFilter.mesh.RecalculateBounds();
        m_MeshFilter.mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < m_VertObjects.Count; i++)
        {
            Gizmos.DrawSphere(m_VertObjects[i].m_Pos, .01f);
        }
    }
}
