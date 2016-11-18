using UnityEngine;
using System.Collections;

// Clean up
// used for getting positions in live
public class ET_Transform_OSCOut : MonoBehaviour 
{
    public enum SendState
    {
        Local,
        World,
        Off,
    }

    public string m_OSCName;

    public SendState m_SendPos = SendState.World;
    public SendState m_SendRot = SendState.Off;
    public SendState m_SendScale = SendState.Off;

    Transform m_Tform;
    OSCHandler m_OSCHandler;

    void Start()
    {
        m_Tform = transform;
        m_OSCHandler = OSCHandler.Instance;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (m_SendPos == SendState.World)
        {
            m_OSCHandler.SendOSCMessage(m_OSCName + "/pos", m_Tform.position);

            // hacks until multi value osc can be sorted
            m_OSCHandler.SendOSCMessage(m_OSCName + "/pos/x", m_Tform.position.x);
            m_OSCHandler.SendOSCMessage(m_OSCName + "/pos/y", m_Tform.position.y);
            m_OSCHandler.SendOSCMessage(m_OSCName + "/pos/z", m_Tform.position.z);
        }
        else if (m_SendPos == SendState.Local) m_OSCHandler.SendOSCMessage(m_OSCName + "/pos", m_Tform.localPosition);

        if (m_SendRot == SendState.World) m_OSCHandler.SendOSCMessage(m_OSCName + "/rot", m_Tform.rotation.eulerAngles);
        else if (m_SendRot == SendState.Local) m_OSCHandler.SendOSCMessage(m_OSCName + "/rot", m_Tform.localRotation.eulerAngles);              
	}
}
