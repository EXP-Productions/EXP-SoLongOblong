using UnityEngine;
using System.Collections.Generic;

public class PortMidiOutDeviceList : PortMidiDeviceList
{
    public override void LoadDevices()
    {
        int outCount = 0;
        foreach (PortMidiSharp.MidiDeviceInfo mdi in PortMidiSharp.MidiDeviceManager.AllDevices)
        {
            if (mdi.IsOutput)
            {
                outCount++;
            }
        }
        m_DeviceArray = new string[outCount];
        int i = 0;
        foreach (PortMidiSharp.MidiDeviceInfo mdi in PortMidiSharp.MidiDeviceManager.AllDevices)
        {
            if (mdi.IsOutput)
            {
                m_DeviceArray[i] = mdi.ID.ToString() + "|" + mdi.Name ;
                i++;
            }
        }
        deviceListLoaded = true;
    }
}
