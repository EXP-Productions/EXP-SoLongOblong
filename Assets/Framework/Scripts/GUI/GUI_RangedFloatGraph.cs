using UnityEngine;
using System.Collections;

public class GUI_RangedFloatGraph : MonoBehaviour
{
    public UnityEngine.UI.Text m_NameGUI;
    public GUI_Graph m_Graph;
    Ranged_Float m_RF;

    public void Initialize( Ranged_Float rf )
    {
        m_RF = rf;
        m_NameGUI.text = m_RF.m_Name;
    }

    public void Update()
    {
        m_Graph.AddValue(m_RF.m_NormalizedValue);
    }
}
