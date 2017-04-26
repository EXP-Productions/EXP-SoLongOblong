using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    /// <summary>
    /// An SLO object stores all the edges, joiners, faces and tabs for the object
    /// </summary>
    public class SLOObject : MonoBehaviour
    {
        // object elements. Edges, joiners and faces
        List<SLO_Edge> m_Edges = new List<SLO_Edge>();
        public List<SLO_Edge> Edges { get { return m_Edges; } }

        List<SLO_Join> m_Joiners = new List<SLO_Join>();
        public List<SLO_Join> Joiners { get { return m_Joiners; } }

        List<SLO_Face> m_Faces = new List<SLO_Face>();
        public List<SLO_Face> Faces { get { return m_Faces; } }

        List<SLO_Join_Tab> m_Tabs = new List<SLO_Join_Tab>();
        public List<SLO_Join_Tab> Tabs { get { return m_Tabs; } }

        List<SLO_JoinCap> m_Caps = new List<SLO_JoinCap>();
        public List<SLO_JoinCap> Caps { get { return m_Caps; } }

        // Component counts
        public int JointCount { get { return m_Joiners.Count; } }
        public int EdgeCount { get { return m_Joiners.Count; } }
        public int FaceCount { get { return m_Joiners.Count; } }
        
        #region Face variables
        // The thickness of the face parts TODO: make tab thickness be determined by the material thickness
        float m_FaceThickness = 3;
        public float FaceThickness
        {
            get { return m_FaceThickness; }
            set
            {
                m_FaceThickness = Mathf.Clamp(value, .1f, 30);
                // Update all faces when value set
                for (int i = 0; i < m_Faces.Count; i++)
                {
                    m_Faces[i].Thickness = m_FaceThickness;
                }
            }
        }
        TriMesh.Extrusion m_FaceExtrusionPlacement = TriMesh.Extrusion.Center;
        #endregion

        #region Edge variables
        public float m_EdgeDiameter = 7f;
        public float EdgeDiameter
        {
            set
            {
                m_EdgeDiameter = Mathf.Clamp(value, 3, 50);
            }
            get
            {
                return m_EdgeDiameter;
            }
        }
        #endregion

        #region Joint variables
        // The type of joint being used. TODO: Change to edge type instead
        public SLO_Join.JointType m_JointType = SLO_Join.JointType.Outer;
        public float m_JoinerLength = 40f;
        public float JoinerLength
        {
            set
            {
                m_JoinerLength = Mathf.Clamp(value, 5, 200);
            }
            get
            {
                return m_JoinerLength;
            }
        }
        
        // 1.55 mm in min shapeways feature thickness
        float m_JointWallThickness = 1.55f;
        public float WallThickness
        {
            set
            {
                m_JointWallThickness = Mathf.Clamp(value, 1.55f, 10);
            }
            get
            {
                return m_JointWallThickness;
            }
        }
        #endregion

        #region Tab variables
        bool m_OuterTabs = false;
        bool m_InnerTabs = true;       
        #endregion

        #region Object stats
        // Total length of all edges
        float m_TotalEdgeLength;
        public float TotalEdgeLength { get { return m_TotalEdgeLength; } }

        // Total area of all faces
        float m_TotalArea;
        public float TotalArea { get { return m_TotalArea; } }
        #endregion

        #region Display variables
        // Materials
        bool m_DisplayFaces = true;
        public bool DisplayFaces
        {
            set
            {
                m_DisplayFaces = value;
                for (int i = 0; i < m_Faces.Count; i++)
                    m_Faces[i].Draw = m_DisplayFaces;
            }         
        }
        #endregion
        
        void Update()
        {
            for (int i = 0; i < m_Edges.Count; i++)
            {
                m_Edges[i].Diameter = m_EdgeDiameter;
            }
            for (int i = 0; i < m_Joiners.Count; i++)
            {
                m_Joiners[i].UpdateJoin(m_JointType, m_EdgeDiameter, m_JointWallThickness, m_JoinerLength);
            }


            // Update show faces
            for (int i = 0; i < m_Faces.Count; i++)
            {
                m_Faces[i].Draw = m_DisplayFaces;
            }
        }

        public void CreateJoiner(Vector3 pos)
        {
            SLO_Join newJoin = new GameObject("Join" + Joiners.Count).AddComponent<SLO_Join>() as SLO_Join;
            newJoin.transform.SetParent(transform);
            newJoin.Init(this, m_Joiners.Count, pos, 10, SLOResourceManager.Instance.m_MatJoiner);

            // Add to list
            m_Joiners.Add(newJoin);
        }

        public void CreateCaps()
        {
            CreateCap(m_Joiners[0]);
            /*
            for (int i = 0; i < Joiners.Count; i++)
            {
                SLO_JoinCap newCap = new GameObject("Cap" + Caps.Count).AddComponent<SLO_JoinCap>();
                newCap.transform.SetParent(transform);
                newCap.Init(Joiners[i]);
                Caps.Add(newCap);
            }
            */
        }

        public void CreateCap(SLO_Join join)
        {
            print("Creating cap");
            SLO_JoinCap newCap = new GameObject("Cap" + Caps.Count).AddComponent<SLO_JoinCap>();
            newCap.transform.SetParent(transform);
            newCap.Init(join);
            Caps.Add(newCap);
        }

        // Create a new edge
        public void CreateEdge( Vector3 v0, Vector3 v1)
        {
            // Create new edge object
            SLO_Edge edge = new GameObject("Edge new" + (Edges.Count)).AddComponent<SLO_Edge>();
            edge.transform.SetParent(transform);
            edge.Init(this, v0, v1);

            // Add to list
            Edges.Add(edge);

            /*
            Edges[i].transform.position = Edges[i].m_Join0.transform.position;
            Edges[i].transform.LookAt(sloObj.Edges[i].m_Join1.transform.position);
            Edges[i].transform.SetParent(sloObj.transform);
            Edges[i].AddEdgeMesh();
            Edges[i].SetMaterial(SLOManager.Instance.m_MatEdge);
            */
        }
        
        // Create a tab by passing in 3 joins
        public void CreateTab(SLO_Join j0, SLO_Join j1, SLO_Join j2)
        {
            SLO_Join_Tab newTab = new GameObject().AddComponent<SLO_Join_Tab>() as SLO_Join_Tab;
            newTab.transform.position = j0.transform.position;
            newTab.Init(this, j0, j1, j2, SLOResourceManager.Instance.m_MatJoiner, m_OuterTabs, m_InnerTabs);
            m_Tabs.Add(newTab);
        }

        public void CreateFace( SLO_Join j0, SLO_Join j1, SLO_Join j2 )
        {
            SLO_Face newFace = new GameObject().AddComponent<SLO_Face>() as SLO_Face;
            newFace.Init(j0, j1, j2, SLOResourceManager.Instance.m_MatFace, m_FaceThickness, m_Faces.Count, m_FaceExtrusionPlacement);
            newFace.transform.SetParent(transform);

            // Add to list
            m_Faces.Add(newFace);
        }

        #region Helper methods
        // Hax - OMG SLOOOOW , fix later
        public SLO_Join FindJoinFromPos(Vector3 pos)
        {
            // parent to joiner
            for (int i = 0; i < m_Joiners.Count; i++)
            {
                if ((pos - m_Joiners[i].transform.position).magnitude < (m_JoinerLength / 1000f))
                {
                    return m_Joiners[i];
                }
            }

            return null;
        }
        #endregion

        #region Object analysis methods
        public void RecalculateAllStats()
        {
            CalculateTotalEdgeLength();
            CalculateTotalArea();
        }

        void CalculateTotalEdgeLength()
        {
            // Set total edge length to 0
            m_TotalEdgeLength = 0;

            for (int i = 0; i < m_Edges.Count; i++)
                m_TotalEdgeLength += m_Edges[i].Length;
        }

        void CalculateTotalArea()
        {
            m_TotalArea = 0;
            for (int i = 0; i < m_Faces.Count; i++)
            {
                m_TotalArea += m_Faces[i].Area;
            }
        }
        #endregion
    }
}
