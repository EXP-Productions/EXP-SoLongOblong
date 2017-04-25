using UnityEngine;
using System.Collections;

namespace SoLongOblong
{
    public class SLO_Join_Tab : MonoBehaviour
    {
        SLOObject m_Object;

        TriMesh m_TabInner;
        TriMesh m_TabOuter;

        SLO_Join m_BaseJoin;
        SLO_Join m_ConnctedJoin0;
        SLO_Join m_ConnctedJoin1;

        SLO_Join_Arm m_Arm12;
        SLO_Join_Arm m_Arm13;

        public float m_WallThickness = 2;

        Vector3[] m_InnerVerts = new Vector3[3];

        public float SpacingRadius
        {
            //get {	return Mathf.Min( MeshEdgeSolver.Instance.FaceThickness + (m_WallThickness/2f), m_BaseJoin.Diameter / 2f );	}
            get { return (m_BaseJoin.Diameter / 2f) * .2f; }
        }

        Vector3 m_Normal;

        public void ParentToJoin(bool parent)
        {
            if (parent)
            {
                transform.parent = m_BaseJoin.transform;
            }
            else
                transform.parent = null;
        }

        // Use this for initialization
        public void Init( SLOObject sloObj, SLO_Join j0, SLO_Join j1, SLO_Join j2, Material mat, bool makeInner, bool makeOuter)
        {
            #region Initialization of references
            // Sets reference to SLO object
            m_Object = sloObj;

            // Set name
            gameObject.name = "Tab";
            
            // Setup the connect joins
            m_BaseJoin = j0;
            m_ConnctedJoin0 = j1;
            m_ConnctedJoin1 = j2;

            // Find arms
            for (int i = 0; i < m_BaseJoin.m_Arms.Count; i++)
            {
                if (m_BaseJoin.m_Arms[i].LookAtJoin == m_ConnctedJoin0)
                    m_Arm12 = m_BaseJoin.m_Arms[i];

                if (m_BaseJoin.m_Arms[i].LookAtJoin == m_ConnctedJoin1)
                    m_Arm13 = m_BaseJoin.m_Arms[i];
            }
            #endregion

            #region Calculations            
            m_InnerVerts = TriMeshTest1.FindInnerVerts(m_BaseJoin.transform.position, m_ConnctedJoin0.transform.position, m_ConnctedJoin1.transform.position, (m_BaseJoin.Diameter / 2f) * .001f, m_BaseJoin.Length * .001f);
            Vector3 v0 = m_InnerVerts[0];
            Vector3 v1 = m_InnerVerts[1];
            Vector3 v2 = m_InnerVerts[2];

            // Get the normal of the triangle
            m_Normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

            // calculate the offset based on spacing
            //Vector3 normalOffset = m_Normal * ((SpacingRadius / 2f)) * .001f;
            #endregion

            if (makeInner)
            {
                // FIND INNER VERTS HERE
                m_TabInner = new GameObject().AddComponent<TriMesh>() as TriMesh;
                m_TabInner.name = "Tab Inner";
                m_TabInner.transform.SetParent(transform);
                m_TabInner.transform.position = Vector3.zero;
                m_TabInner.transform.rotation = Quaternion.identity;
                m_TabInner.m_Mat = mat;
                m_TabInner.m_Thickness = .001f;
                m_TabInner.CreateTri(v0, v1, v2, TriMesh.Extrusion.Center);
            }

            if (makeOuter)
            {
                m_TabOuter = new GameObject().AddComponent<TriMesh>() as TriMesh;
                m_TabOuter.name = "Tab Outer";
                m_TabOuter.transform.SetParent(transform);
                m_TabOuter.m_Mat = mat;
                m_TabOuter.m_Thickness = .001f;
                m_TabOuter.CreateTri(v0, v1, v2, TriMesh.Extrusion.Center);
            }

            //transform.SetParent (m_BaseJoin.transform );
        }

        void Update()
        {
            // needs to take into account the face width but can't be larger than the edge radius
            //float radius = ((m_BaseJoin.Diameter * .49f) ) - (  (m_BaseJoin.Diameter * .5f ) *  Mathf.Sin( ( MeshEdgeSolver.Instance.FaceThickness / m_BaseJoin.Diameter ) * Mathf.PI *.5f ) );
            //float radius = ((m_BaseJoin.Diameter * .5f) ) - (  (m_BaseJoin.Diameter * .5f ) *  ( MeshEdgeSolver.Instance.FaceThickness / m_BaseJoin.Diameter ) );
            //float radius = ( m_BaseJoin.Diameter * .48f );

            //transform.position = -m_BaseJoin.transform.position;

            float desiredRadius = (m_BaseJoin.ArmOuterDiameter / 2f) - (((m_BaseJoin.ArmOuterDiameter / 2f) - (m_BaseJoin.ArmInnerDiameter / 2f)) / 2f);
            float radius = desiredRadius - (SpacingRadius / desiredRadius);
            
            /*
            m_InnerVerts = TriMeshTest1.FindInnerVerts
            (
                m_BaseJoin.transform.position,			// v1 pos
                m_ConnctedJoin0.transform.position,		// v2 pos
                m_ConnctedJoin1.transform.position,		// v3 pos
                radius * .001f ,						// radius from the join
                m_BaseJoin.Length * .001f				// length of the tab 
            );
            */

            m_InnerVerts = TriMeshTest1.FindInnerVerts
            (
                m_BaseJoin.transform.position,          // v1 pos
                m_ConnctedJoin0.transform.position,     // v2 pos
                m_ConnctedJoin1.transform.position,     // v3 pos
                radius * .001f,                         // radius from the join
                m_Arm12.TotalLength * .001f,            // length of the tab 
                m_Arm13.TotalLength * .001f             // length of the tab 
            );

            Vector3 v0 = m_InnerVerts[0];
            Vector3 v1 = m_InnerVerts[1];
            Vector3 v2 = m_InnerVerts[2];

            // Get the normal of the triangle
            m_Normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

            // calculate the offset based on spacing
            Vector3 offset = m_Normal * ((SpacingRadius)) * .001f;

            //m_TabInner.UpdateTri( m_InnerVerts[0] + offset, m_InnerVerts[1] + offset, m_InnerVerts[2] + offset, m_WallThickness );

            if (m_TabInner != null)
                m_TabInner.UpdateTri(m_InnerVerts[0] + offset, m_InnerVerts[1] + offset, m_InnerVerts[2] + offset, m_WallThickness * .001f);

            if (m_TabOuter != null)
                m_TabOuter.UpdateTri(m_InnerVerts[0] - offset, m_InnerVerts[1] - offset, m_InnerVerts[2] - offset, m_WallThickness * .001f);
        }

        /*
        void OnDrawGizmos()
        {
            Vector3[] verts = new Vector3[5];
            //verts = TriMeshTest1.FindInnerVerts ( m_BaseJoin.transform.position, m_ConnctedJoin0.transform.position, m_ConnctedJoin1.transform.position, .0035f , .05f );
            verts = TriMeshTest1.FindInnerVerts(m_BaseJoin.transform.position, m_ConnctedJoin0.transform.position, m_ConnctedJoin1.transform.position, (m_BaseJoin.Diameter / 2f) * .001f, m_BaseJoin.Length * .001f);
            Gizmos.DrawLine(verts[0], verts[1]);
            Gizmos.DrawLine(verts[0], verts[2]);
            Gizmos.DrawLine(verts[1], verts[2]);
        }
        */
    }
}