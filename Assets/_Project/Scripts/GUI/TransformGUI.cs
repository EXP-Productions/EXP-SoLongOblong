using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoLongOblong
{
    public class TransformGUI : MonoBehaviour
    {
        SLOManager m_Manager;
        public Text m_TransformName;
        
        public float m_DragPosScaler = 1;

        // Joint Position
        public UnityEngine.UI.InputField m_JointPosXInput;
        public UnityEngine.UI.InputField m_JointPosYInput;
        public UnityEngine.UI.InputField m_JointPosZInput;

        public ButtonClickDragEvent m_XDragButton;
        public ButtonClickDragEvent m_YDragButton;
        public ButtonClickDragEvent m_ZDragButton;

        // Use this for initialization
        void Start()
        {
            m_JointPosXInput.onEndEdit.AddListener((value) => SetJointPosX(value));
            m_JointPosYInput.onEndEdit.AddListener((value) => SetJointPosY(value));
            m_JointPosZInput.onEndEdit.AddListener((value) => SetJointPosZ(value));

            m_XDragButton.OnDragUpdate.AddListener((value) => UpdateJointPosX(value));
            m_YDragButton.OnDragUpdate.AddListener((value) => UpdateJointPosY(value));
            m_ZDragButton.OnDragUpdate.AddListener((value) => UpdateJointPosZ(value));
        }
              
        public void SetSelectedJoint(SLO_Join join)
        {
            m_JointPosXInput.text = m_Manager.SelectedJoin.transform.position.x.ToString();
            m_JointPosYInput.text = m_Manager.SelectedJoin.transform.position.y.ToString();
            m_JointPosZInput.text = m_Manager.SelectedJoin.transform.position.z.ToString();
        }

        #region SetSelectedJoint X Y Z
        public void SetJointPosX(string str)
        {
            float pos = m_Manager.SelectedJoin.transform.position.x;
            if (float.TryParse(str, out pos))
            {
                m_Manager.SelectedJoin.transform.SetWorldX(pos);
            }

            m_JointPosXInput.text = m_Manager.SelectedJoin.transform.position.x.ToString();
        }

        public void SetJointPosY(string str)
        {
            float pos = m_Manager.SelectedJoin.transform.position.y;
            if (float.TryParse(str, out pos))
            {
                m_Manager.SelectedJoin.transform.SetWorldY(pos);
            }

            m_JointPosYInput.text = m_Manager.SelectedJoin.transform.position.y.ToString();
        }
      
        public void SetJointPosZ(string str)
        {
            float pos = m_Manager.SelectedJoin.transform.position.z;
            if (float.TryParse(str, out pos))
            {
                m_Manager.SelectedJoin.transform.SetWorldZ(pos);
            }

            m_JointPosZInput.text = m_Manager.SelectedJoin.transform.position.z.ToString();
        }
        #endregion


        public void UpdateJointPosX(float val)
        {
            float pos = m_Manager.SelectedJoin.transform.position.x + (val * m_DragPosScaler);
            m_Manager.SelectedJoin.transform.SetWorldX(pos);
            m_JointPosXInput.text = m_Manager.SelectedJoin.transform.position.x.ToString();
        }

        public void UpdateJointPosY(float val)
        {
            float pos = m_Manager.SelectedJoin.transform.position.y + (val * m_DragPosScaler);
            m_Manager.SelectedJoin.transform.SetWorldY(pos);
            m_JointPosYInput.text = m_Manager.SelectedJoin.transform.position.y.ToString();
        }

        public void UpdateJointPosZ(float val)
        {
            float pos = m_Manager.SelectedJoin.transform.position.z + (val * m_DragPosScaler);
            m_Manager.SelectedJoin.transform.SetWorldZ(pos);
            m_JointPosZInput.text = m_Manager.SelectedJoin.transform.position.z.ToString();
        }
    }
}
