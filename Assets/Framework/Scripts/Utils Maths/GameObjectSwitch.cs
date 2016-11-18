using UnityEngine;
using System.Collections;

// Activates and deactivates GOs

public class GameObjectSwitch : MonoBehaviour
{
	GameObject[] m_GameObjects;
	public int m_SelectedIndex;

	public bool m_UseKBInput = true;

	void Update ()
	{
		if( m_UseKBInput )
		{
			if (Input.GetKeyDown (KeyCode.Alpha1))		SelectIndex (0);
			else if (Input.GetKeyDown (KeyCode.Alpha2))	SelectIndex (1);
			else if (Input.GetKeyDown (KeyCode.Alpha3))	SelectIndex (2);
			else if (Input.GetKeyDown (KeyCode.Alpha4))	SelectIndex (3);
			else if (Input.GetKeyDown (KeyCode.Alpha5))	SelectIndex (4);
			else if (Input.GetKeyDown (KeyCode.Alpha6))	SelectIndex (5);
			else if (Input.GetKeyDown (KeyCode.Alpha7))	SelectIndex (6);
			else if (Input.GetKeyDown (KeyCode.Alpha8))	SelectIndex (7);
			else if (Input.GetKeyDown (KeyCode.Alpha9))	SelectIndex (8);
		}
	}

	public void SelectIndex( int index )
	{
		m_SelectedIndex = index;
		for (int i = 0; i < m_GameObjects.Length; i++) 
		{
			m_GameObjects[i].SetActive( i == m_SelectedIndex ); 
		}
	}
}
