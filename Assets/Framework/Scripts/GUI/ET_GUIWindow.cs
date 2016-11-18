using UnityEngine;
using System.Collections;

public class ET_GUIWindow : MonoBehaviour
{
	public enum ScreenXAnchor
	{
		None,
		Left,
		Middle,
		Right,
	}

	public enum ScreenYAnchor
	{		
		None,
		Top,
		Middle,
		Bottom,
	}

	int m_WindowIndex;
	public ScreenXAnchor m_XAnchor = ScreenXAnchor.None;
	public ScreenYAnchor m_YAnchor = ScreenYAnchor.None;

	public bool m_DrawWindow = false;
	public Rect m_WindowGUI_Rect = new Rect( Screen.width / 3 , Screen.height/2, 250, 400);
	Rect m_OffsetWindowGUI_Rect;
	public string m_WindowName = "";
	
	public GameObject m_ObjectToCallFrom;

	public KeyCode m_ToggleKey = KeyCode.Insert;

	public bool m_Horizontal = false;

	public bool m_ClampToScreenSpace = true;

	int m_WindowIndexCount = 0;
		
	void Awake()
	{
		m_WindowIndex = GetWindowIndex();


		//WindowMod.onSetScreenCount += onSetScreenCount;
		m_OffsetWindowGUI_Rect = m_WindowGUI_Rect.Copy();

	}

	void Start()
	{
		ET_GUIManager.Instance.RegisterWindow( this );
	}

	int GetWindowIndex()
	{
		m_WindowIndex++;
		return m_WindowIndex;
	}

	void Update()
	{
		//if( m_ClampToScreenSpace )
		//	m_WindowGUI_Rect.x = Mathf.Min( m_WindowGUI_Rect.x, ( Screen.width / WindowMod.m_ScreenCount ) - m_WindowGUI_Rect.width );

		if( Input.GetKey( KeyCode.LeftShift ) && Input.GetKeyDown( m_ToggleKey ) )
		{
			m_DrawWindow = !m_DrawWindow;
		}

		if( Input.GetKey( KeyCode.LeftShift ) && Input.GetKeyDown( KeyCode.E) )
		{
			Anchor();
		}
	}
	
	public void Init( string name, GameObject gO )
	{
		m_ObjectToCallFrom = gO;
		m_WindowName = name;

		Anchor();
	}

	void onSetScreenCount( int screens )
	{
		Invoke( "Anchor", .6f );
	}

	void Anchor()
	{
		//float adjustedWidth = Screen.width / WindowMod.m_ScreenCount;
		float adjustedWidth = Screen.width;
		
		if( m_XAnchor == ScreenXAnchor.Left )
		{
			m_WindowGUI_Rect.x = 0;
		}
		else if( m_XAnchor == ScreenXAnchor.Right )
		{
			m_WindowGUI_Rect.x = adjustedWidth - m_WindowGUI_Rect.width;
		}
		else if( m_XAnchor == ScreenXAnchor.Middle )
		{
			m_WindowGUI_Rect.x = (adjustedWidth/2) - ( m_WindowGUI_Rect.width / 2 );
		}
		
		if( m_YAnchor == ScreenYAnchor.Top )
		{
			m_WindowGUI_Rect.y = 35;
		}
		else if( m_YAnchor == ScreenYAnchor.Bottom )
		{
			m_WindowGUI_Rect.y = Screen.height - m_WindowGUI_Rect.height;
		}
		else if( m_YAnchor == ScreenYAnchor.Middle )
		{
			m_WindowGUI_Rect.y = ( Screen.height / 2 ) - ( m_WindowGUI_Rect.height / 2 );
		}
	}

	public void Init( string name, GameObject gO, KeyCode key )
	{
		Init( name, gO );
		m_ToggleKey = key;
	}

	public void ToggleWindow()
	{
		m_DrawWindow = !m_DrawWindow;
	}

	string blankWindowName = "";
	public void BeginWindow( )
	{
		if( ET_GUIManager.Instance.SoloMode && ET_GUIManager.Instance.SoloWindow != this )	// If in solo mode and this isnt the solo window, return
			return;

		if( m_DrawWindow && ET_GUIManager.Instance.m_DrawGUI )
		{
			m_OffsetWindowGUI_Rect.x = ET_GUIManager.Instance.m_Offset.x + m_WindowGUI_Rect.x;
			m_OffsetWindowGUI_Rect.y = ET_GUIManager.Instance.m_Offset.y + m_WindowGUI_Rect.y;
			m_OffsetWindowGUI_Rect.width = m_WindowGUI_Rect.width;
			m_OffsetWindowGUI_Rect.height = m_WindowGUI_Rect.height;

			ET_GUIManager.Instance.UseDefaultSkin();
			m_WindowGUI_Rect = GUILayout.Window( m_WindowIndex, m_OffsetWindowGUI_Rect, DrawWindow_GUI, blankWindowName );

			m_WindowGUI_Rect.x -= ET_GUIManager.Instance.m_Offset.x;
			m_WindowGUI_Rect.y -=  ET_GUIManager.Instance.m_Offset.y;
		}
	}

	public void DrawWindowButton()
	{
		if( GUILayout.Button( m_WindowName, GUILayout.Width( 150 )) )
			ToggleWindow();
	}
	
	public void DrawWindow_GUI( int windowID )
	{

		GUILayout.BeginHorizontal("box");
		{
			GUILayout.Label(m_WindowName);
			GUILayout.FlexibleSpace();
			if( GUILayout.Button( "S", GUILayout.Width( 25 )) )
			{
				ET_GUIManager.Instance.SoloToggle( this );
			}
			if( GUILayout.Button( "X", GUILayout.Width( 25 )) )
				m_DrawWindow = false;
		}
		GUILayout.EndHorizontal();

		if( m_Horizontal )
		{
			GUILayout.BeginHorizontal("box");
			{
				m_ObjectToCallFrom.SendMessage( "DrawGUIWindow" );
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginVertical("box");
			{
				m_ObjectToCallFrom.SendMessage( "DrawGUIWindow" );
			}
			GUILayout.EndVertical();
		}

		
		GUI.DragWindow( new Rect(0, 0, Screen.width, Screen.height ) );	
	}
	
}
