using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Edge
{
    public Vector3 m_p1, m_p2;
    public float m_Length;
    public Vector3 m_CenterPos;

    public Edge(Vector3 p1, Vector3 p2)
    {
        m_p1 = p1;
        m_p2 = p2;
        m_CenterPos = (m_p1 + m_p2)/ 2f;

        m_Length = (m_p1 - m_p2).magnitude;
    }
}

public class MeshMeasurer : MonoBehaviour
{
    List<Edge> m_Edges = new List<Edge>();
    public TextMesh m_TextPrefab;

    void Start()
    {
        ConvertMesh(GetComponent<MeshFilter>());
    }

    public void ConvertMesh(MeshFilter meshfilter)
    {
        Mesh mesh = meshfilter.mesh;

        // Create array of edges
        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            m_Edges.Add(new Edge(mesh.vertices[mesh.triangles[(i * 3)]], mesh.vertices[mesh.triangles[(i * 3) + 1]]));
            m_Edges.Add(new Edge(mesh.vertices[mesh.triangles[(i * 3) + 1]], mesh.vertices[mesh.triangles[(i * 3) + 2]]));
            m_Edges.Add(new Edge(mesh.vertices[mesh.triangles[(i * 3) + 2]], mesh.vertices[mesh.triangles[(i * 3)]]));
        }

       
        // Check if any edges are the same and remove copies
        // If edges share a vertex add them to the appropriate connection list
        List<Edge> edgesToRemove = new List<Edge>();
        for (int i = 0; i < m_Edges.Count; i++)
        {
            Edge baseEdge = m_Edges[i];

            if (edgesToRemove.Contains(baseEdge))
                continue;

            for (int j = 0; j < m_Edges.Count; j++)
            {
                if (j == i)
                    continue;

                Edge compareEdge = m_Edges[j];

                if (edgesToRemove.Contains(compareEdge))
                    continue;

                // If not same length then continue
                if (baseEdge.m_Length != compareEdge.m_Length)
                    continue;
                else if (baseEdge.m_p1 == compareEdge.m_p1 && baseEdge.m_p2 == compareEdge.m_p2)
                {
                    edgesToRemove.Add(baseEdge);
                }
                else if (baseEdge.m_p1 == compareEdge.m_p2 && baseEdge.m_p2 == compareEdge.m_p1)
                {
                    edgesToRemove.Add(baseEdge);
                }
            }
        }

        // Remove duplicate edges
        foreach (Edge e in edgesToRemove)
        {
            if (e != null)
            {
                m_Edges.Remove(e);
            }
        }

        foreach (Edge e in m_Edges)
        {
            TextMesh text = Instantiate(m_TextPrefab);
            text.transform.position = e.m_CenterPos + transform.position;
            text.text = (e.m_Length * 1000).ToString("#");
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < m_Edges.Count; i++)
            {
                Vector3 pos = (m_Edges[i].m_p1 + m_Edges[i].m_p2) / 2f;
                pos += transform.position;
                //Gizmos.DrawWireSphere(pos, .3f);                    
            }
        }
    }
}