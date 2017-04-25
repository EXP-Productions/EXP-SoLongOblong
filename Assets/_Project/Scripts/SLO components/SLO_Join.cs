using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SoLongOblong
{
    public class SLO_Join : MonoBehaviour
    {
        SLOObject m_Object;

        public enum JointType
        {
            Inner,
            Outer,
            TabOnly,
        }

        int m_Index;
        public int Index { get { return m_Index; } }

        MeshRenderer m_SphereMesh;

        #region SLO Components
        // Edges attached to the join
        List<SLO_Edge> m_ConnectedEdges = new List<SLO_Edge>();
        public List<SLO_Edge> ConnectedEdges { get { return m_ConnectedEdges; } }

        // Arms attached to the join
        public List<SLO_Join_Arm> m_Arms = new List<SLO_Join_Arm>();
        #endregion

        #region Measurements
        float m_Diameter;
        public float Diameter
        {
            get { return m_Diameter; }
            set
            {
                m_Diameter = value;
                m_SphereMesh.transform.localScale = Vector3.one * (m_Diameter / 1000f);
            }
        }

        public float ArmInnerDiameter
        {
            get { return m_Arms[0].InsideDiameter; }
        }

        public float ArmOuterDiameter
        {
            get { return m_Arms[0].OutsideDiameter; }
        }

        float m_Length = .030f;
        public float Length
        {
            set
            {
                m_Length = value;
                for (int i = 0; i < m_Arms.Count; i++)
                {
                    m_Arms[i].TotalLength = m_Length;
                }
            }
            get
            {
                return m_Arms[0].TotalLength;
            }
        }

        float m_MaxLengthPercentage = .4f;
        float m_Tolerence = 0;

        // TODO: figure out min shapeways wall thickness
        public float m_WallThickness = 2;
        #endregion
        
        // Stores list of all normals that make up this unique vert
        public List<Vector3> m_Normals;

        // hacks, only update on adding
        public Vector3 Normal
        {
            get
            {
                Vector3 normal = Vector3.zero;
                foreach (Vector3 v in m_Normals)
                {
                    normal += v;
                }

                normal /= m_Normals.Count;
                return normal;
            }
        }

        public void Init(SLOObject sloObj, int index, Vector3 pos, float joinerRadius, Material mat)
        {
            m_Object = sloObj;

            m_Index = index;
            m_Diameter = joinerRadius;

            transform.position = pos;

            // TODO: save ref to spheref or scaling
            m_SphereMesh = GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<MeshRenderer>();
            m_SphereMesh.gameObject.SetParentAndZero(transform);
            m_SphereMesh.transform.localScale = Vector3.one * (m_Diameter / 1000f);
            m_SphereMesh.GetComponent<MeshRenderer>().material = mat;

            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = m_Diameter * .001f;

            gameObject.tag = "Selectable";
        }

        public void UpdateJoin(JointType type, float edgeDiameter, float wallthickness, float length)
        {
            if (type == JointType.Inner)
            {
                Diameter = edgeDiameter;
                m_SphereMesh.enabled = true;

                for (int i = 0; i < m_Arms.Count; i++)
                {
                    m_Arms[i].Mesh.m_Renderer.enabled = true;
                    m_Arms[i].OutsideDiameter = m_Diameter - m_Tolerence;
                    m_Arms[i].InsideDiameter = 0;
                }
            }
            else if (type == JointType.Outer)
            {
                Diameter = edgeDiameter + (wallthickness * 2);
                m_SphereMesh.enabled = true;

                for (int i = 0; i < m_Arms.Count; i++)
                {
                    m_Arms[i].Mesh.m_Renderer.enabled = true;
                    m_Arms[i].OutsideDiameter = Diameter;
                    m_Arms[i].InsideDiameter = edgeDiameter + m_Tolerence;
                }
            }
            else if (type == JointType.TabOnly)
            {
                // turn off sphere mesh
                m_SphereMesh.enabled = false;

                for (int i = 0; i < m_Arms.Count; i++)
                {
                    // Turn arms off
                    m_Arms[i].Mesh.m_Renderer.enabled = false;                    
                }
            }

            Length = length;

            for (int i = 0; i < m_Arms.Count; i++)
            {
                m_Arms[i].UpdateArm();
            }
        }

        void OnMouseDown()
        {
           // MeshEdgeSolver.Instance.SetSelectedObject(gameObject);
        }

        public void AddEdge(SLO_Edge edge, float maxJoinLength)
        {
            m_ConnectedEdges.Add(edge);

            SLO_Join lookAtJoin = edge.m_Join0;
            if (lookAtJoin.transform.position == transform.position)
                lookAtJoin = edge.m_Join1;

            // Create new arm
            SLO_Join_Arm newArm = new GameObject("Join Arm").AddComponent<SLO_Join_Arm>();
            newArm.name = "Join Arm";
            newArm.transform.SetParent(transform);
            newArm.Init(edge, this, lookAtJoin, maxJoinLength, m_MaxLengthPercentage);
            m_Arms.Add(newArm);
        }

        public void RemoveEdge(SLO_Edge edge)
        {
            // remove edge from list
            m_ConnectedEdges.Remove(edge);

            if (m_Arms.Contains(edge.m_JoinArm0))
            {
                m_Arms.Remove(edge.m_JoinArm0);
                Destroy(edge.m_JoinArm0.gameObject);
            }
            else if (m_Arms.Contains(edge.m_JoinArm1))
            {
                m_Arms.Remove(edge.m_JoinArm1);
                Destroy(edge.m_JoinArm1.gameObject);
            }

            print("Removing edge from " + name);
        }

        void Show(bool show)
        {
            MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>() as MeshRenderer[];
            foreach (MeshRenderer rend in renderers)
                rend.enabled = show;
        }
    }
}
