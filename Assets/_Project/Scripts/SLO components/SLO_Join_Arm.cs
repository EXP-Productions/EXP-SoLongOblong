using UnityEngine;
using System.Collections;

namespace SoLongOblong
{
    public class SLO_Join_Arm : MonoBehaviour
    {
        SLO_Join m_AttachedJoin;

        SLO_Join m_LookAtJoin;
        public SLO_Join LookAtJoin
        {
            get
            {
                return m_LookAtJoin;
            }
        }

        Vector3 LookAt
        {
            get
            {
                return m_LookAtJoin.transform.position;
            }
        }

        public float m_SleeveLength;
        float m_JoinerMaxPercentageOfEdge;



        // joint radius = edge D + Joint wall thickness 

        ProceduralMesh_Tube m_TubeMesh;

        public float TotalLength
        {
            get
            {
                //return Mathf.Min ( m_MaxLength / 1000f, ( transform.position - m_LookAtJoin.transform.position ).magnitude * m_JoinerMaxPercentageOfEdge );
                return m_IntersectionLength + m_SleeveLength;
            }
            set
            {
                m_SleeveLength = value;
            }
        }

        float m_OutsideDiameter;
        public float OutsideDiameter
        {
            get { return m_OutsideDiameter; }
            set
            {
                m_OutsideDiameter = value;
                m_TubeMesh.OutsideRadius = (m_OutsideDiameter / 2f) * .001f;
            }
        }

        float m_InsideDiameter;
        public float InsideDiameter
        {
            get { return m_InsideDiameter; }
            set
            {
                m_InsideDiameter = value;
                m_TubeMesh.InsideRadius = (m_InsideDiameter / 2f) * .001f;
            }
        }

        SLO_Edge m_Edge;
        public float m_ClosestEdgeAngle = 180;
        public float m_IntersectionLength;

        public void Init(SLO_Edge edge, SLO_Join attachedJoin, SLO_Join lookAtJoin, float maxLength, float joinerMaxPercentageOfEdge)
        {
            m_Edge = edge;
            m_AttachedJoin = attachedJoin;
            m_LookAtJoin = lookAtJoin;
            m_SleeveLength = maxLength;
            m_JoinerMaxPercentageOfEdge = joinerMaxPercentageOfEdge;

            m_TubeMesh = new GameObject("Tube Mesh").AddComponent<ProceduralMesh_Tube>();
            m_TubeMesh.transform.SetParent(transform);
            m_TubeMesh.Init();
            m_TubeMesh.transform.localScale = Vector3.one;
            m_TubeMesh.transform.localRotation = Quaternion.Euler(90, 0, 0);

            m_TubeMesh.m_Renderer.material = SLOResourceManager.Instance.m_MatJoiner;

            if (edge.m_Join0 == m_AttachedJoin) edge.m_JoinArm0 = this;
            else edge.m_JoinArm1 = this;

            transform.LookAt(LookAt);

            UpdateArm();
        }

        public void UpdateArm()
        {
            // Hacks. Only update wehn moved
            float AdjustedLength;
            Vector3 thisEdgeVector;
            thisEdgeVector = m_LookAtJoin.transform.position - m_AttachedJoin.transform.position;
            m_ClosestEdgeAngle = 360;

            // Find the closest angle out of the connected edges
            foreach (SLO_Edge edge in m_AttachedJoin.ConnectedEdges)
            {
                if (edge == m_Edge) continue;

                float angle;
                Vector3 otherEdgeVector;
                if (m_AttachedJoin == edge.m_Join0) otherEdgeVector = edge.m_Join1.transform.position - edge.m_Join0.transform.position;
                else otherEdgeVector = edge.m_Join0.transform.position - edge.m_Join1.transform.position;

                angle = Vector3.Angle(thisEdgeVector, otherEdgeVector);
                if (angle < m_ClosestEdgeAngle)
                    m_ClosestEdgeAngle = angle;
            }

            m_IntersectionLength = (90 / m_ClosestEdgeAngle) * ((m_AttachedJoin.Diameter / 2f) * 1.2f);
            //AdjustedLength = Mathf.Clamp (AdjustedLength, m_AttachedJoin.Diameter, 500);

            transform.position = m_AttachedJoin.transform.position;
            transform.LookAt(LookAt);
            m_TubeMesh.transform.localScale = new Vector3(1, TotalLength * .001f, 1);
            //transform.localScale = new Vector3 ( 1, Length , 1 );
        }
    }
}