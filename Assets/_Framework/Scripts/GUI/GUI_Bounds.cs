using UnityEngine;
using System.Collections;

public class GUI_Bounds : MonoBehaviour
{
    public Bounds3D m_Bounds;

    //GUI
    public UnityEngine.UI.Text m_BoundsDimensionText;
    public UnityEngine.UI.Text m_BoundsVolumeText;
    public UnityEngine.UI.Text m_BoundsVolumeDeltaText;

	
	// Update is called once per frame
	void Update () 
    { 
        //GUI Update
        m_BoundsDimensionText.text = "W: " + m_Bounds.Width.ToDoubleDecimalString() + " / H: " + m_Bounds.Height.ToDoubleDecimalString() + " / D: " + m_Bounds.Depth.ToDoubleDecimalString();
        m_BoundsVolumeText.text = "Volume: " + m_Bounds.Volume.ToDoubleDecimalString();

        m_Bounds.UpdateDeltas( false );
        m_BoundsVolumeDeltaText.text = "Vol Delta: " + m_Bounds.m_VolumeDelta.ToDoubleDecimalString(); 
	}
}
