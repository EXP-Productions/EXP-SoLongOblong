using UnityEngine;
using System.Collections.Generic;

public class PortMidiDeviceList 
{
    public bool deviceListLoaded = false;
    protected string[] m_DeviceArray = null;

    public virtual void LoadDevices()
    { 
        
    }
    
    public string[] DeviceArray
    {
        get
        {
            if (!deviceListLoaded)
            {
                LoadDevices();
            }
            return m_DeviceArray;
        }
    }

}
