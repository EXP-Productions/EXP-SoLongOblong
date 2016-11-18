using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
// hand to hand
// hand to toe
// Check previous CCL for quality names
// Dynamic size of space based on points

// Add slider for scale
// Add slider for trail time

// Think about events to add
// Foot step/ ground touch
// Foot/ground sweep
// Jump
// Air time

public class ET_TransformGroup_Analysis : MonoBehaviour
{
    // Array of all the transforms to analyse
    public ET_Transform_Analysis[] m_AnalysedTransforms;
    List<ET_Transform_Analysis> m_UpdatedList = new List<ET_Transform_Analysis>();

    // Scale of transforms
    private float m_TransformScale = .1f;

    public float TransformScale
    {
        set
        { 
            m_TransformScale = value;

            foreach (ET_Transform_Analysis tf in m_AnalysedTransforms)
                tf.transform.localScale = m_TransformScale * Vector3.one;
        }
    }
    // Length in time of trails
    public float m_TrailTime = .3f;

    // Reference to the bounding volume
    public Bounds3D m_Bounds_All;
    public Bounds3D m_Bounds_Upper;
    public Bounds3D m_Bounds_Lower;

    // Tracks the rate of change of the bounding volume
    public Ranged_Float m_BoundsVolumeRateOfChange = new Ranged_Float();

    // Average velocity of all transforms
    public Vector3 m_TransformAvVel;

    // The center of mass of all transforms
    public Vector3 m_TransformCOM;
    Vector3 m_TransformCOMVel;
   
    // The velocity of the bounding volume
    public Vector3 m_BoundsVel;
    Vector3 m_PrevBoundsCOM;

    // Trackable movement qualities
    // Space Intensity (Lazy versus Exhausting)
    Ranged_Float m_SpaceIntensity;
    public UnityEngine.UI.Text m_SpaceIntensityGUI;
    // Speed (Slow versus Fast)
    Ranged_Float m_Speed;
    //public UnityEngine.UI.Text m_SpeedGUI;
    public GUI_RangedFloatGraph m_SpeedRFGUI;

    // Smoothness (Legato versus Stacatto)
    Ranged_Float m_Smoothness;
    //public UnityEngine.UI.Text m_SmoothnessGUI;
    public GUI_RangedFloatGraph m_SmoothRFGUI;
    // Reach (In versus Out)
    Ranged_Float m_Reach;

    public float Reach
    {
        get { return m_Reach.m_NormalizedValue; }
    }
    //public UnityEngine.UI.Text m_ReachGUI;
    public GUI_RangedFloatGraph m_ReachRFGUI;
    // Density (High/Low)
    Ranged_Float m_Density;
   // public UnityEngine.UI.Text m_DensityGUI;
    public GUI_RangedFloatGraph m_DensityRFGUI;
    // Coherence (Isolated vs. Widespread)
    Ranged_Float m_Coherence;
    public UnityEngine.UI.Text m_CoherenceGUI;
    // Travel Intensity (local vs. far)
    Ranged_Float m_TravelIntensity;
    public UnityEngine.UI.Text m_TravelIntensityGUI;
    public GUI_RangedFloatGraph m_TravelRFGUI;
    // Rhythm (Steady vs. Chaotic)
    Ranged_Float m_Rhythm;
    public UnityEngine.UI.Text m_RhythmGUI;
    // Harmony (Harmonic vs Disharmonic)
    Ranged_Float m_Harmony;
    public UnityEngine.UI.Text m_HarmonyGUI;
    // Energy (Calm vs Energetic) - Ratio of fast to slow velocities?
    Ranged_Float m_Energy;
    public UnityEngine.UI.Text m_EnergyGUI;
    // Contraction/Expansion (Average velocity toward or away from the COM)
    Ranged_Float m_Expansion;
    public UnityEngine.UI.Text m_ExpansionGUI;

    


    public bool m_Pause = false;

    // Brads values
    public bool m_AdjustRangesOnStart = true;

    // Upper and lower rotational vectors to find the differencne between upper and lower. 
    // essentailly a twist approx

    Vector3 m_UpperRotationVec;
    Vector3 m_UpperRotPosFrom;
    ET_Transform_Analysis m_FurthestUpper00;
    ET_Transform_Analysis m_FurthestUpper01;

    Vector3 m_LowerRotationVec;
    Vector3 m_LowerRotPosFrom;
    ET_Transform_Analysis m_FurthestLower00;
    ET_Transform_Analysis m_FurthestLower01;
    float m_Twist;


    // GUI
    public RectTransform m_GUIPanel;
    public GUI_SliderButton m_SliderButton;


    // OSC variables
    public bool m_OutputOSC = false;
    OSCHandler m_OSCHandler;
    public string m_OSCPrefix = "/tformGroup";

    public GUI_Bounds m_GUIBounds;

    void Awake()
    {
        m_SpaceIntensity = new Ranged_Float("Space Intensity", m_SpaceIntensityGUI);
        m_Speed = new Ranged_Float("Speed", m_SpeedRFGUI);
        m_Smoothness = new Ranged_Float("Smoothness", m_SmoothRFGUI);
        m_Reach = new Ranged_Float("Reach", m_ReachRFGUI);
        m_Density = new Ranged_Float("Density", m_DensityRFGUI);
        m_Coherence = new Ranged_Float("Coherence", m_CoherenceGUI);
        m_TravelIntensity = new Ranged_Float("Travel Intensity", m_TravelRFGUI);
        m_Rhythm = new Ranged_Float("Rhythm", m_RhythmGUI);
        m_Harmony = new Ranged_Float("Harmony", m_HarmonyGUI);
        m_Energy = new Ranged_Float("Energy", m_EnergyGUI);
        m_Expansion = new Ranged_Float("Expansion", m_ExpansionGUI); 
    }

	void Start () 
    {
        Application.targetFrameRate = 60;

        // Initialize bounds
        m_Bounds_All = new Bounds3D();
        m_Bounds_Upper = new Bounds3D();
        m_Bounds_Lower = new Bounds3D();

        // Set reference to the Bound in the GUI Bounds
        m_GUIBounds.m_Bounds = m_Bounds_All;

        m_OSCHandler = OSCHandler.Instance;

        // Get transforms if the group has any children
        if (transform.childCount != 0)
            UpdateTransforms();       

	}

    void OnTimeLineStart()
    {
        SetTrailTime( m_TrailTime );
    }

	void OnTimeLineEnd () 
    {        
        SetTrailTime( 0 );
	}

    bool m_RerangeValues = false;
    public void RerangeValueToggle()
    {
        m_RerangeValues = !m_RerangeValues;
        Ranged_Float_Manager.Instance.AdjustRanges(m_RerangeValues);        
    }

    public float m_CenterOfMassSmoothing = 8;

    public void UpdateTransforms()
    {
        m_AnalysedTransforms = null;
        m_AnalysedTransforms = (ET_Transform_Analysis[])GetComponentsInChildren<ET_Transform_Analysis>();
        foreach (ET_Transform_Analysis tfrom in m_AnalysedTransforms)
        {           
                tfrom.OnGroundCollission += OnGroundCollision;            
        }

        SetTrailTime(m_TrailTime);
        
    }

    public ParticleSystem m_StepPSys;
    void OnGroundCollision( Vector3 pos )
    {
        // Spawn particle at position
        m_StepPSys.Emit(pos, Vector3.zero, 1, .3f, Color.red);
    }

    bool m_FrameSkip = false;


	void Update () 
    {
        if (m_FrameSkip)
        {
            m_FrameSkip = false;
            return;
        }
        else
            m_FrameSkip = true;

        m_UpdatedList.Clear();
        foreach (ET_Transform_Analysis tform in m_AnalysedTransforms)
            if (tform.m_UpdatedThisFrame)
                m_UpdatedList.Add(tform);



        // Update transfomr scale * Needs to be moved to not every frame


        
       
       

        // If there are transforms to analyse
        if ( m_UpdatedList.Count == 0 || m_Pause ) return;

        // Update bounds
        m_Bounds_All.ResetBounds(m_UpdatedList[0].transform.position);
        m_TransformAvVel = Vector3.zero;
        for (int i = 0; i < m_UpdatedList.Count; i++)
		{
            if (m_UpdatedList[i].Velocity != Vector3.zero)
            {
                m_Bounds_All.ExtendToInclude(m_UpdatedList[i].Tform.position);
                m_TransformAvVel += m_UpdatedList[i].m_Velocity;
            }
		}

        m_TransformAvVel /= m_UpdatedList.Count;


        m_BoundsVolumeRateOfChange.RawValue = m_Bounds_All.Volume;

        // Calculate center of mass
        Vector3 prevCOM = m_TransformCOM;
        m_TransformCOM = Vector3.zero;
        float count = 0;
        for (int i = 0; i < m_UpdatedList.Count; i++)
        {
            if (m_UpdatedList[i].Velocity != Vector3.zero)
            {
                m_TransformCOM += m_UpdatedList[i].Tform.position;
                count++;
            }
        }
        m_TransformCOM /= count;

        m_TransformCOMVel = ( m_TransformCOM - prevCOM ) / Time.deltaTime;

        // Calculate bounds velocity
        m_BoundsVel = ( m_Bounds_All.Center - m_PrevBoundsCOM ) / Time.deltaTime;
        m_PrevBoundsCOM = m_Bounds_All.Center;

        // Twist
        // Create a list of upper and lower markers. 
        // Try splitting by COM.y or by median height
        List<ET_Transform_Analysis> upperMarkers = new List<ET_Transform_Analysis>();
        List<ET_Transform_Analysis> lowerMarkers = new List<ET_Transform_Analysis>();

        for (int i = 0; i < m_UpdatedList.Count; i++)
        {
            if( m_UpdatedList[i].Tform.position.y > m_TransformCOM.y ) upperMarkers.Add(m_UpdatedList[i]);
            else lowerMarkers.Add(m_UpdatedList[i]);
        }

        Vector2 longestVector = Vector2.zero;
        for (int i = 0; i < upperMarkers.Count; i++)
        {
            for (int j = 0; j < upperMarkers.Count; j++)
            {
                Vector2 pos1 = new Vector2(upperMarkers[i].Tform.position.x, upperMarkers[i].Tform.position.z);
                Vector2 pos2 = new Vector2(upperMarkers[j].Tform.position.x, upperMarkers[j].Tform.position.z);

                Vector2 vec = pos1 - pos2;
                if (vec.magnitude > longestVector.magnitude)
                {
                    m_FurthestUpper00 = upperMarkers[i];
                   // m_FurthestUpper00.SetColour(Color.blue);
                    m_FurthestUpper01 = upperMarkers[j];
                    //m_FurthestUpper01.SetColour(Color.blue);

                    m_UpperRotPosFrom = upperMarkers[j].Tform.position;
                    longestVector = vec;
                }
            }
        }
        m_UpperRotationVec = new Vector3(longestVector.x, m_UpperRotPosFrom.y, longestVector.y);

        longestVector = Vector2.zero;
        for (int i = 0; i < lowerMarkers.Count; i++)
        {
            for (int j = 0; j < lowerMarkers.Count; j++)
            {
                Vector2 pos1 = new Vector2(lowerMarkers[i].Tform.position.x, lowerMarkers[i].Tform.position.z);
                Vector2 pos2 = new Vector2(lowerMarkers[j].Tform.position.x, lowerMarkers[j].Tform.position.z);

                Vector2 vec = pos1 - pos2;
                if (vec.magnitude > longestVector.magnitude)
                {
                    m_FurthestLower00 = lowerMarkers[i];
                    m_FurthestLower01 = lowerMarkers[j];

                    m_LowerRotPosFrom = lowerMarkers[j].Tform.position;
                    longestVector = vec;
                }
            }
        }
        m_LowerRotationVec = new Vector3(longestVector.x, m_LowerRotPosFrom.y, longestVector.y);


        // Upper and lower bounds
        m_Bounds_Lower.ResetBounds(lowerMarkers[0].transform.position);
        for (int i = 0; i < lowerMarkers.Count; i++)
        {
            m_Bounds_Lower.ExtendToInclude(lowerMarkers[i].Tform.position);            
        }

        m_Bounds_Upper.ResetBounds(upperMarkers[0].transform.position);
        for (int i = 0; i < upperMarkers.Count; i++)
        {
            m_Bounds_Upper.ExtendToInclude(upperMarkers[i].Tform.position);
        }


        // Update movement qualities
       // m_SpaceIntensity

        
        
        // Smoothness or noisyness
        // Summing all the rotational velocities instead of getting the delta of average velocity
        float smooth = 0;
        count = 0;
        for (int i = 0; i < m_UpdatedList.Count; i++)
        {
            if (m_UpdatedList[i].m_RotationalVelocity != 0)
            {
                smooth += m_UpdatedList[i].m_RotationalVelocity;
                count++;
            }

        }
        smooth /= count;
        m_Smoothness.RawValue = smooth;

        // Reach
        // Furthest point form Center of mass
        // Perhaps add evetn for reach joint change
        ET_Transform_Analysis furthesetTform = m_UpdatedList[0];
        float furthestDistance = 0;
        int furthestIndex = 0;
        float summedAngle = 0;
        for (int i = 0; i < m_UpdatedList.Count; i++)
        {
            float distance = Vector3.Distance(m_TransformCOM, m_UpdatedList[i].Tform.position);
            m_UpdatedList[i].m_DistanceFromCOM = distance;

            m_UpdatedList[i].m_AngleTowardCOM = Vector3.Angle(m_UpdatedList[i].Velocity, m_TransformCOM);
            summedAngle += m_UpdatedList[i].m_AngleTowardCOM;
            if( distance > furthestDistance )
            {
                furthestDistance = distance;
                furthestIndex = i;
            }
        }

        m_Reach.RawValue = furthestDistance;

        // Expansion
        // Average direction toward center of mass. Compare to bounds volume vel
        m_Expansion.RawValue = summedAngle;

        // Density
        // The amount of movement per volume
        m_Density.RawValue = m_TransformAvVel.magnitude / m_Bounds_All.Volume;

        // Coherence
        // Not quite sure yet

        // Travel
        // The speed of the center of mass
        m_TravelIntensity.RawValue = m_TransformCOMVel.magnitude;

        // Energy
        // The speed of all of the joints together
        m_Energy.RawValue = m_TransformAvVel.magnitude;

        // Speed
        // Not sure how this is mean to be different to the energy?? Discuss
        m_Speed.RawValue = m_TransformAvVel.magnitude;




        // Center of mass of energy

        /*     
        m_Rhythm = new Ranged_Float("Rhythm", m_RhythmGUI);
        m_Harmony = new Ranged_Float("Harmony", m_HarmonyGUI);
        m_Energy = new Ranged_Float("Energy", m_EnergyGUI);
         * */

       
	}

    void FixedUpdate()
    {
        if (m_OutputOSC) OutputOSC();
    }

    public void SetTrailTimeSlider( float time )
    {
        m_TrailTime = time;
        SetTrailTime( m_TrailTime );
    }

    void SetTrailTime( float trailTime )
    {
        foreach (ET_Transform_Analysis tform in m_AnalysedTransforms)
        {
            TrailRenderer renderer = tform.GetComponent<TrailRenderer>();
            if( renderer != null )
            {
                renderer.time = trailTime;
            }
        }
    }

    void SetupGUI()
    {
        GUI_SliderButton btn = Instantiate(m_SliderButton) as GUI_SliderButton;
        btn.transform.ParentAndZero(m_GUIPanel);
    }

    void OutputOSC()
    {
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/boundsVel", m_BoundsVel);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/transformAvVel", m_TransformAvVel);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/boundsCOM", m_Bounds_All.Center );
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/transformCOM", m_TransformCOM);

        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/spaceIntensity", m_SpaceIntensity.m_NormalizedValue);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/speed", m_Speed.m_NormalizedValue);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/smoothness", m_Smoothness.m_NormalizedValue);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/reach", m_Reach.m_NormalizedValue);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/density", m_Density.m_NormalizedValue);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/coherence", m_Coherence.m_NormalizedValue);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/travelIntensity", m_TravelIntensity.m_NormalizedValue);
        m_OSCHandler.SendOSCMessage(m_OSCPrefix + "/energy", m_Energy.m_NormalizedValue);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            m_Bounds_All.DrawGizmo(true);

            Gizmos.color = Color.yellow;
            m_Bounds_Lower.DrawGizmo(false);
            m_Bounds_Upper.DrawGizmo(false);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_TransformCOM, .2f);
            Gizmos.DrawLine(m_TransformCOM, m_TransformCOM + m_TransformAvVel);
           // Gizmos.color = Color.cyan;
           // Gizmos.DrawLine(m_TransformCOM, m_TransformCOM + m_TransformCOMVel);

            Gizmos.color = Color.blue;
            m_LowerRotPosFrom.y = 3;
            m_LowerRotationVec.y = 3;
           

           // Gizmos.DrawLine(m_FurthestUpper00.Tform.position + (Vector3.up * 2), m_FurthestUpper01.Tform.position + (Vector3.up * 2));
           // Gizmos.DrawLine(m_FurthestLower00.Tform.position + (Vector3.up * 2), m_FurthestLower01.Tform.position + (Vector3.up * 2));

        }
    }
}
