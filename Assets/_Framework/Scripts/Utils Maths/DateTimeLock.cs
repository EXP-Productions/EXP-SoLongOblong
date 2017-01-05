using UnityEngine;
using System.Collections;

public class DateTimeLock : MonoBehaviour
{
	public int m_Date = 1;
	public int m_Month = 1;
	public int m_Year = 2015;

	void Awake()
	{
		if (System.DateTime.Now.Year > m_Year)
			Lock();
		else if (System.DateTime.Now.Month > m_Month)
			Lock();
		else if (System.DateTime.Now.Day > m_Date)
			Lock();
		else
			print ("Not locked");
	}

	void Lock()
	{
		print ("Locked");
		/*
		if( Application.isEditor )
			UnityEditor.EditorApplication.isPlaying = false;
		else
			Application.Quit ();
			*/
	}
}
