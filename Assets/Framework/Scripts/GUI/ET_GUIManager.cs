using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ET_ GUI manager.
///  - Handles the GUI skin
///  - Determines which windows draw to screen
///  - Controls mouse drawing to the screen
///  - Handles the offset of the GUI for panning/scrolling through GUI
/// </summary>


public class ET_GUIManager : MonoBehaviour
{
	static ET_GUIManager 			m_Instance { get; set; }	
	public static ET_GUIManager Instance												// Singleton dynamically creates
    {        
        get 
		{	
            /*
			GameObject managerParent = GameObject.Find( "ET_Project_Managers" );
			if( managerParent == null )
				managerParent = new GameObject( "ET_Project_Managers" );
			
			if( m_Instance == null )
			{
				m_Instance = new GameObject("GUI Manager").AddComponent< ET_GUIManager >();				
			}	
             * */
			
			//m_Instance.transform.parent = managerParent.transform;
			
			return m_Instance; 
		}
    }

	public GUISkin 					m_GUISkin;									// GUI skin	
	GUIStyle 						centeredStyle;
	public Vector2 					m_Offset = Vector2.zero;					// The offset of the GUI. Used for panning and scrolling
	public Vector2 					m_TempOffset;								// Temporary offset. Check?
	List< ET_GUIWindow > 			m_AllWindows = new List<ET_GUIWindow>();	// All registered windows
	public List< ET_GUIWindow > 	AllWindows { get{ return m_AllWindows; } }	// Public get accessor for all windows
	public bool 					m_WindowSnapping = false;					// Window snapping flag for snapping windows to one another
	public float 					m_SnappingThreshold = 10;					// Threshold for the window snapping

	bool 							m_SoloMode = false;							// Solo mode flag for displaying a single window for focussing attention
	public bool 					SoloMode{get{ return m_SoloMode; }}			// Solo mode public accessor
	ET_GUIWindow 					m_SoloWindow;								// Window that is selected for solo mode	
	public ET_GUIWindow 			SoloWindow{get{ return m_SoloWindow; }}  	// Public accessor for solo window

	public bool 					m_RadialMenuActive = false;					// Is the radial menu currently active?
	public Texture 					m_RadialBG;									// Radial menu background texture
	//public GUI_RadialButton 		m_ActiveRadialGUI;							// The active radial gui	

	public bool 					m_DrawGUI = true;							// Draw GUI flag	
	
	void Awake()
	{
		m_Instance = this;	
	}

	void Start()
	{
		m_GUISkin =  Resources.Load( "ET GUI Skin", typeof( GUISkin ) ) as GUISkin;
	}

	void Update()
	{
		if( Input.GetKey( KeyCode.LeftShift ) )
		{
			if( Input.GetKeyDown( KeyCode.G ) )
			{
				ToggleGUI();
			}
		}
		/*
		if( Input.GetMouseButtonDown( 2 ) )
		{
			m_TempOffset = m_Offset;
		}
		
		else if( Input.GetMouseButton( 2 ) && InputManager.Instance.Alt2Active )
		{
			m_Offset.x = m_TempOffset.x + ( InputManager.Instance.m_NormalizedRelativeMouseDownValue.x * Screen.width );
			m_Offset.y = m_TempOffset.y + ( -InputManager.Instance.m_NormalizedRelativeMouseDownValue.y * Screen.height );
		}
		
		
		if( InputManager.Instance.Alt2Active && Input.GetKeyDown( KeyCode.R) )
		{
			m_Offset = Vector2.zero;
		}

		if( Input.GetKey( KeyCode.LeftShift ) &&  Input.GetKeyDown( KeyCode.G )  )
		{			
			m_DrawGUI = !m_DrawGUI;
		}
		*/
	}
	
	void ToggleGUI()
	{
		m_DrawGUI = !m_DrawGUI;
		Cursor.visible = m_DrawGUI;
		
	}

	public void UseDefaultSkin()
	{
		GUI.skin = ET_GUIManager.Instance.m_GUISkin;
	}
	
	/*
	public void RegisterMenu( GUI_Menu menu )
	{
		m_Menus.Add( menu );
	}
	*/

	public void RegisterWindow( ET_GUIWindow window )
	{
		m_AllWindows.Add( window );
	}

	public void SoloToggle( ET_GUIWindow win )
	{
		if( m_SoloWindow == win && m_SoloMode )	// Incase solo is already active then it deactivates. So the solo 
			DisableSoloMode();
		else
		{
			m_SoloMode = true;
			m_SoloWindow = win;
		}
	}

	public void DisableSoloMode( )
	{
		m_SoloMode = false;
	}

	public void CheckWindowSnapping( ET_GUIWindow window )	// Window snapping
	{
		Vector2 closestSnapPos;

		foreach( ET_GUIWindow win in m_AllWindows )
		{
			//if( win.m_WindowGUI_Rect)
		}
	}
	
	/*
	public void AddToMenu( string menuName, GameObject go )
	{
		for( int i = 0; i < m_Menus.Count; i++ )
		{
			if( m_Menus[i].m_MenuName == menuName )
			{
				m_Menus[i].AddMenuItem( go );
			}
		}
	}
	*/

	float size = 240;
	float contentRadius = 80;
	float contentSize = 80;
	void OnGUI()
	{
		if( !m_DrawGUI ) return;
		ET_GUIManager.Instance.UseDefaultSkin();

		if( centeredStyle == null )
			centeredStyle = GUI.skin.GetStyle("Label");

		/*
		if( m_ActiveRadialGUI != null )
		{
			if( m_ActiveRadialGUI.m_RadialDown )
			{
				GUI.depth = -100;
				GUI.DrawTexture( new Rect( ET_InputManager.Instance.m_MouseDownPos.x - (size/2), ( Screen.height - ET_InputManager.Instance.m_MouseDownPos.y ) - (size/2), size, size ), m_RadialBG, ScaleMode.StretchToFill );

				float divisionAngle = 360 / m_ActiveRadialGUI.m_Options.Length; //Angle between menu items


				centeredStyle.alignment = TextAnchor.MiddleCenter;


				// Place items in correct spot
				for(int i = 0; i < m_ActiveRadialGUI.m_Options.Length; i++)
				{
					Vector2 localPos = Vector2.zero;
					float angle = 360 - ( (i * divisionAngle ) + 180 );

					localPos.x = Mathf.Sin( angle * Mathf.Deg2Rad ) * contentRadius;
					localPos.x += ET_InputManager.Instance.m_MouseDownPos.x - (contentSize/2);
					localPos.y = Mathf.Cos( angle * Mathf.Deg2Rad ) * contentRadius;
					localPos.y += ( Screen.height - ET_InputManager.Instance.m_MouseDownPos.y );

					if( i == m_ActiveRadialGUI.m_SelectedIndex )
						centeredStyle.fontSize = 16;
					else
						centeredStyle.fontSize = 10;

					GUI.Label( new Rect( localPos.x, localPos.y - (30/2), contentSize, 30 ), m_ActiveRadialGUI.m_Options[ i ], centeredStyle );
				}



				centeredStyle.fontSize = 12;
				//centeredStyle.alignment = TextAnchor.MiddleCenter;
				
				float m_NormalizedSelection = 	1 - (  ( Utils.SignedAngleBetweenVectors( -Vector3.up, ET_InputManager.Instance.m_NormalizedRelativeMouseDownValue.normalized  ) + 180 ) / 360 );
				m_NormalizedSelection += 		( 1f / (float)m_ActiveRadialGUI.m_Options.Length ) / 2;
				m_NormalizedSelection = 		m_NormalizedSelection.WrapFloatToRange( 0, 1 );



				//print(  Utils.SignedAngleBetweenVectors( -Vector3.up, InputManager.Instance.m_NormalizedRelativeMouseDownValue.normalized  ) + 180  );
				m_ActiveRadialGUI.m_SelectedIndex = (int)( m_NormalizedSelection * m_ActiveRadialGUI.m_Options.Length );
				m_ActiveRadialGUI.m_SelectedIndex = m_ActiveRadialGUI.m_SelectedIndex.WrapIntToRange( 0, m_ActiveRadialGUI.m_Options.Length - 1 );

				GUI.Label( new Rect( ET_InputManager.Instance.m_MouseDownPos.x - (contentSize/2) , ( Screen.height - ET_InputManager.Instance.m_MouseDownPos.y ) - 15, contentSize, 30 ), m_ActiveRadialGUI.m_Options[ m_ActiveRadialGUI.m_SelectedIndex ], centeredStyle ) ;
			}
		}
		*/

	}

}
