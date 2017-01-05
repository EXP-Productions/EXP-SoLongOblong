using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityOSC;

[System.Serializable]
public class OSCListener
{
    public string m_Address = "/test";
    public int m_CurrentMessageCount;
    public bool m_ListenForChildren = false;
    public int m_DefualtValueIndex = 0;
    public Queue<KeyValuePair<string, List<object>>> m_Data;
    public bool m_IsInitialized { get { if (m_Data == null) return false; else return true; } }

    Dictionary<string, long> m_LastCheckTimeStamps = new Dictionary<string,long>();

    public bool m_ScaleIncoming = false;
    public Vector2 m_ScaleFromRange = new Vector2(0, 1);
    public Vector2 m_ScaleToRange = new Vector2(0, 1);


    public OSCListener(string address)
    {
        m_Address = address;
        OSCHandler.Instance.AddListener(this, false);
        m_Data = new Queue<KeyValuePair<string, List<object>>>();
    }

    public void Init()
    {
        OSCHandler.Instance.AddListener(this, false);
    }

    public OSCListener(string address, bool doNotDestroyOnLoad)
    {
        m_Address = address;
        OSCHandler.Instance.AddListener(this, doNotDestroyOnLoad);
        m_Data = new Queue<KeyValuePair<string, List<object>>>();
    }

    public bool Updated
    {
        get
        {
            bool returnval = false;
            foreach (OSCPacket oscPack in OSCHandler.Instance.m_OSCAll)
            {
                if (oscPack == null)
                {
                    Debug.Log("Null osc pack ");
                    break;
                }
                if (m_ListenForChildren)
                {
                    if (oscPack.Address.Contains(m_Address))
                    {
                        if (!m_LastCheckTimeStamps.ContainsKey(oscPack.Address)) 
                        {
                            m_LastCheckTimeStamps.Add(oscPack.Address, oscPack.TimeStamp);
                            m_Data.Enqueue(new KeyValuePair<string, List<object>>(oscPack.Address, oscPack.Data));

                            returnval = true;
                        }
                        else if (m_LastCheckTimeStamps[oscPack.Address] != oscPack.TimeStamp)
                        {
                            m_LastCheckTimeStamps[oscPack.Address] = oscPack.TimeStamp;
                            m_Data.Enqueue(new KeyValuePair<string, List<object>>(oscPack.Address, oscPack.Data));

                            returnval = true;
                        }
                    }
                }
                else
                {
                    if (oscPack.Address == m_Address)
                    {
                        if (!m_LastCheckTimeStamps.ContainsKey(oscPack.Address))
                        {
                            m_LastCheckTimeStamps.Add(oscPack.Address, oscPack.TimeStamp);
                            m_Data.Enqueue(new KeyValuePair<string, List<object>>(oscPack.Address, oscPack.Data));

                            returnval = true;
                        }
                        else if (m_LastCheckTimeStamps[oscPack.Address] != oscPack.TimeStamp)
                        {
                            m_LastCheckTimeStamps[oscPack.Address] = oscPack.TimeStamp;
                            m_Data.Enqueue(new KeyValuePair<string, List<object>>(oscPack.Address, oscPack.Data));

                            returnval = true;
                        }
                    }
                }
            }
            return returnval;
        }
    }

    public bool DataAvailable
    {
        get
        {
            return m_Data.Count > 0;

        }
    }

    public List<object> GetAllData()
    {
        return m_Data.Dequeue().Value;
    }


    public object GetData(int index)
    {
        return m_Data.Dequeue().Value[index];
    }

    public Byte[] GetBytes()
    {
        if (m_Data != null)
        {
            Byte[] bytes = (Byte[])m_Data.Dequeue().Value[0];
            return bytes;
        }
        else
        {
            Debug.Log("Data is null");
            return null;
        }

    }

    public float GetDataAsFloat(int index)
    {
        if (m_ScaleIncoming)
        {
            float val = (float)m_Data.Dequeue().Value[index];
            val = val.Scale(m_ScaleFromRange.x, m_ScaleFromRange.y, m_ScaleToRange.x, m_ScaleToRange.y);
            return val;
        }
        else
        {
            return (float)m_Data.Dequeue().Value[index];
        }
    }

    public float GetDataAsFloat()
    {
        if (m_ScaleIncoming)
        {
            float val = (float)m_Data.Dequeue().Value[m_DefualtValueIndex];
            val = val.Scale(m_ScaleFromRange.x, m_ScaleFromRange.y, m_ScaleToRange.x, m_ScaleToRange.y);
            return val;
        }
        else
        {
            return (float)m_Data.Dequeue().Value[m_DefualtValueIndex];
        }
    }
}
