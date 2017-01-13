using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    public class SLOResourceManager : MonoBehaviour
    {
        public static SLOResourceManager Instance { get { return m_Instance; } }
        static SLOResourceManager m_Instance;

        public Material m_MatJoiner;
        public Material m_MatEdge;
        public Material m_MatFace;
        public Material m_FlatFaceMat;

        void Awake()
        {
            m_Instance = this;
        }
    }
}
