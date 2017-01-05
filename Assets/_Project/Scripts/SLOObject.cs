using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    public class SLOObject : MonoBehaviour
    {
        // object elements. Edges, joiners and faces
        List<SLO_Edge> m_Edges = new List<SLO_Edge>();
        List<SLO_Join> m_Joiners = new List<SLO_Join>();
        List<SLO_Face> m_Faces = new List<SLO_Face>();

        public int JointCount { get { return m_Joiners.Count; } }
        public int EdgeCount { get { return m_Joiners.Count; } }
        public int FaceCount { get { return m_Joiners.Count; } }

        // The type of joint being used. TODO: Change to edge type instead
        public SLO_Join.JointType m_JointType = SLO_Join.JointType.Inner;

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
        public Material m_MatJoiner;
        public Material m_MatEdge;
        public Material m_MatFace;
        public Material m_FlatFaceMat;
        #endregion

        
    }
}
