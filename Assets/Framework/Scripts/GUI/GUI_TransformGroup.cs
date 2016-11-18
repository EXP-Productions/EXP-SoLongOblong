using UnityEngine;
using System.Collections;

public class GUI_TransformGroup : MonoBehaviour
{
    public ET_TransformGroup_Analysis m_TransformGroup;

    //GUI
    public UnityEngine.UI.Text m_COMText;
    public UnityEngine.UI.Text m_BoundsCOMText;
   
  //  public UnityEngine.UI.Text m_BoundsVolumeDeltaText;
    
    // Update is called once per frame
    void Update()
    {
        //GUI Update
        m_COMText.text = "Volume: " + m_TransformGroup.m_TransformCOM;
       // m_BoundsCOMText.text = "Bounds COM: " + m_Bounds.Width.ToDoubleDecimalString() + " / H: " + m_Bounds.Height.ToDoubleDecimalString() + " / D: " + m_Bounds.Depth.ToDoubleDecimalString();
       

    }
}
