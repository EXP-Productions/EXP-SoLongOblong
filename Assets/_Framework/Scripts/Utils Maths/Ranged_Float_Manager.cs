using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ranged_Float_Manager : MonoBehaviour
{
    // Static singleton property
    static Ranged_Float_Manager m_Instance { get; set; }

    public static Ranged_Float_Manager Instance
    {       
        get
        {
            if (m_Instance == null)
            {
                GameObject managerParent = GameObject.Find("_Managers");
                if (managerParent == null)
                    managerParent = new GameObject("_Managers");

                m_Instance = new GameObject("Ranged Float Manager").AddComponent<Ranged_Float_Manager>();
                m_Instance.transform.parent = managerParent.transform;
            }

            return m_Instance;
        }
    }

    List<Ranged_Float> m_RangedFloats = new List<Ranged_Float>();

    public void AddRangedFloat( Ranged_Float rf )
    {
        m_RangedFloats.Add(rf);
    }
	
	// Update is called once per frame
	void Update () 
    {
        if( Input.GetKeyDown( KeyCode.R) )
        {
            foreach (Ranged_Float rf in m_RangedFloats)
                rf.m_AdjustRange = !rf.m_AdjustRange;
        }	
	}

    public void AdjustRanges( bool adjust )
    {
        print("here: " + adjust);
        foreach (Ranged_Float rf in m_RangedFloats)
            rf.m_AdjustRange = adjust;
    }
}
