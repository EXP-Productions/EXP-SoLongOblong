using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ComboBox))]
public class ComboBoxEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		var comboBox = target as ComboBox;

		var allowUpdate = comboBox.transform.Find("Button") != null;

		if (allowUpdate)
			comboBox.UpdateGraphics();
		
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck())
		{
			if (Application.isPlaying)
			{
				comboBox.HideFirstItem = comboBox.HideFirstItem;
				comboBox.Interactable = comboBox.Interactable;
			}
			else
				if (allowUpdate)
					comboBox.RefreshSelected();
		}
	}
}

public class ComboBoxMenuItem
{
	[MenuItem("GameObject/UI/ComboBox")]
	public static void CreateComboBox()
	{
		var canvas = Object.FindObjectOfType<Canvas>();
		var canvasGO = canvas == null ? null : canvas.gameObject;
		if (canvasGO == null)
		{
			canvasGO = new GameObject("Canvas");
			canvas = canvasGO.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.AddComponent<CanvasScaler>();
			canvasGO.AddComponent<GraphicRaycaster>();
		}
		var eventSystem = Object.FindObjectOfType<EventSystem>();
		var eventSystemGO = eventSystem == null ? null : eventSystem.gameObject;
		if (eventSystemGO == null)
		{
			eventSystemGO = new GameObject("EventSystem");
			eventSystem = eventSystemGO.AddComponent<EventSystem>();
			eventSystemGO.AddComponent<StandaloneInputModule>();
			eventSystemGO.AddComponent<TouchInputModule>();
		}
		var comboBox = new GameObject("ComboBox");
		comboBox.transform.SetParent(canvasGO.transform);
		var rTransform = comboBox.AddComponent<RectTransform>();
		rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
		rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
		for (var i = 0; i < Selection.objects.Length; i++)
		{
			var selected = Selection.objects[i] as GameObject;
			var hierarchyItem = selected.transform;
			canvas = null;
			while (hierarchyItem != null && (canvas = hierarchyItem.GetComponent<Canvas>()) == null)
				hierarchyItem = hierarchyItem.parent;
			if (canvas != null)
			{
				comboBox.transform.SetParent(selected.transform);
				break;
			}
		}
		rTransform.anchoredPosition = Vector2.zero;
		var styledComboBox = comboBox.AddComponent<ComboBox>();
		styledComboBox.CreateControl();
		Selection.activeGameObject = comboBox;
	}
}