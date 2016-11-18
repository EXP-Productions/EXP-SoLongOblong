using UnityEngine;
using System.Collections;

// Clean up
// used for getting positions in live
public class ET_Transform_OSCIn : MonoBehaviour 
{
    public enum RecieveState
    {
        Local,
        World,
        Off,
    }

    public string m_OSCName;

    public RecieveState m_GetPos = RecieveState.World;
    public RecieveState m_GetRot = RecieveState.Off;
    public RecieveState m_GetScale = RecieveState.Off;

    Transform m_Tform;
    OSCHandler m_OSCHandler;

    OSCListener m_PosOSC;
    OSCListener m_RotOSC;
    OSCListener m_ScaleOSC;

    // hack until multivalue osc fixed
    OSCListener m_PosXOSC;
    OSCListener m_PosYOSC;
    OSCListener m_PosZOSC;

    Vector3 pos;
    Vector3 rot;
    Vector3 scale;


	// Use this for initialization
	public void Start ( ) 
    {
        m_Tform = transform;

        if (m_GetPos != RecieveState.Off) m_PosOSC = new OSCListener(m_OSCName + "/pos");
        if (m_GetRot != RecieveState.Off) m_RotOSC = new OSCListener(m_OSCName + "/rot");
        if (m_GetScale != RecieveState.Off) m_ScaleOSC = new OSCListener(m_OSCName + "/scale");

        // hack until multivalue osc fixed
        m_PosXOSC = new OSCListener(m_OSCName + "/pos/x");
        m_PosYOSC = new OSCListener(m_OSCName + "/pos/y");
        m_PosZOSC = new OSCListener(m_OSCName + "/pos/z");
	}
	
	// Update is called once per frame
	void Update () 
    {
        if( m_GetPos != RecieveState.Off )
        {
            // hack until multivalue osc fixed
            if (m_PosXOSC.Updated) pos.x = m_PosXOSC.GetDataAsFloat();
            if (m_PosYOSC.Updated) pos.y = m_PosYOSC.GetDataAsFloat();
            if (m_PosZOSC.Updated) pos.z = m_PosZOSC.GetDataAsFloat();

            m_Tform.position = pos;

            /*
           if( m_PosOSC.Updated )
           {
               pos.x = m_PosOSC.GetDataAsFloat(0);
               pos.y = m_PosOSC.GetDataAsFloat(1);
               pos.z = m_PosOSC.GetDataAsFloat(2);

               if (m_GetPos == RecieveState.Local) m_Tform.localPosition = pos;
               else if (m_GetPos == RecieveState.Local) m_Tform.position = pos;
           }
             * */
        }
	}
}
