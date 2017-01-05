using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    public class SLOManager : MonoBehaviour
    {
        public enum ComponentType
        {
            None,
            Join,
            Edge,
            Face,
        }

        // The object that is currently selected
        public GameObject m_SelectedObject;

        // The component type of the object selected
        ComponentType m_SelectedComponentType = ComponentType.None;

        // GUI
        public SLO_GUI m_GUI;

        // Selected join
        SLO_Join m_SelectedJoin;
        public SLO_Join SelectedJoin
        {
            get { return m_SelectedJoin; }
            set
            {
                m_SelectedJoin = value;
                m_GUI.SetSelectedJoint(m_SelectedJoin);
            }
        }

        public void SetSelectedObject(GameObject go)
        {
            m_SelectedObject = go;

            if (m_SelectedObject.GetComponent<ProceduralMesh_Tube>() != null)
            {
                if (m_SelectedObject.transform.parent.GetComponent<SLO_Edge>() != null)
                {
                    m_SelectedComponentType = ComponentType.Edge;
                    m_SelectedObject = m_SelectedObject.transform.parent.gameObject;
                }
                else if (m_SelectedObject.transform.parent.parent.GetComponent<SLO_Join>() != null)
                {
                    m_SelectedComponentType = ComponentType.Join;
                    m_SelectedObject = m_SelectedObject.transform.parent.parent.gameObject;
                    SelectedJoin = m_SelectedObject.transform.parent.parent.GetComponent<SLO_Join>();

                }
            }
            else if (m_SelectedObject.GetComponent<SLO_Face>() != null) m_SelectedComponentType = ComponentType.Face;
            else if (m_SelectedObject.GetComponent<SLO_Join>() != null) m_SelectedComponentType = ComponentType.Join;
            else
            {
                m_SelectedComponentType = ComponentType.None;
            }

            print("Selected: " + m_SelectedObject.name);

        }
    }
}
