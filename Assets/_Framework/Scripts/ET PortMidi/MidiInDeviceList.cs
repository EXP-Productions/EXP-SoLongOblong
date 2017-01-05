using UnityEngine;
using System.Collections.Generic;
using System;

public class PortMidiInDeviceList : PortMidiDeviceList
{
    public override void LoadDevices()
    {
        int inCount = 0;
        foreach (PortMidiSharp.MidiDeviceInfo mdi in PortMidiSharp.MidiDeviceManager.AllDevices)
        {
            if (mdi.IsInput)
            {
                inCount++;
            }
        }
        m_DeviceArray = new string[inCount];
        int i = 0;
        foreach (PortMidiSharp.MidiDeviceInfo mdi in PortMidiSharp.MidiDeviceManager.AllDevices)
        {
            if (mdi.IsInput)
            {
                m_DeviceArray[i] = mdi.ID.ToString() + "|" + mdi.Name.Substring(0,(Math.Min(50, mdi.Name.Length)));
                i++;
            }
        }
        deviceListLoaded = true;
    }
}
