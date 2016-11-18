using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ET_Transform_Analysis : MonoBehaviour
{
    // Reference to the transform
    Transform           m_Tform;
    public Transform    Tform { get { return m_Tform; } }

    // Bounds reference
    Bounds3D        m_Bounds;

    // Current velocity   
	public Vector3	m_Velocity;  
    public Vector3  Velocity { get { return m_Velocity; } }
    public Vector3  Direction { get { return m_Velocity.normalized; } }
    public Vector3  m_Acceleration;

    // Velocity smoothign variables. Included to stop you getting crazy spikes from incorrect data
    public Vector3  m_SmoothedVelocity;   
    public float    m_VelocitySmoothing = 12;
    public bool m_Destroying = false;

    // Smoothed speed
    private float   m_Speed;
    public float    SmoothedSpeed    {        get { return m_SmoothedVelocity.magnitude; }    }

    
	
    // The range in which you want to normalized the magnitude to
	public float    m_SpeedNormalizationRange = 30;
    public float    NormalizedSpeed { get { return m_Velocity.magnitude.ScaleTo01(0, m_SpeedNormalizationRange); } }
    
	Vector3         m_CurrentPos;
	Vector3         m_PrevPos;

    // Testing
    public float m_DistanceFromCOM;
    public float m_AngleTowardCOM;

    public bool     m_FaceDirection = false;
    public float    m_RotationalVelocity;
    public float    NormalizedRotationalVelocity { get { return m_RotationalVelocity / 360f; } }

    bool m_OnGround = false;

    Renderer rend;

    public EventHandlers.VectorHandler OnGroundCollission;

    public bool     m_DrawGizmos = false;

    public bool m_UpdatedThisFrame = false;

    public float Smoothing = 8;

	void Start()
	{
        // Get a reference to the transform
		m_Tform = transform;

        Renderer rend = gameObject.GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update ()
	{
        // Get the current position
		m_CurrentPos = m_Tform.position;

        if (m_CurrentPos != m_PrevPos)
            m_UpdatedThisFrame = true;
        else
            m_UpdatedThisFrame = false;

        Vector3 prevVel = m_Velocity;
        
        // Update the velocity
		m_Velocity = ( m_CurrentPos - m_PrevPos ) / Time.deltaTime;

       // m_Acceleration = 

        // Calculate the smoothed velocity   ** not sure if from previous smoothed velocity or from prev velocity
        m_SmoothedVelocity = Vector3.Lerp(m_SmoothedVelocity, m_Velocity, Time.deltaTime * m_VelocitySmoothing);

        m_RotationalVelocity = Vector3.Angle(prevVel, m_Velocity) / Time.deltaTime;


        //Test
        if ( m_FaceDirection )
        {
            m_Tform.LookAt(m_Tform.position + Direction);

           // m_AngleFromUp = Vector3.Angle(Vector3.up, m_Tform.rotation.eulerAngles);
           // m_AngleFromUp = Quaternion.Angle(Quaternion.Euler(Vector3.up), m_Tform.rotation);
        }


        if ( !m_OnGround && m_Tform.position.y < .1f)
        {
            m_OnGround = true;
            if (OnGroundCollission != null )
                OnGroundCollission( m_Tform.position );
        }
        else if (m_OnGround && m_Tform.position.y > .1f)
        {
            m_OnGround = false;
            // In air event
        }

        if (m_OnGround)
        {
            if (rend != null)
                rend.material.SetCol(Color.red);
        }
        else
        {
            if (rend != null )
                rend.material.SetCol(Color.yellow);
        }

       
            
        		
		m_PrevPos = m_Tform.position;
	}


    // Build rigid connecctions
    ET_Transform_Analysis m_ConnectedTransform;
   // List
    float m_RigidDistance;
    float m_Variation;
    // Find rigid body
    public void FindRigidConnection( ET_Transform_Analysis[] transforms )
    {
        for (int i = 0; i < transforms.Length; i++)
        {
            float dist = Vector3.Distance(Tform.position, transforms[i].Tform.position);

        }
    }

    public void SetColour( Color col )
    {
        
        if (rend != null)
            rend.material.SetCol(Color.red);
    }

	void OnDrawGizmos()
	{
        if ( Application.isPlaying && m_DrawGizmos )
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(m_Tform.position, m_Tform.position + m_Velocity);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(m_Tform.position, m_Tform.position + m_SmoothedVelocity);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(m_Tform.position + (Vector3.right * .5f), m_Tform.position + (Vector3.right * .7f));
            Gizmos.DrawLine(m_Tform.position + (Vector3.right * .5f) + (Vector3.up * 1), m_Tform.position + (Vector3.right * .7f) + (Vector3.up * 1));
            Gizmos.DrawLine(m_Tform.position + (Vector3.right * .6f), m_Tform.position + (Vector3.right * .6f) + (Vector3.up * NormalizedSpeed));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(m_Tform.position + (Vector3.right * .7f), m_Tform.position + (Vector3.right * .9f));
            Gizmos.DrawLine(m_Tform.position + (Vector3.right * .7f) + (Vector3.up * 1), m_Tform.position + (Vector3.right * .9f) + (Vector3.up * 1));
            Gizmos.DrawLine(m_Tform.position + (Vector3.right * .8f), m_Tform.position + (Vector3.right * .8f) + (Vector3.up * NormalizedRotationalVelocity));
        }
	}
}
