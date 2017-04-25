using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    public class SLOManager : MonoBehaviour
    {
        public static SLOManager Instance { get { return m_Instance; } }
        static SLOManager m_Instance;

        public enum ComponentType
        {
            None,
            Join,
            Edge,
            Face,
        }

        public SLOObjectFromMesh m_MeshConvertor;

        #region Selection
        // Currently selected SLO Objct
        SLOObject m_SelectedSLOObject;
        public SLOObject SelectedSLOObject { get { return m_SelectedSLOObject; } }


        // The object that is currently selected component of the object
        GameObject m_SelectedComponent;

        // The component type of the object selected
        ComponentType m_SelectedComponentType = ComponentType.None;

        // Selected join
        SLO_Join m_SelectedJoin;
        public SLO_Join SelectedJoin
        {
            get { return m_SelectedJoin; }
            set
            {
                m_SelectedJoin = value;
                //m_GUI.SetSelectedJoint(m_SelectedJoin);
            }
        }
        #endregion
        
        string m_ProjectName = "New SLO Project";
        public string ProjectName { get { return m_ProjectName; } set { m_ProjectName = value; } }
        
        // GUI
        public SelectedObjectGUI m_GUI;

        public MeshFilter m_MeshFilterTest;          
        
        void Awake()
        {
            m_Instance = this;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                m_SelectedSLOObject = m_MeshConvertor.ConvertMesh(m_MeshFilterTest, true, true);
            }

            // Testing
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                if (m_SelectedComponentType == ComponentType.Face)
                {
                    SLO_Face face = m_SelectedComponent.GetComponent<SLO_Face>();

                    m_SelectedSLOObject.Faces.Remove(face);

                    // TODO: Remove tabs?

                    Destroy(face.gameObject);

                    m_GUI.m_SelectedElement = null;
                }
                else if (m_SelectedComponentType == ComponentType.Edge)
                {
                    SLO_Edge edge = m_SelectedComponent.GetComponent<SLO_Edge>();

                    // Remove form list
                    m_SelectedSLOObject.Edges.Remove(edge);

                    edge.Delete();

                    // set gui back to null
                    m_GUI.m_SelectedElement = null;
                }
            }
        }

        public void ConvertMesh( MeshFilter filter )
        {

        }

        public void SetSelectedObject(GameObject go)
        {
            m_SelectedComponent = go;

            if (m_SelectedComponent.GetComponent<ProceduralMesh_Tube>() != null)
            {
                if (m_SelectedComponent.transform.parent.GetComponent<SLO_Edge>() != null)
                {
                    m_SelectedComponentType = ComponentType.Edge;
                    m_SelectedComponent = m_SelectedComponent.transform.parent.gameObject;
                }
                else if (m_SelectedComponent.transform.parent.parent.GetComponent<SLO_Join>() != null)
                {
                    m_SelectedComponentType = ComponentType.Join;
                    m_SelectedComponent = m_SelectedComponent.transform.parent.parent.gameObject;
                    SelectedJoin = m_SelectedComponent.transform.parent.parent.GetComponent<SLO_Join>();

                }
            }
            else if (m_SelectedComponent.GetComponent<SLO_Face>() != null) m_SelectedComponentType = ComponentType.Face;
            else if (m_SelectedComponent.GetComponent<SLO_Join>() != null) m_SelectedComponentType = ComponentType.Join;
            else
            {
                m_SelectedComponentType = ComponentType.None;
            }

            print("Selected: " + m_SelectedComponent.name);

        }
    }
}
