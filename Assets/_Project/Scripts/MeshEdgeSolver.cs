using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

/// <summary>
/// Mesh edge solver.
///  * Output tri cut list
/// </summary>
/// 

public class MeshEdgeSolver : MonoBehaviour
{
	public Transform m_MeshParent;
	Mesh m_Mesh;

	// Singleton instance. TODO: Try design this out later on
	static MeshEdgeSolver m_Instance { get; set; }
	public static MeshEdgeSolver Instance {	get { return m_Instance; 	}	}



	// The type of joint being used. TODO: Change to edge type instead
	public SLO_Join.JointType m_JointType = SLO_Join.JointType.Inner; 

	// List of edges
	List< SLO_Edge > m_Edges = new List<SLO_Edge>();
	List< SLO_Join > m_Joiners = new List<SLO_Join>();
	List< SLO_Face > m_Faces = new List<SLO_Face>();

	public int JointCount{ get { return m_Joiners.Count; } }
	public int EdgeCount{ get { return m_Joiners.Count; } }
	public int FaceCount{ get { return m_Joiners.Count; } }
	
	// The thickness of the face parts TODO: make tab thickness be determined by the material thickness
	float 	m_FaceThickness = 3;	
	public float FaceThickness
	{
		get {	return m_FaceThickness;		}
		set
		{
			m_FaceThickness = Mathf.Clamp( value, .1f, 30 );
			// Update all faces when value set
			for (int i = 0; i < m_Faces.Count; i++)
			{
				m_Faces[i].Thickness = m_FaceThickness;
			}
		}
	}    

	public float m_EdgeDiameter = 7f;
	public float EdgeDiameter
	{
		set
		{
			m_EdgeDiameter = Mathf.Clamp( value, 3, 50 );
		}
		get
		{
			return m_EdgeDiameter; 
		}
	}


	public float m_JoinerLength = 40f;
	public float JoinerLength
	{
		set
		{
			m_JoinerLength = Mathf.Clamp( value, 5, 200 );
		}
		get
		{
			return m_JoinerLength;
		}
	}

	// 1.55 mm in min shapeways feature thickness
	float 	m_JointWallThickness = 1.55f;

	public float WallThickness
	{
		set
		{
			m_JointWallThickness = Mathf.Clamp( value, 1.55f, 10 );
		}
		get
		{
			return m_JointWallThickness;
		}
	}

	public bool m_ShowFaces = true;


	// Total length of all edges
	float m_TotalEdgeLength;
	public float TotalEdgeLength {	get {	return m_TotalEdgeLength;	}}

	// Total area of all faces
	float m_TotalArea;
	public float TotalArea {		get {	return m_TotalArea;		}}

	// Materials
	public Material m_MatJoiner;
	public Material m_MatEdge;
	public Material m_MatFace;
	public Material m_FlatFaceMat;


    // Moved to SLO object exporter
	// File output
    public bool m_WriteFile = false;
    public string  fileName = "MyFile.txt";

    // Move to manager - selection and interaction
	public GameObject m_SelectedObject;
	ComponentType m_SelectedComponentType = ComponentType.None;
	enum ComponentType
	{
		None,
		Join,
		Edge,
		Face,
	}

    // Moved to manager
    SLO_Join m_SelectedJoin;
    public SLO_Join SelectedJoin
    {
        get { return m_SelectedJoin; }
        set 
        {
            m_SelectedJoin = value;
            m_GUI.SetSelectedJoint(m_SelectedJoin);
        }
    }

	Transform SLOMeshParent;

    // Moved to manager
    public SLO_GUI m_GUI;

	void Start () 
    {
		m_Instance = this;

		m_EdgeDiameter = 	PlayerPrefs.GetFloat ("m_EdgeDiameter", 9.5f);
		m_JoinerLength = 	PlayerPrefs.GetFloat ("m_JoinerLength", 20);
		WallThickness = 	PlayerPrefs.GetFloat ("WallThickness", 2 );

	
		m_GUI.UpdateText ();

		m_Mesh = m_MeshParent.GetComponent< MeshFilter > ().mesh;

        // Create array of edges
        for (int i = 0; i < m_Mesh.triangles.Length/3; i++)
        {
			SLO_Edge e0 = new GameObject( "Edge new" + ((i*3) + 0) ).AddComponent( typeof( SLO_Edge ) ) as SLO_Edge;
			e0.transform.SetParent( transform );
			e0.Init( m_Mesh.vertices[ m_Mesh.triangles[(i * 3)] ], m_Mesh.vertices[  m_Mesh.triangles[(i * 3) + 1]] );

			SLO_Edge e1 = new GameObject( "Edge new"+ ((i*3) + 1) ).AddComponent( typeof( SLO_Edge ) )as SLO_Edge;
			e1.transform.SetParent( transform );
			e1.Init( m_Mesh.vertices[m_Mesh.triangles[(i * 3) + 1]],   m_Mesh.vertices[ m_Mesh.triangles[(i * 3) + 2]]);

			SLO_Edge e2 = new GameObject( "Edge new"+ ((i*3) + 2)).AddComponent( typeof( SLO_Edge ) )as SLO_Edge;
			e2.transform.SetParent( transform );
			e2.Init( m_Mesh.vertices[m_Mesh.triangles[(i * 3) + 2]],    m_Mesh.vertices[m_Mesh.triangles[(i * 3)]]);

            m_Edges.Add(e0);
            m_Edges.Add(e1);
            m_Edges.Add(e2);

			m_TotalArea += MathExtensions.AreaOfTriangle( m_Mesh.vertices[m_Mesh.triangles[(i * 3)]] , m_Mesh.vertices[ m_Mesh.triangles[(i * 3) + 1 ]  ] ,m_Mesh.vertices[ m_Mesh.triangles[(i * 3) + 2 ] ] );
        }


        // Check if any edges are the same and remove copies
        // If edges share a vertex add them to the appropriate connection list
		List< SLO_Edge > edgesToRemove = new List< SLO_Edge >();
        for (int i = 0; i < m_Edges.Count; i++)
        {
			SLO_Edge baseEdge = m_Edges[i];

            if (edgesToRemove.Contains(baseEdge))
                continue;

            for (int j = 0; j < m_Edges.Count; j++)
            {
                if (j == i)
                    continue;

				SLO_Edge compareEdge = m_Edges[j];

                if (edgesToRemove.Contains(compareEdge))
                    continue;

                // If not same length then continue
                if (baseEdge.Length != compareEdge.Length)
                    continue;
				else if ( baseEdge.m_VertexPos0 == compareEdge.m_VertexPos0 && baseEdge.m_VertexPos1 == compareEdge.m_VertexPos1 )
                {                    
                    edgesToRemove.Add(baseEdge);
                }
				else if (baseEdge.m_VertexPos0 == compareEdge.m_VertexPos1 && baseEdge.m_VertexPos1 == compareEdge.m_VertexPos0 )
                {
                    edgesToRemove.Add(baseEdge);
                }
            }
        }

		print ("Removing edge count  " + edgesToRemove.Count);

		// Remove duplicate edges
		foreach (SLO_Edge e in edgesToRemove)
		{
			if( e != null )
			{
				m_Edges.Remove (e);
				DestroyImmediate( e.gameObject );
			}
		}

		print ("Edge left  " + m_Edges.Count);

		// Index the remaining edges
        for (int i = 0; i < m_Edges.Count; i++)
        {
            m_Edges[i].m_Index = i;
			m_Edges[i].name = "Edge " + i;
        }

		List<Vector3> uniqueVertPositions = new List<Vector3>();
		// Find all unique verts
		for (int i = 0; i < m_Mesh.vertices.Length; i++) 
		{
			if( !uniqueVertPositions.Contains( m_Mesh.vertices[i] ) )
				uniqueVertPositions.Add(  m_Mesh.vertices[i] );
		}

		print (uniqueVertPositions.Count + "   Unique verts ");

		// Create the SLO Mesh parent transform
		SLOMeshParent = new GameObject ( name + " edge mesh").transform;
		// Set to the transform of the mesh
		SLOMeshParent.transform.position = transform.position;


		// Create all joins
		for (int i = 0; i < uniqueVertPositions.Count; i++) 
		{
			CreateJoiner(m_MeshParent.TransformPoint( uniqueVertPositions[i] ), SLOMeshParent );
		}

		print (" Joiners created  " + m_Joiners.Count);

		// Set edge joiners
		for (int i = 0; i < m_Edges.Count; i++) 
		{
			for (int j = 0; j < m_Joiners.Count; j++)
			{
				if(m_MeshParent.TransformPoint( m_Edges[i].m_VertexPos0 ) == m_Joiners[j].transform.position )
				{
					m_Edges[i].m_Join0 = m_Joiners[j];
				}

				if(m_MeshParent.TransformPoint( m_Edges[i].m_VertexPos1 ) == m_Joiners[j].transform.position )
				{
					m_Edges[i].m_Join1 = m_Joiners[j];
				}
			}
		}

		// Set edge joiners
		for (int i = 0; i < m_Edges.Count; i++)
		{
			m_Edges[i].m_Join0.AddEdge( m_Edges[i], m_JoinerLength );
			m_Edges[i].m_Join1.AddEdge( m_Edges[i], m_JoinerLength );
		}

		// Initalize joins
		for (int i = 0; i < m_Edges.Count; i++) 
		{
			m_Edges[i].transform.position = m_Edges [i].m_Join0.transform.position;
			m_Edges[i].transform.LookAt( m_Edges [i].m_Join1.transform.position );
			m_Edges[i].transform.SetParent( SLOMeshParent );
			m_Edges[i].AddEdgeMesh( );
			m_Edges[i].SetMaterial( m_MatEdge );
		}


       // MakeMeshFromCylinders();

		CreateFaces();
		CreateTabs ();

		CalculateTotalEdgeLength ();

		LayFacesFlat ();

        print("Edges remaining " + m_Edges.Count );

		m_MeshParent.gameObject.SetActive (false);

        // Set selected joint to first 
        SelectedJoin = m_Joiners[0];


	}

    // Moved to manager
    public void SetSelectedObject( GameObject go )
	{
		m_SelectedObject = go;

		if (m_SelectedObject.GetComponent< ProceduralMesh_Tube > () != null)
		{
			if (m_SelectedObject.transform.parent.GetComponent< SLO_Edge > () != null)
			{
				m_SelectedComponentType = ComponentType.Edge;
				m_SelectedObject = m_SelectedObject.transform.parent.gameObject;
			}
			else if (m_SelectedObject.transform.parent.parent.GetComponent< SLO_Join > () != null)
			{
				m_SelectedComponentType = ComponentType.Join;
				m_SelectedObject = m_SelectedObject.transform.parent.parent.gameObject;
                SelectedJoin = m_SelectedObject.transform.parent.parent.GetComponent<SLO_Join>();
               
			}
		}
		else if (m_SelectedObject.GetComponent< SLO_Face > () != null)	m_SelectedComponentType = ComponentType.Face;
		else if (m_SelectedObject.GetComponent< SLO_Join > () != null)	m_SelectedComponentType = ComponentType.Join;
		else 
		{
			m_SelectedComponentType = ComponentType.None;
		}

		print ( "Selected: " + m_SelectedObject.name );

	}

    // Moved to manager
    void OnApplicationQuit()
	{
		PlayerPrefs.SetFloat ("m_EdgeDiameter", m_EdgeDiameter);
		PlayerPrefs.SetFloat ("m_JoinerLength", m_JoinerLength);
		PlayerPrefs.SetFloat ("WallThickness", WallThickness);
	}

    // moved to object
	void CalculateTotalEdgeLength()
	{
		// Set total edge length to 0
		m_TotalEdgeLength = 0;

		for (int i = 0; i < m_Edges.Count; i++) 
			m_TotalEdgeLength += m_Edges [i].Length;
	}
		

	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < m_Edges.Count; i++) 
		{
			m_Edges[i].Diameter = m_EdgeDiameter;
		}
		for (int i = 0; i < m_Joiners.Count; i++) 
		{
			m_Joiners[i].UpdateJoin( m_JointType, m_EdgeDiameter, m_JointWallThickness, m_JoinerLength );
		}


		// Update show faces
		for (int i = 0; i < m_Faces.Count; i++)
		{
			m_Faces[i].Draw = m_ShowFaces;
		}

		// Testing
		if (Input.GetKeyDown (KeyCode.Delete)) 
		{
			if( m_SelectedComponentType == ComponentType.Face )
			{
				SLO_Face face = m_SelectedObject.GetComponent< SLO_Face >();

				m_Faces.Remove( face );

				// TODO: Remove tabs?

				Destroy( face.gameObject );

				m_GUI.m_SelectedElement = null;
			}
			else if( m_SelectedComponentType == ComponentType.Edge )
			{
				SLO_Edge edge = m_SelectedObject.GetComponent< SLO_Edge >();

				// Remove form list
				m_Edges.Remove( edge );

				edge.Delete();

				// set gui back to null
				m_GUI.m_SelectedElement = null;
			}
		}
	}

	public TextMesh m_TextMesh;
	// Uber hax, fix with maths later
	void LayFacesFlat()
	{
		GameObject parent = new GameObject (m_MeshParent.name + " Flat Faces ");
		parent.transform.position = Vector3.right * 5;
		TriMesh[] flatFaces = new TriMesh[m_Faces.Count];
		float xPos = 7;
		float yPos = 0;
		float prevTriDist = 0;
		float largestTri = 0;

        // For all faces
		for (int i = 0; i < m_Faces.Count; i++) 
		{
            // Create a new trimesh
			TriMesh newTriMesh = new GameObject( m_MeshParent.name + " - Flat face " + i ).AddComponent< TriMesh >();
			newTriMesh.gameObject.layer = 8;
			Vector3 midPoint = (m_Faces[i].Join0.transform.position +  m_Faces[i].Join1.transform.position +  m_Faces[i].Join2.transform.position) / 3f;
		
			newTriMesh.CreateTri(
				newTriMesh.transform.TransformPoint( m_Faces[i].Join0.transform.position - midPoint ),
				newTriMesh.transform.TransformPoint( m_Faces[i].Join1.transform.position - midPoint ),
                newTriMesh.transform.TransformPoint( m_Faces[i].Join2.transform.position - midPoint ),
				TriMesh.Extrusion.None );
		
            // Apply material
			newTriMesh.GetComponent< MeshRenderer >().material = m_FlatFaceMat;

            #region Spacing and placement
            // Get max vertex distance on x to make sure next tri is spaced accordingly
            float maxVertDistance = 0;
			for (int j = 0; j < 3; j++) 
			{
				float dist = Vector3.Distance( newTriMesh.transform.position, newTriMesh.transform.TransformPoint( newTriMesh.BaseVerts[j] ) );
				if( dist > maxVertDistance )
					maxVertDistance = dist;
		   	}	
			xPos += maxVertDistance + prevTriDist ;

			prevTriDist = maxVertDistance;
				   
			newTriMesh.transform.rotation = Quaternion.FromToRotation( m_Faces[i].Normal, -Vector3.forward );

			if( maxVertDistance > largestTri )
				largestTri = maxVertDistance;

			if( i == m_Faces.Count / 3 )
			{
				xPos = 7;
				yPos = largestTri * 2f;
			}
			else if( i == (int)(m_Faces.Count / 3) * 2 )
			{
				xPos = 7;
				yPos = largestTri * 4f;
			}

			Vector3 offset = new Vector3(  xPos, yPos, -1 );

			newTriMesh.transform.position = offset;
			newTriMesh.transform.parent = parent.transform;
            #endregion
            
            TextMesh edgeLength0 = Instantiate( m_TextMesh );
			edgeLength0.name = "Edge text 01";
			edgeLength0.gameObject.layer = 8;
			edgeLength0.transform.SetParent( newTriMesh.transform );
			edgeLength0.transform.position = newTriMesh.Edge01MidPoint;
            edgeLength0.transform.localScale = Vector3.one * .02f;
            edgeLength0.text = m_Faces[i].Edge2Length.ToString("##");
			
			TextMesh edgeLength1 = Instantiate( m_TextMesh );
			edgeLength1.name = "Edge text 12";
			edgeLength1.gameObject.layer = 8;
			edgeLength1.transform.SetParent( newTriMesh.transform );
			edgeLength1.transform.position = newTriMesh.Edge12MidPoint;
            edgeLength1.transform.localScale = Vector3.one * .02f;
            edgeLength1.text = m_Faces[i].Edge1Length.ToString("##");
			
			TextMesh edgeLength2 = Instantiate( m_TextMesh );
			edgeLength2.name = "Edge text 20";
			edgeLength2.gameObject.layer = 8;
			edgeLength2.transform.SetParent( newTriMesh.transform );
			edgeLength2.transform.position= newTriMesh.Edge20MidPoint;
            edgeLength2.transform.localScale = Vector3.one * .02f;
            edgeLength2.text = m_Faces[i].Edge0Length.ToString("##");
		
			

			flatFaces[i] = newTriMesh;

			//newTriMesh.transform.LookAt( Vector3.up );
			//SLO_Face newFace = Instantiate( m_Faces[i] );
			//newFace.transform.position = 
		}

		//CombineMeshes (parent);
	}

	SLO_Join[] m_JoinerCopies;
	GameObject copyJoinersParent;




	void CreateJoiner( Vector3 pos, Transform meshParent )
	{
		SLO_Join newJoin = new GameObject("Join" + m_Joiners.Count ).AddComponent< SLO_Join >() as SLO_Join;
		newJoin.transform.SetParent( meshParent );
		newJoin.Init( m_Joiners.Count, pos, 10, m_MatJoiner );

		m_Joiners.Add (newJoin);
	}

	void InnerJoint()
	{
		m_JointType = SLO_Join.JointType.Inner;

		for (int i = 0; i < m_Tabs.Count; i++)
		{
			m_Tabs[i].gameObject.SetActive( false );
		}

		print ("here");
	}

	void OuterJoint()
	{
		m_JointType = SLO_Join.JointType.Outer;

		for (int i = 0; i < m_Tabs.Count; i++)
		{
			m_Tabs[i].gameObject.SetActive( true );
		}
	}

    public void SetJointType( int jointTypeIndex )
    {
        if (jointTypeIndex == 0) InnerJoint();
        else OuterJoint();
    }




	void CreateFaces()
	{
		for (int i = 0; i < m_Mesh.triangles.Length/3f; i++)
		{
			int triIndex = (i * 3);

			Vector3 v0 =m_MeshParent.TransformPoint (m_Mesh.vertices [m_Mesh.triangles [triIndex]]);
			Vector3 v1 =m_MeshParent.TransformPoint (m_Mesh.vertices [m_Mesh.triangles [triIndex + 1]]);
			Vector3 v2 =m_MeshParent.TransformPoint (m_Mesh.vertices [m_Mesh.triangles [triIndex + 2]]);

			SLO_Join j0 = FindJoinFromPos( v0 );
			SLO_Join j1 = FindJoinFromPos( v1 );
			SLO_Join j2 = FindJoinFromPos( v2 );

			SLO_Face newFace = new GameObject().AddComponent< SLO_Face >() as SLO_Face;
			newFace.Init( j0, j1, j2, m_MatFace, m_FaceThickness, i, m_FaceExtrusion );
			newFace.transform.SetParent( SLOMeshParent );

			m_Faces.Add( newFace );
		}
	}

	// Hax - OMG SLOOOOW , fix later
	SLO_Join FindJoinFromPos( Vector3 pos )
	{
		// parent to joiner
		for (int i = 0; i < m_Joiners.Count; i++) 
		{
			if( ( pos - m_Joiners[i].transform.position).magnitude < ( m_JoinerLength / 1000f)  )
			{
				return m_Joiners[i];
			}
		}

		return null;
	}

	List< SLO_Join_Tab > m_Tabs = new List<SLO_Join_Tab>();
	
	void CreateTabs()
	{
		for (int i = 0; i < m_Faces.Count; i++)
		{
			int triIndex = (i*3);
			// Tri Mid point
			
			Vector3 v1 =m_MeshParent.TransformPoint(	m_Mesh.vertices[ m_Mesh.triangles[ triIndex ]] );
			Vector3 v2 =m_MeshParent.TransformPoint(	m_Mesh.vertices[ m_Mesh.triangles[ triIndex + 1 ]] );
			Vector3 v3 =m_MeshParent.TransformPoint(	m_Mesh.vertices[ m_Mesh.triangles[ triIndex + 2 ]] );
			
			float radius = ( 10 * .001f * .5f) - .0015f;

			// Tri 1
			NewTab( m_Faces[i].Join0,  m_Faces[i].Join1, m_Faces[i].Join2 );
			NewTab( m_Faces[i].Join1,  m_Faces[i].Join2, m_Faces[i].Join0 );
			NewTab( m_Faces[i].Join2,  m_Faces[i].Join0, m_Faces[i].Join1 );
		}
	}
	
	public TriMesh.Extrusion m_FaceExtrusion = TriMesh.Extrusion.Center;

	public bool m_MakeOuterTabs = true;
	public bool m_MakeInnerTabs = false;

	void NewTab( SLO_Join j0, SLO_Join j1, SLO_Join j2 )
	{		
		SLO_Join_Tab newTab = new GameObject().AddComponent< SLO_Join_Tab >() as SLO_Join_Tab;
		newTab.transform.position = j0.transform.position;
		newTab.Init (j0, j1, j2, m_MatJoiner, m_MakeOuterTabs, m_MakeInnerTabs );
		m_Tabs.Add (newTab);
	}

	public string m_SavePath;
	public string m_ProjectName = "Project 01";
	public void OutputAssets()
	{

		// BAKE
		//TODO: Create copy, bake, output, delete. this way you maintain a working copy
		
		copyJoinersParent = new GameObject ("Copy joiners");
		copyJoinersParent.transform.position = m_MeshParent.position;
		m_JoinerCopies = new SLO_Join[ m_Joiners.Count ];
		
		// parent all tabs
		for (int j = 0; j < m_Tabs.Count; j++)
		{
			m_Tabs[j].ParentToJoin( true );
		}
		
		for (int i = 0; i < m_JoinerCopies.Length; i++) 
		{
			SLO_Join copyJoin = Instantiate( m_Joiners[i] );
			copyJoin.enabled = false;
			copyJoin.name = m_Joiners[i].name;
			
			copyJoin.transform.parent = copyJoinersParent.transform;
			
			m_JoinerCopies[i] = copyJoin;
		}
		
		// Combine the joint meshes
		for (int j = 0; j < m_Joiners.Count; j++) 
		{
			
			CombineMeshes ( m_JoinerCopies[j].gameObject );
		}
		
		// parent all tabs
		for (int j = 0; j < m_Tabs.Count; j++)
		{
			m_Tabs[j].ParentToJoin( false );
		}


		// Create folder
		System.IO.Directory.CreateDirectory ( m_SavePath + "/" + m_ProjectName );

		for (int i = 0; i < m_JoinerCopies.Length; i++) 	
		{
			SLO_Join join = m_JoinerCopies[i];
			Mesh joinerMesh = join.GetComponent< MeshFilter >().mesh;
			joinerMesh = MeshExtensions.ScaleVerts( joinerMesh , 1000 );
			//STL.ExportBinary( join.GetComponent< MeshFilter >(), Application.dataPath + join.name + ".STL" );
			STL.ExportBinary( join.GetComponent< MeshFilter >(), m_SavePath + "/" + m_ProjectName + "/" + m_ProjectName + " - Join " + i + " -  D " + m_EdgeDiameter + "  L " + m_JoinerLength + "  T " + m_JointWallThickness + ".STL" );

			join.GetComponent< MeshFilter >().mesh = MeshExtensions.ScaleVerts( joinerMesh , -1000 );
		}

		print ("Output " + m_JoinerCopies.Length + " STLs" );

		// Export STLs

		Destroy (copyJoinersParent);

		WriteFile();
	}



	GameObject CreateJoinerSleeves( Transform meshParent, Vector3 pos, Vector3 lookAtPos, float radius, float joinLength, GameObject mesh )
	{
		GameObject newJoinerSleeve = Instantiate( mesh );
		newJoinerSleeve.transform.SetParent( meshParent );
		newJoinerSleeve.transform.position = pos;
		newJoinerSleeve.transform.LookAt( lookAtPos );
		newJoinerSleeve.transform.localScale = new Vector3 ( radius, radius, joinLength );

		return newJoinerSleeve;
	}



	void CombineMeshes( GameObject go ) 
	{
		MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];

		int i = 0;
		while (i < meshFilters.Length)
		{
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			i++;
		}

		go.transform.DestroyAllChildren ();

		print ("Combining: " + meshFilters.Length);

		MeshFilter meshFilter = go.AddComponent< MeshFilter > ();
		
		go.AddComponent< MeshRenderer > ().material = meshFilters[0].GetComponent< MeshRenderer >().material;
		go.GetComponent< MeshRenderer > ().material.SetCol (Color.gray);
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.CombineMeshes(combine);

		Vector3[] newVerts = new Vector3[meshFilter.mesh.vertices.Length];

		for (int v = 0; v < meshFilter.mesh.vertices.Length; v++)
		{
			newVerts[v] = meshFilter.mesh.vertices[v];
			newVerts[v] -= go.transform.position;
			//newVerts[v].y -= go.transform.meshParent.transform.position.y;
		}
		meshFilter.mesh.vertices = newVerts;
		meshFilter.mesh.RecalculateBounds ();
		//meshFilter.mesh.RecalculateNormals ();

		go.transform.gameObject.active = true;
		go.AddComponent< MeshCollider > ();

		//go.transform.position = Vector3.zero;
		go.tag = "Selectable";
	}

    void CalculateTotalArea()
    {
        m_TotalArea = 0;
        for (int i = 0; i < m_Faces.Count; i++)
        {
            m_TotalArea += m_Faces[i].Area;
        }
    }

    void WriteFile()
    {

		List< SLO_Edge > sortedEdges = new List< SLO_Edge >();
		sortedEdges = m_Edges.OrderBy (x => x.Length).ToList ();  

		var sr = File.CreateText( m_SavePath + "/" + m_ProjectName + "/" + m_ProjectName + ".txt" );

        CalculateTotalArea();
        ///
        //TODO: group together same edge lengths into multiples
       
        sr.WriteLine( "CUT LIST \n" + fileName );

        sr.WriteLine("Total area: " + m_TotalArea);

        sr.WriteLine( "" );
		sr.WriteLine( "" );
		sr.WriteLine( "Edges:   Round bar: {0}mm \t  Total length: {1} ", m_EdgeDiameter.ToString("##"), m_TotalEdgeLength * 1000f  );
		sr.WriteLine( "" );
		sr.WriteLine( "Joint length {0}mm", m_JoinerLength.ToString("##") );
		sr.WriteLine( "Length (mm) \t\t Edge number" );
		for (int i = 0; i < sortedEdges.Count; i++) 
		{
			sr.WriteLine( "\t{0} \t\t {1}",(sortedEdges[i].Length * 1000).ToString("##"), sortedEdges[i].m_Index );
		}

		sr.WriteLine( "\n\n" );

        for (int i = 0; i < m_Edges.Count; i++)
        {
            sr.WriteLine( "Edge {0}", i );
            sr.WriteLine("\t Length: {0}  \t\t Vertex 0: {1}   \t\t Vertex 1: {2}", m_Edges[i].Length, m_Edges[i].m_Join0.transform.position, m_Edges[i].m_Join1.transform.position);

            sr.WriteLine( "\t\t Vertex 0 connects too: " );
			foreach( SLO_Edge edge in m_Edges[i].EdgesConnectedToVert0 )
                sr.WriteLine("\t\t\t Edge {0}", edge.m_Index );

            sr.WriteLine("\t\t  Vertex 1 connects too: ");
			foreach ( SLO_Edge edge in m_Edges[i].EdgesConnectedToVert1)
                sr.WriteLine("\t\t\t Edge {0}", edge.m_Index);

            sr.WriteLine("");
        }

		sr.WriteLine( "TRIS" );
		for (int i = 0; i < m_Faces.Count; i++)
		{
			//length
			//angle to next length
			sr.WriteLine( "Edge {0}", i );
			sr.WriteLine("\t Length: {0}  \t\t Vertex 0: {1}   \t\t Vertex 1: {2}", m_Edges[i].Length, m_Edges[i].m_Join0.transform.position, m_Edges[i].m_Join1.transform.position);
			
			sr.WriteLine( "\t\t Vertex 0 connects too: " );
			foreach( SLO_Edge edge in m_Edges[i].EdgesConnectedToVert0 )
				sr.WriteLine("\t\t\t Edge {0}", edge.m_Index );
			
			sr.WriteLine("\t\t  Vertex 1 connects too: ");
			foreach ( SLO_Edge edge in m_Edges[i].EdgesConnectedToVert1)
				sr.WriteLine("\t\t\t Edge {0}", edge.m_Index);
			
			sr.WriteLine("");
		}

        sr.Close();
    }
}
