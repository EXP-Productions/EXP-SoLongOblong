using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class XSettings : EditorWindow
{
    public bool recalcNorms;
    public bool recalcBounds;
    public float exportScale;
    public bool saveExternal;
    public string saveDirectory;
    public bool saveSettings;

    private static EditorWindow owindow;
    public static void ShowOptionsWindow()
    {
        owindow = EditorWindow.GetWindow(typeof(XSettings));
        owindow.title = "OPTIONS";
        owindow.minSize = new Vector2(600, 200);
        owindow.Show();
    }

    void OnEnable()
    {
        recalcNorms = EditorPrefs.GetBool("RecalcNormals");
        recalcBounds = EditorPrefs.GetBool("RecalcBounds");
        exportScale = EditorPrefs.GetFloat("ExportScale");
        saveExternal = EditorPrefs.GetBool("SaveExternal");
        saveDirectory = EditorPrefs.GetString("SaveDirectory");
        if (exportScale == 0) exportScale = 100;
        if (saveDirectory == null)
        {
            saveDirectory = Directory.GetCurrentDirectory();
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        EditorStyles.label.wordWrap = true;

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        saveExternal = EditorGUILayout.Toggle("Save to External Directory?", saveExternal);
        EditorGUILayout.EndHorizontal();

        if (saveExternal)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("Save Directory", saveDirectory);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.green;
            var chooseDirectory = GUILayout.Button("Choose Directory");
            EditorGUILayout.EndHorizontal();

            if (chooseDirectory)
            {
                saveDirectory = EditorUtility.SaveFolderPanel("Select Directory", "", "");
            }
        }

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        recalcNorms = EditorGUILayout.Toggle("Fix Normals(slow)", recalcNorms);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        recalcBounds = EditorGUILayout.Toggle("Fix Bounds(slower)", recalcBounds);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        exportScale = EditorGUILayout.FloatField("Export Scale", exportScale);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.cyan;
        EditorGUILayout.LabelField("Normal Export Scale for Unity is 100. This May Be Adjusted for other 3D Software.");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.green;
        saveSettings = GUILayout.Button("Save");
        EditorGUILayout.EndHorizontal();

        if (saveSettings)
        {
            EditorPrefs.SetBool("RecalcNormals", recalcNorms);
            EditorPrefs.SetBool("RecalcBounds", recalcBounds);
            EditorPrefs.SetFloat("ExportScale", exportScale);
            EditorPrefs.SetBool("SaveExternal", saveExternal);
            EditorPrefs.SetString("SaveDirectory", saveDirectory);
            owindow.Close();
        }
    }

}