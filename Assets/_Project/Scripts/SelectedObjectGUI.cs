using UnityEngine;
using System.Collections;

/// <summary>
/// SL o_ GU.
/// 
/// * Add joint, edge and face count
/// * Hide particular parts
/// </summary>
/// 
/// Chop onion
/// garlic
/// mushrooms half of them dont use them all julez will get harold on yo ass
/// 
namespace SoLongOblong
{
    public class SelectedObjectGUI : MonoBehaviour
    {
        public SLOManager m_Manager;
        SLOObject m_SelectedObj;

        #region Inputfields for component measurements
        // Edge diameter
        public UnityEngine.UI.InputField m_EdgeDiameterInput;

        // Joint length
        public UnityEngine.UI.InputField m_JointLengthInput;

        // Joint length
        public UnityEngine.UI.InputField m_FaceThicknessInput;

        // Joint length
        public UnityEngine.UI.InputField m_WallThicknessInput;
        #endregion

        #region Object stats text
        public UnityEngine.UI.Text m_TextEdgeLength;
        public UnityEngine.UI.Text m_TextAreaTotal;
        public UnityEngine.UI.Text m_TextJointCount;
        #endregion
        
     



        public UnityEngine.UI.Text m_UITxt_Tri;
        public GameObject m_SelectedElement;

        public UnityEngine.UI.Toggle m_ShowFacesToggle;

        public void SetSelected(GameObject go)
        {
            m_SelectedElement = go;
        }

        void Start()
        {
            m_Manager = SLOManager.Instance;

            m_EdgeDiameterInput.onEndEdit.AddListener((value) => SetEdgeDiameter(value));
            m_JointLengthInput.onEndEdit.AddListener((value) => SetJoinerLength(value));
            m_FaceThicknessInput.onEndEdit.AddListener((value) => SetFaceThickness(value));
            m_WallThicknessInput.onEndEdit.AddListener((value) => SetWallThickness(value));
            //m_ShowFacesToggle.onValueChanged.AddListener((value) => m_SelectedObj.m_ShowFaces = value);

            UpdateText();
        }

        public void UpdateText()
        {
            /*
            m_EdgeDiameterInput.text = m_SelectedObj.EdgeDiameter.ToString();
            m_JointLengthInput.text = m_SelectedObj.JoinerLength.ToString();
            m_FaceThicknessInput.text = m_SelectedObj.FaceThickness.ToString();
            m_WallThicknessInput.text = m_SelectedObj.WallThickness.ToString();
            m_ProjectNameInput.text = SLOManager.Instance.ProjectName;
            */
        }

        // Update is called once per frame
        void Update()
        {
            // hax
            if (m_SelectedElement != null)
            {
                if (m_SelectedElement.GetComponent<SLO_Face>())
                {
                    SLO_Face face = m_SelectedElement.GetComponent<SLO_Face>();
                    m_UITxt_Tri.text =
                        "Face: #" + face.m_Index + "\n" +
                            "Edge 0 Length: " + (face.Edge0Length * 1000).ToString("###") + "\n" +
                            "Edge 01 Angle: " + face.Angle01.ToString("##.#") + "\n" +
                            "Edge 1 Length: " + (face.Edge1Length * 1000).ToString("###") + "\n" +
                            "Edge 12 Angle: " + face.Angle12.ToString("##.#") + "\n" +
                            "Edge 2 Length: " + (face.Edge2Length * 1000).ToString("###") + "\n" +
                            "Edge 20 Angle: " + face.Angle20.ToString("##.#") + "\n";
                }
                else if (m_SelectedElement.transform.parent.transform.parent.GetComponent<SLO_Edge>())
                {
                    SLO_Edge edge = m_SelectedElement.transform.parent.transform.parent.GetComponent<SLO_Edge>();
                    m_UITxt_Tri.text =
                        "Edge: #" + edge.m_Index + "\n" +
                            "Length: " + (edge.Length * 1000).ToString("###") + "\n" +
                            "Connected to Joiners: " + edge.m_Join0.Index + " - " + edge.m_Join1.Index;
                }

            }


            // Update GUI
            m_TextEdgeLength.text = "Edge Length: " + (m_SelectedObj.TotalEdgeLength * 1000f).ToString("##");
            m_TextAreaTotal.text = "Area: " + (m_SelectedObj.TotalArea * 1000f).ToString("##");

            m_TextJointCount.text = "Joint count: " + m_SelectedObj.JointCount;
        }

        public void SetEdgeDiameter(string str)
        {
            float d = m_SelectedObj.EdgeDiameter;
            if (float.TryParse(str, out d))
            {
                m_SelectedObj.EdgeDiameter = d;
            }

            m_EdgeDiameterInput.text = m_SelectedObj.EdgeDiameter.ToString();
        }

        public void SetJoinerLength(string str)
        {
            float l = m_SelectedObj.JoinerLength;
            if (float.TryParse(str, out l))
            {
                m_SelectedObj.JoinerLength = l;
            }

            m_JointLengthInput.text = m_SelectedObj.JoinerLength.ToString();
        }

        public void SetFaceThickness(string str)
        {
            float t = m_SelectedObj.FaceThickness;
            if (float.TryParse(str, out t))
            {
                m_SelectedObj.FaceThickness = t;
            }

            m_FaceThicknessInput.text = m_SelectedObj.FaceThickness.ToString();
        }

       

        public void SetWallThickness(string str)
        {
            float t = m_SelectedObj.WallThickness;
            if (float.TryParse(str, out t))
            {
                m_SelectedObj.WallThickness = t;
            }

            m_WallThicknessInput.text = m_SelectedObj.WallThickness.ToString();
        }

       

    }
}
