using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    public class SLOObjectFromMesh : MonoBehaviour
    {        
        public SLOObject ConvertMesh(MeshFilter meshfilter, bool addOuterTabs, bool addInnerTabs )
        {
            // Create new object
            SLOObject sloObj = new GameObject("SLO Obj - " ).AddComponent<SLOObject>();
            sloObj.transform.position = Vector3.zero;
            
            float m_EdgeDiameter = PlayerPrefs.GetFloat("m_EdgeDiameter", 9.5f);
            float m_JoinerLength = PlayerPrefs.GetFloat("m_JoinerLength", 20);
            float WallThickness = PlayerPrefs.GetFloat("WallThickness", 2);

            Mesh mesh = meshfilter.mesh;

            // Create array of edges
            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                // Create edge for 0 - 1
                sloObj.CreateEdge(mesh.vertices[mesh.triangles[(i * 3)]], mesh.vertices[mesh.triangles[(i * 3) + 1]]);
                // Create edge for 1 - 2
                sloObj.CreateEdge(mesh.vertices[mesh.triangles[(i * 3) + 1]], mesh.vertices[mesh.triangles[(i * 3) + 2]]);
                // Create edge for 2 - 0
                sloObj.CreateEdge(mesh.vertices[mesh.triangles[(i * 3) + 2]], mesh.vertices[mesh.triangles[(i * 3)]]);                
            }

            #region Find and remove duplicate edges
            // Check if any edges are the same and remove copies
            // If edges share a vertex add them to the appropriate connection list
            List<SLO_Edge> edgesToRemove = new List<SLO_Edge>();
            for (int i = 0; i < sloObj.Edges.Count; i++)
            {
                SLO_Edge baseEdge = sloObj.Edges[i];

                if (edgesToRemove.Contains(baseEdge))
                    continue;

                for (int j = 0; j < sloObj.Edges.Count; j++)
                {
                    if (j == i)
                        continue;

                    SLO_Edge compareEdge = sloObj.Edges[j];

                    if (edgesToRemove.Contains(compareEdge))
                        continue;

                    // If not same length then continue
                    if (baseEdge.Length != compareEdge.Length)
                        continue;
                    else if (baseEdge.m_VertexPos0 == compareEdge.m_VertexPos0 && baseEdge.m_VertexPos1 == compareEdge.m_VertexPos1)
                    {
                        edgesToRemove.Add(baseEdge);
                    }
                    else if (baseEdge.m_VertexPos0 == compareEdge.m_VertexPos1 && baseEdge.m_VertexPos1 == compareEdge.m_VertexPos0)
                    {
                        edgesToRemove.Add(baseEdge);
                    }
                }
            }
            
            // Remove duplicate edges
            foreach (SLO_Edge e in edgesToRemove)
            {
                if (e != null)
                {
                    sloObj.Edges.Remove(e);
                    DestroyImmediate(e.gameObject);
                }
            }
            #endregion
            
            // Index the remaining edges
            for (int i = 0; i < sloObj.Edges.Count; i++)
            {
                sloObj.Edges[i].m_Index = i;
                sloObj.Edges[i].name = "Edge " + i;
            }
            
            // Find all unique verts
            List<Vector3> uniqueVertPositions = new List<Vector3>();
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                if (!uniqueVertPositions.Contains(mesh.vertices[i]))
                    uniqueVertPositions.Add(mesh.vertices[i]);
            }
            
            // Set to the transform of the mesh
            sloObj.transform.position = transform.position;
            
            // Create joiners for each unique vert
            for (int i = 0; i < uniqueVertPositions.Count; i++)
            {
                sloObj.CreateJoiner(meshfilter.transform.TransformPoint(uniqueVertPositions[i]));
            }
            
            // Assign joiners to edges
            for (int i = 0; i < sloObj.Edges.Count; i++)
            {
                for (int j = 0; j < sloObj.Joiners.Count; j++)
                {
                    if (meshfilter.transform.TransformPoint(sloObj.Edges[i].m_VertexPos0) == sloObj.Joiners[j].transform.position)
                    {
                        sloObj.Edges[i].m_Join0 = sloObj.Joiners[j];
                    }

                    if (meshfilter.transform.TransformPoint(sloObj.Edges[i].m_VertexPos1) == sloObj.Joiners[j].transform.position)
                    {
                        sloObj.Edges[i].m_Join1 = sloObj.Joiners[j];
                    }
                }
            }

            // Assign joiners to edges
            for (int i = 0; i < sloObj.Edges.Count; i++)
            {
                sloObj.Edges[i].m_Join0.AddEdge(sloObj.Edges[i], m_JoinerLength);
                sloObj.Edges[i].m_Join1.AddEdge(sloObj.Edges[i], m_JoinerLength);
            }

            // Initalize edges
            for (int i = 0; i < sloObj.Edges.Count; i++)
            {
                sloObj.Edges[i].transform.position = sloObj.Edges[i].m_Join0.transform.position;
                sloObj.Edges[i].transform.LookAt(sloObj.Edges[i].m_Join1.transform.position);
                sloObj.Edges[i].transform.SetParent(sloObj.transform);
                sloObj.Edges[i].AddEdgeMesh();
               // sloObj.Edges[i].SetMaterial(SLOManager.Instance.m_MatEdge);
            }

            // Create faces
            for (int i = 0; i < mesh.triangles.Length / 3f; i++)
            {
                int triIndex = (i * 3);

                Vector3 v0 = meshfilter.transform.TransformPoint(mesh.vertices[mesh.triangles[triIndex]]);
                Vector3 v1 = meshfilter.transform.TransformPoint(mesh.vertices[mesh.triangles[triIndex + 1]]);
                Vector3 v2 = meshfilter.transform.TransformPoint(mesh.vertices[mesh.triangles[triIndex + 2]]);

                SLO_Join j0 = sloObj.FindJoinFromPos(v0);
                SLO_Join j1 = sloObj.FindJoinFromPos(v1);
                SLO_Join j2 = sloObj.FindJoinFromPos(v2);

                sloObj.CreateFace(j0, j1, j2);
            }

            for (int i = 0; i < sloObj.Faces.Count; i++)
            {
                int triIndex = (i * 3);
                
                SLO_Join j0 = sloObj.Faces[i].Join0; // m_MeshParent.TransformPoint(m_Mesh.vertices[m_Mesh.triangles[triIndex]]);
                SLO_Join j1 = sloObj.Faces[i].Join1; //m_MeshParent.TransformPoint(m_Mesh.vertices[m_Mesh.triangles[triIndex + 1]]);
                SLO_Join j2 = sloObj.Faces[i].Join2; //m_MeshParent.TransformPoint(m_Mesh.vertices[m_Mesh.triangles[triIndex + 2]]);

                // Tri 1
                sloObj.CreateTab(j0, j1, j2);
                sloObj.CreateTab(j1, j2, j0);
                sloObj.CreateTab(j2, j0, j1);
            }

            sloObj.RecalculateAllStats();

            // Deactivate original meshfilter
            meshfilter.transform.gameObject.SetActive(false);
                          
            return sloObj;
        }
    }
}
