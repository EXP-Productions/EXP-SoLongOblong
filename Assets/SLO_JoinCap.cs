using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    /// <summary>
    /// SLO join cap essentailly combines all the tabs to make a cap piece that the faces can affix to
    /// </summary>
    public class SLO_JoinCap : MonoBehaviour
    {
        SLO_Join m_Join;
        TriFanMesh m_TriFanMesh;

        Transform[] m_Transforms;

        float m_EdgeLength = .01f;

        public void Init( SLO_Join join )
        {
            m_Join = join;
            m_TriFanMesh = gameObject.AddComponent<TriFanMesh>();

            m_Transforms = new Transform[m_Join.ConnectedEdges.Count];

            transform.SetParent(m_Join.transform);
            transform.localPosition = Vector3.zero;        

            // Create transforms
            for (int i = 0; i < m_Join.ConnectedEdges.Count; i++)
            {
                Transform t = new GameObject(i.ToString()).transform;
                t.SetParent(transform);
                m_Transforms[i] = t;
                m_Transforms[i].position = m_Join.ConnectedEdges[i].GetOppositeJoinPos(m_Join);
            }

            m_TriFanMesh.Init(m_Transforms);
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < m_Join.ConnectedEdges.Count; i++)
            {
                m_Transforms[i].position = m_Join.ConnectedEdges[i].GetOppositeJoinPos(m_Join);
            }           
        }
    }
}
