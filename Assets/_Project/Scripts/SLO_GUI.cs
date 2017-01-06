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
    public class SLO_GUI : MonoBehaviour
    {
        public SLOManager m_Manager;

        // Edge diameter
        public UnityEngine.UI.InputField m_EdgeDiameterInput;

        // Joint length
        public UnityEngine.UI.InputField m_JointLengthInput;

        // Joint length
        public UnityEngine.UI.InputField m_FaceThicknessInput;

        // Joint length
        public UnityEngine.UI.InputField m_WallThicknessInput;

        public UnityEngine.UI.InputField m_ProjectNameInput;

        public UnityEngine.UI.Text m_TextEdgeLength;
        public UnityEngine.UI.Text m_TextAreaTotal;

        public UnityEngine.UI.Text m_TextJointCount;


        // Joint Position
        public UnityEngine.UI.InputField m_JointPosXInput;
        public UnityEngine.UI.InputField m_JointPosYInput;
        public UnityEngine.UI.InputField m_JointPosZInput;


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
            m_ProjectNameInput.onEndEdit.AddListener((value) => SLOManager.Instance.ProjectName = value);

            m_ShowFacesToggle.onValueChanged.AddListener((value) => m_Mesh.m_ShowFaces = value);

            UpdateText();
        }

        public void UpdateText()
        {
            m_EdgeDiameterInput.text = m_Mesh.EdgeDiameter.ToString();
            m_JointLengthInput.text = m_Mesh.JoinerLength.ToString();
            m_FaceThicknessInput.text = m_Mesh.FaceThickness.ToString();
            m_WallThicknessInput.text = m_Mesh.WallThickness.ToString();
            m_ProjectNameInput.text = SLOManager.Instance.ProjectName;
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
            m_TextEdgeLength.text = "Edge Length: " + (m_Mesh.TotalEdgeLength * 1000f).ToString("##");
            m_TextAreaTotal.text = "Area: " + (m_Mesh.TotalArea * 1000f).ToString("##");

            m_TextJointCount.text = "Joint count: " + m_Mesh.JointCount;
        }

        public void SetEdgeDiameter(string str)
        {
            float d = m_Mesh.EdgeDiameter;
            if (float.TryParse(str, out d))
            {
                m_Mesh.EdgeDiameter = d;
            }

            m_EdgeDiameterInput.text = m_Mesh.EdgeDiameter.ToString();
        }

        public void SetJoinerLength(string str)
        {
            float l = m_Mesh.JoinerLength;
            if (float.TryParse(str, out l))
            {
                m_Mesh.JoinerLength = l;
            }

            m_JointLengthInput.text = m_Mesh.JoinerLength.ToString();
        }

        public void SetFaceThickness(string str)
        {
            float t = m_Mesh.FaceThickness;
            if (float.TryParse(str, out t))
            {
                m_Mesh.FaceThickness = t;
            }

            m_FaceThicknessInput.text = m_Mesh.FaceThickness.ToString();
        }

        public void SetSelectedJoint(SLO_Join join)
        {
            m_JointPosXInput.text = m_Mesh.SelectedJoin.transform.position.x.ToString();
            m_JointPosYInput.text = m_Mesh.SelectedJoin.transform.position.y.ToString();
            m_JointPosZInput.text = m_Mesh.SelectedJoin.transform.position.z.ToString();
        }

        public void SetWallThickness(string str)
        {
            float t = m_Mesh.WallThickness;
            if (float.TryParse(str, out t))
            {
                m_Mesh.WallThickness = t;
            }

            m_WallThicknessInput.text = m_Mesh.WallThickness.ToString();
        }

        public void SetJointPosX(string str)
        {
            float pos = m_Mesh.SelectedJoin.transform.position.x;
            if (float.TryParse(str, out pos))
            {
                m_Mesh.SelectedJoin.transform.SetWorldX(pos);
            }

            m_JointPosXInput.text = m_Mesh.SelectedJoin.transform.position.x.ToString();
        }

        public void SetJointPosY(string str)
        {
            float pos = m_Mesh.SelectedJoin.transform.position.y;
            if (float.TryParse(str, out pos))
            {
                m_Mesh.SelectedJoin.transform.SetWorldY(pos);
            }

            m_JointPosYInput.text = m_Mesh.SelectedJoin.transform.position.y.ToString();
        }

        public void SetJointPosZ(string str)
        {
            float pos = m_Mesh.SelectedJoin.transform.position.z;
            if (float.TryParse(str, out pos))
            {
                m_Mesh.SelectedJoin.transform.SetWorldZ(pos);
            }

            m_JointPosZInput.text = m_Mesh.SelectedJoin.transform.position.z.ToString();
        }

        public float m_DragPosScaler = 1;

        public void UpdateJointPosX(float val)
        {
            float pos = m_Mesh.SelectedJoin.transform.position.x + (val * m_DragPosScaler);
            m_Mesh.SelectedJoin.transform.SetWorldX(pos);
            m_JointPosXInput.text = m_Mesh.SelectedJoin.transform.position.x.ToString();
        }

        public void UpdateJointPosY(float val)
        {
            float pos = m_Mesh.SelectedJoin.transform.position.y + (val * m_DragPosScaler);
            m_Mesh.SelectedJoin.transform.SetWorldY(pos);
            m_JointPosYInput.text = m_Mesh.SelectedJoin.transform.position.y.ToString();
        }

        public void UpdateJointPosZ(float val)
        {
            float pos = m_Mesh.SelectedJoin.transform.position.z + (val * m_DragPosScaler);
            m_Mesh.SelectedJoin.transform.SetWorldZ(pos);
            m_JointPosZInput.text = m_Mesh.SelectedJoin.transform.position.z.ToString();
        }
    }
}
