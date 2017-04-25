using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SoLongOblong
{
    public class SLO_Edge : MonoBehaviour
    {
        SLOObject m_Object;

        public int m_Index;
        ProceduralMesh_Tube m_TubeMesh;

        public Vector3 m_VertexPos0;
        public Vector3 m_VertexPos1;

        public SLO_Join m_Join0;
        public SLO_Join m_Join1;
        public SLO_Join_Arm m_JoinArm0;
        public SLO_Join_Arm m_JoinArm1;


        float m_JointTolerance;
        float JointTolerance
        {
            set
            {
                m_JointTolerance = value;
                Length = m_Length;
            }
        }

        float m_Diameter = 7;
        public float Diameter
        {
            set
            {
                m_Diameter = value;

                if (m_Object.m_JointType == SLO_Join.JointType.Outer)
                {
                    m_TubeMesh.m_Renderer.enabled = true;
                    m_TubeMesh.OutsideRadius = (m_Diameter / 2f) * .001f;
                }
                else if (m_Object.m_JointType == SLO_Join.JointType.Inner)
                {
                    m_TubeMesh.m_Renderer.enabled = true;
                    m_TubeMesh.OutsideRadius = ((m_Diameter / 2f) + (m_Object.WallThickness * 2)) * .001f;
                    m_TubeMesh.InsideRadius = (m_Diameter / 2f) * .001f;
                }
                else if (m_Object.m_JointType == SLO_Join.JointType.TabOnly)
                {
                    m_TubeMesh.m_Renderer.enabled = false;
                }
            }
        }

        float m_Length;
        public float Length
        {
            set
            {
                m_Length = value;
                //transform.localScale = new Vector3 (1,  1, ( m_Length) - ( (m_JoinArm0.m_IntersectionLength + m_JoinArm1.m_IntersectionLength) * .001f ) );
            }
            get { return m_Length; }

        }

        private List<SLO_Edge> m_EdgesConnectedToVert0 = new List<SLO_Edge>();
        public List<SLO_Edge> EdgesConnectedToVert0
        {
            get { return m_EdgesConnectedToVert0; }
            set { m_EdgesConnectedToVert0 = value; }
        }

        private List<SLO_Edge> m_EdgesConnectedToVert1 = new List<SLO_Edge>();
        public List<SLO_Edge> EdgesConnectedToVert1
        {
            get { return m_EdgesConnectedToVert1; }
            set { m_EdgesConnectedToVert1 = value; }
        }

        public SLO_Face m_AdjascentFace0;
        public SLO_Face m_AdjascentFace1;

        Vector3 m_EdgeVector;

        public void Init(SLOObject sloObj, Vector3 vertexPos0, Vector3 vertexPos1)
        {
            m_Object = sloObj;

            m_VertexPos0 = vertexPos0;
            m_VertexPos1 = vertexPos1;

            m_Length = (vertexPos0 - vertexPos1).magnitude;

            gameObject.tag = "Selectable";
        }

        public void AddEdgeMesh()
        {
            // TODO: figure out joint tollerence based on anglesof joins nd intersections
            //m_JointTolerance = jointTolerance;

            Length = (m_Join0.transform.position - m_Join1.transform.position).magnitude;
            
            m_TubeMesh = new GameObject("Tube Mesh").AddComponent<ProceduralMesh_Tube>();
            m_TubeMesh.Init();
            m_TubeMesh.transform.SetParent(transform);
            m_TubeMesh.name = "Edge Tube";
            m_TubeMesh.transform.position = m_Join0.transform.position;
            m_TubeMesh.transform.localRotation = Quaternion.Euler(90, 0, 0);
            m_TubeMesh.transform.localScale = Vector3.one;

            SetMaterial(SLOResourceManager.Instance.m_MatEdge);
        }

        void Update()
        {
            m_EdgeVector = m_Join0.transform.position - m_Join1.transform.position;
            transform.position = m_Join0.transform.position - (m_EdgeVector.normalized * m_JoinArm0.m_IntersectionLength * .001f);
            Length = (m_Join0.transform.position - m_Join1.transform.position).magnitude - ((m_JoinArm0.m_IntersectionLength + m_JoinArm1.m_IntersectionLength) * .001f);
            transform.localScale = new Vector3(1, 1, Length);
            transform.LookAt(m_Join1.transform.position);
        }

        public void SetMaterial(Material mat)
        {
            if (m_TubeMesh != null)
                m_TubeMesh.m_Renderer.material = mat;
        }

        public void Delete()
        {
            m_Join0.RemoveEdge(this);
            m_Join1.RemoveEdge(this);

            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            //Vector3 vec01 = ( m_Join0.transform.position - m_Join1.transform.position ).normalized;

            //Gizmos.DrawLine (m_Join0.transform.position - ( vec01 * m_Joint0Offset ) , m_Join1.transform.position );
        }
    }
}