using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoLongOblong
{
    public class ExporterGUI : MonoBehaviour
    {
        public SLOObjectExporter m_Exporter;

        public Button m_OutputButton;
        public InputField m_ProjectName;

        void Start()
        {
            m_ProjectName.onEndEdit.AddListener((value) => m_Exporter.m_ProjectName = value);
            m_OutputButton.onClick.AddListener(() => m_Exporter.OutputAssets());
        }

        public void UpdateProjectName(string name)
        {
            m_ProjectName.text = name;
        }
    }
}
