using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[ RequireComponent( typeof(LineRenderer)) ]
public class MeasurmentTool : MonoBehaviour
{
	public List< GameObject > m_SelectedObjects = new List<GameObject>();

	LineRenderer m_Line;

	public List< TextMesh > m_TextMeshes;

	public int m_FontSize = 40;
	public float m_Scale = 1;



	void Start ()
	{
		m_Line = gameObject.GetComponent< LineRenderer > ();


	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.A))
			m_TextMeshes.Clear ();

		if( m_Line == null )
			m_Line = gameObject.GetComponent< LineRenderer > ();



		//hax
		if (m_TextMeshes.Count < (m_SelectedObjects.Count * 3))
		{
			while (m_TextMeshes.Count < ( m_SelectedObjects.Count * 3))
			{
				TextMesh newtext = new GameObject ("text mesh " + m_TextMeshes.Count).AddComponent< TextMesh > ();
				m_TextMeshes.Add (newtext);
			}
		}

		textCount = 0;

		foreach (TextMesh t in m_TextMeshes)
		{
			t.transform.rotation = Camera.main.transform.rotation;
			t.transform.localScale = Vector3.one * m_Scale;
			t.fontSize = m_FontSize;
		}

		// If more than one vert selected
		if (m_SelectedObjects.Count > 1)
		{
			m_Line.enabled = true;
			m_Line.SetVertexCount (m_SelectedObjects.Count);

			for (int i = 0; i < m_SelectedObjects.Count; i++)
			{
				m_Line.SetPosition (i, m_SelectedObjects [i].transform.position);

				int prevIndex = ( i - 1 );
				if( prevIndex < 0 ) prevIndex = m_SelectedObjects.Count - 1;

				int nextIndex = ( i + 1 );
				if( nextIndex > m_SelectedObjects.Count - 1 ) nextIndex = 0;

				// Edge length
				TextMesh edgeText = GetTextMesh();
				EdgeLength( edgeText,  m_SelectedObjects[i].transform.position, m_SelectedObjects[nextIndex].transform.position );

				// Line
				m_Line.SetVertexCount ( m_SelectedObjects.Count + 1 );
				m_Line.SetPosition ( m_SelectedObjects.Count, m_SelectedObjects [0].transform.position);

				// Angle
				Vector3 edge1 = m_SelectedObjects[i].transform.position - m_SelectedObjects[prevIndex].transform.position;
				Vector3 edge2 = m_SelectedObjects[i].transform.position - m_SelectedObjects[nextIndex].transform.position;
				float angle = Vector3.Angle (edge1, edge2);
				TextMesh vertext = GetTextMesh();
				vertext.text = "v"+i + ": " + angle.ToString("##");
				vertext.transform.position = m_SelectedObjects[i].transform.position;
			}

			// hax trianlge area
			float area = MathExtensions.AreaOfTriangle( m_SelectedObjects [0].transform.position, m_SelectedObjects [1].transform.position, m_SelectedObjects [2].transform.position );
			TextMesh areaText = GetTextMesh();
			areaText.text = area.ToString("##.##");
			areaText.transform.position = ( m_SelectedObjects [0].transform.position + m_SelectedObjects [1].transform.position + m_SelectedObjects [2].transform.position ) / 3f;


			for (int i = textCount + 1; i < m_TextMeshes.Count; i++)
			{
				m_TextMeshes[ i ].gameObject.SetActive( false );
			}
		}
		else
		{
			m_Line.enabled = false;
			m_Line.SetVertexCount ( 0 );
		}
		
	}




	int textCount;
	TextMesh GetTextMesh()
	{
		m_TextMeshes[ textCount ].gameObject.SetActive(true);
		textCount++;
		return m_TextMeshes [textCount];
	}

	void EdgeLength( TextMesh text, Vector3 pos1, Vector3 pos2 )
	{
		text.transform.position = ( pos1 + pos2 ) / 2f;
		text.text = (( pos1 - pos2 ).magnitude * 1000 ).ToString("####");
	}



	void ClearSelection()
	{
		m_SelectedObjects.Clear();
	}
}
