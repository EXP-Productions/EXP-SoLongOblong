using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Linq;
using System.Threading;

public struct ObjMaterial
{
    public string name;
    public string textureName;
    public Color colors;
    public float alpha;
}

public class XMain : EditorWindow
{
    static Process xProcess = new Process();
    public bool recalcBounds;
    public bool recalcNorms;
    public float exportScale;
    public bool saveExternal;
    public string saveDirectory;
    public Vector3 originalScale;
    private int vertexOffset = 0;
    private int normalOffset = 0;
    private int uvOffset = 0;
    public int vertexSize;
    public static string targetFolder = "X-PORTS";
    public string batchDir = targetFolder + Path.DirectorySeparatorChar + "BATCH";
    public string batchDirOBJ = targetFolder + Path.DirectorySeparatorChar + "BATCH" + Path.DirectorySeparatorChar + "OBJ";
    public string batchDirFBX = targetFolder + Path.DirectorySeparatorChar + "BATCH" + Path.DirectorySeparatorChar + "FBX";
    public static string copyFolder = "tmp";
    public string uni32Copy;
    public string fileName;
    public string selectionName;
    public string fbxTemp = copyFolder + Path.DirectorySeparatorChar + "FBX";
    public string daeTemp = copyFolder + Path.DirectorySeparatorChar + "DAE";
    public string dxfTemp = copyFolder + Path.DirectorySeparatorChar + "DXF";
    public string objMove = targetFolder + Path.DirectorySeparatorChar + "OBJ";
    public string fbxMove = targetFolder + Path.DirectorySeparatorChar + "FBX";
    public string daeMove = targetFolder + Path.DirectorySeparatorChar + "DAE";
    public string dxfMove = targetFolder + Path.DirectorySeparatorChar + "DXF";
    public string destfbx;
    public string destdae;
    public string destdxf;
    public string destbatch;
    public bool hasTexture = false;
    public bool isOptions = false;
    public bool isReset;
    public bool isLegacy = true;
    bool showLegacy = true;
    string m_Console = "All is Well";
    bool showOBJ;
    bool showFBX;
    bool showDAE;
    bool showDXF;
    bool showBatch;
    bool showOBJBatch;
    bool showFBXBatch;
    public bool isobj;
    public bool isfbx;
    public bool isdae;
    public bool isdxf;
    public bool isbat;
    public bool doobj;
    public bool dofbx;
    public bool dodae;
    public bool dodxf;
    private Texture2D m_Logo = null;
    private Texture2D m_Info = null;
    public string meshtypeLabel = "Thinking...";
    public Color legacyColor = Color.green;
    public Color batchColor = Color.white;
    public Color consoleColor = Color.green;
    public bool isBatch;
    public string batchnames;
    public Transform selectionParent;
    public bool infoLegacy;
    public bool infoBatch;
    public string InfoLegacy = "This is the Legacy mode, which exports MeshFilter's to a combined Model format. All selected MeshFilters are written to a single file.";
    public string InfoBatch = "Batch mode will export all selected MeshFilters to separate files. Initially only supports Legacy and OBJ export.";
    public float x_TotalValue = 10000;
    public float x_ProgressValue;
    public string x_ProgText = "Processing...";
    public Dictionary<string, ObjMaterial> PrepareFileWrite()
    {
        vertexOffset = 0;
        normalOffset = 0;
        uvOffset = 0;
        return new Dictionary<string, ObjMaterial>();
    }

    private static EditorWindow window;
    [MenuItem("Tools/M-Theory/X-Change 3D")]
    public static void ShowWindow()
    {
        window = EditorWindow.GetWindow(typeof(XMain));
        window.title = "X-CHANGE 3D";
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(300, 300);
        window.Show();
    }

    void OnEnable()
    {
        recalcNorms = EditorPrefs.GetBool("RecalcNorms");
        recalcBounds = EditorPrefs.GetBool("RecalcBounds");
        exportScale = EditorPrefs.GetFloat("ExportScale");
        saveExternal = EditorPrefs.GetBool("SaveExternal");
        saveDirectory = EditorPrefs.GetString("SaveDirectory");
        m_Logo = (Texture2D)Resources.Load("logo", typeof(Texture2D));
        m_Info = (Texture2D)Resources.Load("info", typeof(Texture2D));
        if (!Directory.Exists(copyFolder)) createStruct();
    }

    void OnInspectorUpdate()
    {
        recalcNorms = EditorPrefs.GetBool("RecalcNorms");
        recalcBounds = EditorPrefs.GetBool("RecalcBounds");
        exportScale = EditorPrefs.GetFloat("ExportScale");
        saveExternal = EditorPrefs.GetBool("SaveExternal");
        saveDirectory = EditorPrefs.GetString("SaveDirectory");
        if (!Directory.Exists(copyFolder)) createStruct();
        Repaint();
    }

    void OnDestroy()
    {
        resetScene();
    }

    void OnGUI()
    {
        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        if (selection.Length == 0 || selection == null)
        {
            consoleColor = Color.magenta;
            m_Console = "Please Select an Object in Scene View";
        }
        if (selection.Length > 0 && selection != null)
        {
            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter)).Concat(selection[i].GetComponentsInChildren(typeof(SkinnedMeshRenderer))).ToArray();
                if (meshfilter.Length < 1)
                {
                    consoleColor = Color.magenta;
                    m_Console = "Please Select an Object in Scene View";
                }
                for (int m = 0; m < meshfilter.Length; m++)
                {
                    if (m > 0)
                    {
                        consoleColor = Color.green;
                        m_Console = "Ready to Process " + selection[i].name;
                    }
                }
            }
        }

        GUI.color = Color.yellow;
        GUI.Label(new Rect(40, 10, Screen.width, 30), "Mesh Type: " + meshtypeLabel + " File Scale: " + exportScale);

        GUI.color = Color.white;
        GUI.backgroundColor = Color.clear;
        infoLegacy = GUI.Button(new Rect(60, 65, 30, 30), m_Info);
        if (infoLegacy) EditorUtility.DisplayDialog("MESH", InfoLegacy, "Okay");

        GUI.color = legacyColor;
        GUI.backgroundColor = Color.white;
        isLegacy = GUI.Button(new Rect(0, 90, Screen.width / 2, 30), "MESH");
        if (isLegacy)
        {
            showLegacy = true;
            showBatch = false;
            legacyColor = Color.green;
            batchColor = Color.white;
        }

        GUI.color = Color.white;
        GUI.backgroundColor = Color.clear;
        infoBatch = GUI.Button(new Rect(Screen.width - 80, 65, 30, 30), m_Info);
        if (infoBatch) EditorUtility.DisplayDialog("BATCH", InfoBatch, "Okay");

        GUI.color = batchColor;
        GUI.backgroundColor = Color.white;
        isBatch = GUI.Button(new Rect(Screen.width - 150, 90, Screen.width / 2, 30), "BATCH");
        if (isBatch)
        {
            showLegacy = false;
            showBatch = true;
            legacyColor = Color.white;
            batchColor = Color.green;
        }

        GUI.color = Color.cyan;
        if (true == GUI.Button(new Rect(0, 40, Screen.width / 2, 20), "Open X-Ports Folder"))
        {
            OpenInFileBrowser(targetFolder);
        }


        GUI.color = Color.yellow;
        isOptions = GUI.Button(new Rect(160, 40, 60, 20), "Options");
        if (isOptions)
        {
            XSettings.ShowOptionsWindow();
        }

        GUI.color = Color.red;
        isReset = GUI.Button(new Rect(Screen.width - 60, 40, 60, 20), "RESET");
        if (isReset)
        {
            resetScene();
        }

        GUI.color = Color.white;
        GUI.Label(new Rect(150 - 64, 170, 128, 128), m_Logo);

        if (showLegacy)
        {
            GUI.color = Color.white;
            showOBJ = GUI.Button(new Rect(0, 125, Screen.width, 30), "X-Port to OBJ");
            showFBX = GUI.Button(new Rect(0, 165, Screen.width, 30), "X-Port to FBX");
            showDAE = GUI.Button(new Rect(0, 200, Screen.width, 30), "X-Port to DAE");
            showDXF = GUI.Button(new Rect(0, 235, Screen.width, 30), "X-Port to DXF");
        }

        if (showBatch)
        {
            GUI.color = Color.white;
            showOBJBatch = GUI.Button(new Rect(0, 130, Screen.width, 30), "X-Port to OBJ");
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                showFBXBatch = GUI.Button(new Rect(0, 165, Screen.width, 30), "X-Port to FBX");
            }
        }

        if (showOBJ)
        {
            resetScene();
            runOBJ();
        }

        if (showFBX)
        {
            resetScene();
            runFBX();
        }

        if (showDAE)
        {
            resetScene();
            runDAE();
        }

        if (showDXF)
        {
            resetScene();
            runDXF();
        }

        if (showOBJBatch)
        {
            resetScene();
            runOBJBatch();
        }

        if (showFBXBatch)
        {
            resetScene();
            runFBXBatch();
        }

        GUI.color = consoleColor;
        GUI.TextArea(new Rect(0, 270, Screen.width, 30), m_Console);
    }

    void runOBJ()
    {
        try
        {
            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
            {
                resetScene();
                consoleColor = Color.red;
                m_Console = "Please select one or more target objects";
                return;
            }

            string destdir = targetFolder + Path.DirectorySeparatorChar + "OBJ";
            if (!Directory.Exists(destdir)) Directory.CreateDirectory(destdir);
            isobj = true;
            isfbx = false;
            isdae = false;
            isdxf = false;
            doobj = true;
            dofbx = false;
            dodae = false;
            dodxf = false;
            selectionParent = Selection.activeTransform.root;
            originalScale = new Vector3(selectionParent.transform.localScale.x, selectionParent.transform.localScale.y, selectionParent.transform.localScale.z);
            selectionParent.localScale = new Vector3(selectionParent.localScale.x * exportScale, selectionParent.localScale.y * exportScale, selectionParent.localScale.z * exportScale);
			selectionName = Selection.activeTransform.name;
            if (selectionName.Contains(" ")) selectionName = selectionName.Replace(" ", "");

            int exportedObjects = 0;
            ArrayList mfList = new ArrayList();

            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter)).Concat(selection[i].GetComponentsInChildren(typeof(SkinnedMeshRenderer))).ToArray();

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    mfList.Add(meshfilter[m]);
                }
            }

            if (exportedObjects == 0)
            {
                selectionParent.localScale = originalScale;
                resetScene();
                consoleColor = Color.red;
                m_Console = "Selected Object has no Mesh Filters.";
                return;
            }

            else if (exportedObjects > 0)
            {
                Component[] mf = new Component[mfList.Count];

                for (int i = 0; i < mfList.Count; i++)
                {
                    mf[i] = (Component)mfList[i];
                }

                string filename = selectionName;
                MeshesToFile(mf, destdir, filename);
                OpenInFileBrowser(destdir);
                EditorUtility.ClearProgressBar();
                selectionParent.localScale = originalScale;

                consoleColor = Color.green;
                m_Console = "Success!";
                m_Console += "\r\n";
                foreach (Transform tr in selection)
                {
                    m_Console += "Exported " + tr.name + " to " + targetFolder;
                }
            }
            if (saveExternal)
            {
                string saveDate = String.Format("{0:MM-dd-yyyy_hh-mm-ss}", DateTime.Now);
                DirectoryCopy(objMove, saveDirectory + Path.DirectorySeparatorChar + objMove + "_" + saveDate, true);
            }
            doobj = false;
        }
        catch (Exception ex)
        {
            resetScene();
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
        }
    }

    void runOBJBatch()
    {
        try
        {
            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
            {
                resetScene();
                consoleColor = Color.red;
                m_Console = "Please select one or more target objects";
                return;
            }

            if (!Directory.Exists(batchDirOBJ)) Directory.CreateDirectory(batchDirOBJ);
            isobj = true;
            isfbx = false;
            isdae = false;
            isdxf = false;
            doobj = true;
            dofbx = false;
            dodae = false;
            dodxf = false;
            isbat = true;
            selectionParent = Selection.activeTransform.root;
            originalScale = new Vector3(selectionParent.transform.localScale.x, selectionParent.transform.localScale.y, selectionParent.transform.localScale.z);
            selectionParent.localScale = new Vector3(selectionParent.localScale.x * exportScale, selectionParent.localScale.y * exportScale, selectionParent.localScale.z * exportScale);

            int exportedObjects = 0;

            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter)).Concat(selection[i].GetComponentsInChildren(typeof(SkinnedMeshRenderer))).ToArray();

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    MeshToFile((Component)meshfilter[m], batchDirOBJ, selection[i].name + "_" + i + "_" + m);
                    batchnames = selection[i].name + "_" + i + "_" + m;
                }
            }

            if (exportedObjects == 0)
            {
                selectionParent.localScale = originalScale;
                consoleColor = Color.red;
                resetScene();
                m_Console = "Selected Object has no Mesh Filters or SMR's.";
                return;
            }

            else if (exportedObjects > 0)
            {
                OpenInFileBrowser(batchDirOBJ);
            }

            selectionParent.localScale = originalScale;
            EditorUtility.ClearProgressBar();
            selectionParent.localScale = originalScale;

            consoleColor = Color.green;
            m_Console = "Success!";
            m_Console += "\r\n";
            foreach (Transform tr in selection)
            {
                m_Console += "Exported " + tr.name + " to " + targetFolder;
            }

            if (saveExternal)
            {
                string saveDate = String.Format("{0:MM-dd-yyyy_hh-mm-ss}", DateTime.Now);
                DirectoryCopy(batchDirOBJ, saveDirectory + Path.DirectorySeparatorChar + batchDirOBJ + "_" + saveDate, true);
            }
            doobj = false;
        }

        catch (Exception ex)
        {
            resetScene();
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
        }
    }

    void runFBX()
    {
        try
        {
            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
            {
                resetScene();
                consoleColor = Color.red;
                m_Console = "Please select one or more target objects";
                return;
            }

            isobj = false;
            isfbx = true;
            isdae = false;
            isdxf = false;
            dofbx = true;
            doobj = false;
            dodae = false;
            dodxf = false;
            selectionParent = Selection.activeTransform.root;
            originalScale = new Vector3(selectionParent.transform.localScale.x, selectionParent.transform.localScale.y, selectionParent.transform.localScale.z);
            selectionParent.localScale = new Vector3(selectionParent.localScale.x * exportScale, selectionParent.localScale.y * exportScale, selectionParent.localScale.z * exportScale);
			selectionName = Selection.activeTransform.name;
            if (selectionName.Contains(" ")) selectionName = selectionName.Replace(" ", "");

            int exportedObjects = 0;
            ArrayList mfList = new ArrayList();

            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter)).Concat(selection[i].GetComponentsInChildren(typeof(SkinnedMeshRenderer))).ToArray();
                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    mfList.Add(meshfilter[m]);
                }
            }

            if (exportedObjects == 0)
            {
                resetScene();
                selectionParent.localScale = originalScale;
                consoleColor = Color.red;
                m_Console = "Selected Object has no Mesh Filters";
                return;
            }

            else if (exportedObjects > 0)
            {
                Component[] mf = new Component[mfList.Count];

                for (int i = 0; i < mfList.Count; i++)
                {
                    mf[i] = (Component)mfList[i];
                }

                string filename = selectionName;
                int stripIndex = filename.LastIndexOf(Path.DirectorySeparatorChar);
                if (stripIndex >= 0)
                    filename = filename.Substring(stripIndex + 1).Trim();
                MeshesToFile(mf, copyFolder, filename);

                consoleColor = Color.green;
                m_Console = "Success!";
                m_Console += "\r\n";
                foreach (Transform tr in selection)
                {
                    m_Console += "Exported " + tr.name + " to " + targetFolder;
                }
            }
            uniFBX();
        }
        catch (Exception ex)
        {
            resetScene();
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
        }
        EditorUtility.ClearProgressBar();
    }

    void runFBXBatch()
    {
        try
        {
            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
            {
                resetScene();
                consoleColor = Color.red;
                m_Console = "Please select one or more target objects";
                return;
            }

            if (!Directory.Exists(batchDirFBX)) Directory.CreateDirectory(batchDirFBX);
            isobj = false;
            isfbx = true;
            isdae = false;
            isdxf = false;
            doobj = false;
            dofbx = true;
            dodae = false;
            dodxf = false;
            isbat = true;
            selectionParent = Selection.activeTransform.root;
            originalScale = new Vector3(selectionParent.transform.localScale.x, selectionParent.transform.localScale.y, selectionParent.transform.localScale.z);

            selectionParent.localScale = new Vector3(selectionParent.localScale.x * exportScale, selectionParent.localScale.y * exportScale, selectionParent.localScale.z * exportScale);

            int exportedObjects = 0;

            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter)).Concat(selection[i].GetComponentsInChildren(typeof(SkinnedMeshRenderer))).ToArray();

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    batchnames = selection[i].name + "_" + i + "_" + m;
                    exportedObjects++;
                    MeshToFile((Component)meshfilter[m], batchDir, selection[i].name + "_" + i + "_" + m);
                }
            }

            if (exportedObjects == 0)
            {
                selectionParent.localScale = originalScale;
                consoleColor = Color.red;
                resetScene();
                m_Console = "Selected Object has no Mesh Filters or SMR's.";
                return;
            }

            else if (exportedObjects > 0)
            {
                OpenInFileBrowser(batchDirFBX);

                consoleColor = Color.green;
                m_Console = "Success!";
                m_Console += "\r\n";
                foreach (Transform tr in selection)
                {
                    m_Console += "Exported " + tr.name + " to " + targetFolder;
                }
            }
            selectionParent.localScale = originalScale;
            EditorUtility.ClearProgressBar();

            if (saveExternal)
            {
                string saveDate = String.Format("{0:MM-dd-yyyy_hh-mm-ss}", DateTime.Now);
                DirectoryCopy(batchDirFBX, saveDirectory + Path.DirectorySeparatorChar + batchDirFBX + "_" + saveDate, true);
            }
            dofbx = false;
        }

        catch (Exception ex)
        {
            resetScene();
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
        }
    }

    void runDAE()
    {
        try
        {
            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
            {
                resetScene();
                consoleColor = Color.red;
                m_Console = "Please select one or more target objects";
                return;
            }

            isobj = false;
            isfbx = false;
            isdae = true;
            isdxf = false;
            dodae = true;
            dofbx = false;
            doobj = false;
            dodxf = false;
            selectionParent = Selection.activeTransform.root;
            originalScale = new Vector3(selectionParent.transform.localScale.x, selectionParent.transform.localScale.y, selectionParent.transform.localScale.z);
            selectionParent.localScale = new Vector3(selectionParent.localScale.x * exportScale, selectionParent.localScale.y * exportScale, selectionParent.localScale.z * exportScale);

			selectionName = Selection.activeTransform.name;
            if (selectionName.Contains(" ")) selectionName = selectionName.Replace(" ", "");

            int exportedObjects = 0;
            ArrayList mfList = new ArrayList();

            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter)).Concat(selection[i].GetComponentsInChildren(typeof(SkinnedMeshRenderer))).ToArray();

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    mfList.Add(meshfilter[m]);
                }
            }

            if (exportedObjects == 0)
            {
                selectionParent.localScale = originalScale;
                consoleColor = Color.red;
                resetScene();
                m_Console = "Selected Object has no Mesh Filters";
                return;
            }

            else if (exportedObjects > 0)
            {
                Component[] mf = new Component[mfList.Count];

                for (int i = 0; i < mfList.Count; i++)
                {
                    mf[i] = (Component)mfList[i];
                }

                string filename = selectionName;
                int stripIndex = filename.LastIndexOf(Path.DirectorySeparatorChar);
                if (stripIndex >= 0)
                    filename = filename.Substring(stripIndex + 1).Trim();
                MeshesToFile(mf, copyFolder, filename);

                consoleColor = Color.green;
                m_Console = "Success!";
                m_Console += "\r\n";
                foreach (Transform tr in selection)
                {
                    m_Console += "Exported " + tr.name + " to " + targetFolder;
                }
            }
            uniDAE();
            EditorUtility.ClearProgressBar();
        }

        catch (Exception ex)
        {
            resetScene();
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
        }
    }

	// Exports DXF
    void runDXF()
    {
        try
        {
            Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
            {
                resetScene();
                consoleColor = Color.red;
                m_Console = "Please select one or more target objects";
                return;
            }

            isobj = false;
            isfbx = false;
            isdae = false;
            isdxf = true;
            dodxf = true;
            dofbx = false;
            dodae = false;
            doobj = false;
            selectionParent = Selection.activeTransform.root;
            originalScale = new Vector3(selectionParent.transform.localScale.x, selectionParent.transform.localScale.y, selectionParent.transform.localScale.z);
            selectionParent.localScale = new Vector3(selectionParent.localScale.x * exportScale, selectionParent.localScale.y * exportScale, selectionParent.localScale.z * exportScale);

			selectionName = Selection.activeTransform.name;
            if (selectionName.Contains(" ")) selectionName = selectionName.Replace(" ", "");

			// Find objects to be exported by building a list of meshfilters in children
            int exportedObjects = 0;
            ArrayList mfList = new ArrayList();

            for (int i = 0; i < selection.Length; i++)
            {
                Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter)).Concat(selection[i].GetComponentsInChildren(typeof(SkinnedMeshRenderer))).ToArray();
                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    mfList.Add(meshfilter[m]);
                }
            }

			// if no meshfilters found...
            if (exportedObjects == 0)
            {
                selectionParent.localScale = originalScale;
                consoleColor = Color.red; selectionParent.localScale = originalScale;
                resetScene();
                m_Console = "Selected Object has no Mesh Filters";
                return;
            }
			// If mesh filters are found
            else if (exportedObjects > 0)
            {
                Component[] mf = new Component[mfList.Count];

                for (int i = 0; i < mfList.Count; i++)
                {
                    mf[i] = (Component)mfList[i];
                }

				// creates filename from selection name
                string filename = selectionName;
                int stripIndex = filename.LastIndexOf(Path.DirectorySeparatorChar);
                if (stripIndex >= 0)
                    filename = filename.Substring(stripIndex + 1).Trim();

				// Meshes to file
                MeshesToFile(mf, copyFolder, filename);

                consoleColor = Color.green;
                m_Console = "Success!";
                m_Console += "\r\n";
                foreach (Transform tr in selection)
                {
                    m_Console += "Exported " + tr.name + " to " + targetFolder;
                }
            }
            uniDXF();
            EditorUtility.ClearProgressBar();
        }

        catch (Exception ex)
        {
            resetScene();
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
        }
    }

    public void createStruct()
    {
        try
        {
            Directory.CreateDirectory(copyFolder);
            Directory.CreateDirectory(targetFolder);
            Directory.CreateDirectory(batchDir);

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string uni32Temp = "Assets" + Path.DirectorySeparatorChar + "X-Change3D" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "libfbxsdk.mt";
                string uni32Temp2 = "Assets" + Path.DirectorySeparatorChar + "X-Change3D" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "xchange.mt";
                uni32Copy = copyFolder + Path.DirectorySeparatorChar + "libfbxsdk.dll";
                string uni32Copy2 = copyFolder + Path.DirectorySeparatorChar + "xChange.exe";
                FileUtil.CopyFileOrDirectory(uni32Temp, uni32Copy);
                FileUtil.CopyFileOrDirectory(uni32Temp, batchDir + Path.DirectorySeparatorChar + "libfbxsdk.dll");
                FileUtil.CopyFileOrDirectory(uni32Temp2, uni32Copy2);
                FileUtil.CopyFileOrDirectory(uni32Temp2, batchDir + Path.DirectorySeparatorChar + "xBatch.exe");
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string uni32Temp = "Assets" + Path.DirectorySeparatorChar + "X-Change3D" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "libfbxsdkMAC.mt";
                string uni32Temp2 = "Assets" + Path.DirectorySeparatorChar + "X-Change3D" + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "xchangeMac.mt";
                uni32Copy = copyFolder + Path.DirectorySeparatorChar + "libfbxsdk.dylib";
                string uni32Copy2 = copyFolder + Path.DirectorySeparatorChar + "xChange";
                FileUtil.CopyFileOrDirectory(uni32Temp, uni32Copy);
                FileUtil.CopyFileOrDirectory(uni32Temp2, uni32Copy2);
                FileUtil.CopyFileOrDirectory(uni32Temp, batchDir + Path.DirectorySeparatorChar + "libfbxsdk.dylib");
                FileUtil.CopyFileOrDirectory(uni32Temp2, batchDir + Path.DirectorySeparatorChar + "xBatch");
            }
        }
        catch (Exception ex)
        {
            resetScene();
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
        }
    }

    public string MeshToString(Component mf, Dictionary<string, ObjMaterial> materialList)
    {
        Mesh m;
        Material[] mats;

        if (mf is MeshFilter)
        {
            m = (mf as MeshFilter).sharedMesh;
            mats = mf.GetComponent<Renderer>().sharedMaterials;
            meshtypeLabel = "MESH FILTER";
        }

        else if (mf is SkinnedMeshRenderer)
        {
            m = (mf as SkinnedMeshRenderer).sharedMesh;
            mats = (mf as SkinnedMeshRenderer).sharedMaterials;
            meshtypeLabel = "SKINNED MESH";
        }

        else
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("g ").Append(mf.name).Append("\n");

        foreach (Vector3 lv in m.vertices)
        {
            Vector3 wv = mf.transform.TransformPoint(lv);

            sb.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }

        sb.Append("\n");

        foreach (Vector3 lv in m.normals)
        {
            Vector3 wv = mf.transform.TransformDirection(lv);

            sb.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }

        sb.Append("\n");

        foreach (Vector3 v in m.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }

        for (int material = 0; material < m.subMeshCount; material++)
        {
            sb.Append("\n");
            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            sb.Append("usemap ").Append(mats[material].name).Append("\n");

            try
            {
                ObjMaterial objMaterial = new ObjMaterial();
                objMaterial.colors = mats[material].color;
                objMaterial.alpha = mats[material].color.a;
                objMaterial.name = mats[material].name;

                if (mats[material].mainTexture)
                {
                    objMaterial.textureName = AssetDatabase.GetAssetPath(mats[material].mainTexture);
                }
                else
                {
                    objMaterial.textureName = null;
                }

                materialList.Add(objMaterial.name, objMaterial);
            }

            catch (ArgumentException)
            {
                //
            }

            int[] triangles = m.GetTriangles(material);
            vertexSize = m.vertices.Length;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                x_TotalValue = (float)(triangles.Length * 4000) / (float)(x_ProgressValue);
                x_ProgText = "Triangulating Materials from Mesh " + x_TotalValue;
                displayProgress();

                if (recalcNorms)
                    m.RecalculateNormals();

                if (recalcBounds)
                    m.RecalculateBounds();

                sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
    triangles[i] + 1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i + 2] + 1 + uvOffset));
            }
        }
        vertexOffset += m.vertices.Length;
        normalOffset += m.normals.Length;
        uvOffset += m.uv.Length;
        vertexSize += m.vertices.Length;
        EditorUtility.ClearProgressBar();

        return sb.ToString();
    }

    public void MaterialsToFile(Dictionary<string, ObjMaterial> materialList, string folder, string filename)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(folder + Path.DirectorySeparatorChar + filename + ".mtl"))
            {
                foreach (KeyValuePair<string, ObjMaterial> kvp in materialList)
                {
                    string col = kvp.Value.colors.ToString();
                    col = col.Replace("RGBA", "");
                    col = col.Replace("(", "");
                    col = col.Replace(")", "");
                    col = col.Replace(",", "");
                    string alph = kvp.Value.alpha.ToString();
                    sw.Write("\r\n");
                    sw.Write("newmtl {0}", kvp.Key);
                    sw.Write("\r\n");
                    sw.Write("Ka 0.6 0.6 0.6");
                    sw.Write("\r\n");
                    sw.Write("Kd {0}\n", col);
                    sw.Write("\r\n");
                    sw.Write("Ks 0.6 0.6 0.6");
                    sw.Write("\r\n");
                    sw.Write("d {0}\n", alph);
                    sw.Write("\r\n");
                    sw.Write("Ns 10.0000");
                    sw.Write("\r\n");
                    sw.Write("illum 1");
                    sw.Write("\r\n");
                    sw.Write("sharpness 60");
                    sw.Write("\r\n");

                    if (kvp.Value.textureName != null)
                    {
                        hasTexture = true;
                        string textureFile = kvp.Value.textureName;
                        int stripIndex = textureFile.LastIndexOf(Path.PathSeparator);
                        if (stripIndex >= 0) textureFile = textureFile.Substring(stripIndex + 1).Trim();

                        destfbx = targetFolder + Path.DirectorySeparatorChar + "FBX";
                        destbatch = targetFolder + Path.DirectorySeparatorChar + "BATCH" + Path.DirectorySeparatorChar + "FBX";
                        destdae = targetFolder + Path.DirectorySeparatorChar + "DAE";
                        destdxf = targetFolder + Path.DirectorySeparatorChar + "DXF";
                        string destFolder = "";

                        string destName = Path.GetFileName(textureFile);

                        string destinationFile = "";

                        if (isobj && !isbat)
                        {
                            destFolder = targetFolder + Path.DirectorySeparatorChar + "OBJ";
                        }

                        if (isobj && isbat)
                        {
                            destFolder = targetFolder + Path.DirectorySeparatorChar + "BATCH" + Path.DirectorySeparatorChar + "OBJ";
                        }

                        if (isfbx && isbat)
                        {
                            destFolder = targetFolder + Path.DirectorySeparatorChar + "BATCH" + Path.DirectorySeparatorChar + "FBX";
                        }

                        if (isfbx && !isbat)
                        {
                            destFolder = targetFolder + Path.DirectorySeparatorChar + "FBX";
                        }

                        if (isdae)
                        {
                            destFolder = targetFolder + Path.DirectorySeparatorChar + "DAE";
                        }

                        if (isdxf)
                        {
                            destFolder = targetFolder + Path.DirectorySeparatorChar + "DXF";
                        }
                        if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                        destinationFile = destFolder + Path.DirectorySeparatorChar + destName;

                        FileUtil.CopyFileOrDirectory(kvp.Value.textureName, destinationFile);
                        sw.Write("map_Kd {0}", destName);
                    }

                    else if (kvp.Value.textureName == null)
                    {
                        hasTexture = false;
                        sw.Write("\r\n\r\n\r\n");
                    }
                }
            }
        }

        catch (Exception ex)
        {
            resetScene();
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
        }
    }

    public void MeshesToFile(Component[] mf, string folder, string filename)
    {
        try
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
            using (StreamWriter sw = new StreamWriter(folder + Path.DirectorySeparatorChar + filename + ".obj"))
            {
                sw.Write("mtllib ./" + filename + ".mtl\n");

                for (int i = 0; i < mf.Length; i++)
                {
                    sw.Write(MeshToString(mf[i], materialList));
                }
            }
            MaterialsToFile(materialList, folder, filename);
        }

        catch (Exception ex)
        {
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
        }
    }

    private void MeshToFile(Component mf, string folder, string filename)
    {
        try
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter sw = new StreamWriter(folder + Path.DirectorySeparatorChar + filename + ".obj"))
            {
                sw.Write("mtllib ./" + filename + ".mtl\n");

                sw.Write(MeshToString(mf, materialList));
            }
            MaterialsToFile(materialList, folder, filename);
            uniBatch();
        }

        catch (Exception ex)
        {
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
        }
    }

    public void uniFBX()
    {
        try
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                xProcess.StartInfo.FileName = "xChange.exe";
                xProcess.StartInfo.Arguments = selectionName + ".obj" + " " + selectionName + ".fbx" + " " + ".fbx";
                xProcess.StartInfo.WorkingDirectory = copyFolder;
            }

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string pathname = Path.GetFullPath(copyFolder);
                string args = selectionName + ".obj" + " " + selectionName + ".fbx" + " " + ".fbx";
                xProcess.StartInfo.UseShellExecute = false;
                xProcess.StartInfo.WorkingDirectory = pathname;
                xProcess.StartInfo.FileName = pathname + Path.DirectorySeparatorChar + "./xChange";
                xProcess.StartInfo.Arguments = args;
            }

            xProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            xProcess.StartInfo.CreateNoWindow = true;
            xProcess.Start();

            x_ProgText = "Handshake With FBX SDK. This May Take Several Minutes";
            displayProgress();

            xProcess.WaitForExit();
            xProcess.Close();

            EditorUtility.ClearProgressBar();
            consoleColor = Color.green;
            m_Console = "Export Complete...";

            moveFBX();
        }

        catch (Exception ex)
        {
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            KillProcess();
        }
    }

    public void uniDAE()
    {
        try
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                xProcess.StartInfo.FileName = "xChange.exe";
                xProcess.StartInfo.Arguments = selectionName + ".obj" + " " + selectionName + ".dae" + " " + ".dae";
                xProcess.StartInfo.WorkingDirectory = copyFolder;
            }

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string pathname = Path.GetFullPath(copyFolder);
                string args = selectionName + ".obj" + " " + selectionName + ".dae" + " " + ".dae";
                xProcess.StartInfo.UseShellExecute = false;
                xProcess.StartInfo.WorkingDirectory = pathname;
                xProcess.StartInfo.FileName = pathname + Path.DirectorySeparatorChar + "./xChange";
                xProcess.StartInfo.Arguments = args;
            }

            xProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            xProcess.StartInfo.CreateNoWindow = true;
            xProcess.Start();

            x_ProgText = "Handshake With FBX SDK. This May Take Several Minutes";
            displayProgress();

            xProcess.WaitForExit();
            xProcess.Close();

            EditorUtility.ClearProgressBar();
            consoleColor = Color.green;
            m_Console = "Export Complete...";

            moveDAE();
        }

        catch (Exception ex)
        {
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            KillProcess();
        }
    }

    public void uniDXF()
    {
        try
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                xProcess.StartInfo.FileName = "xChange.exe";
                xProcess.StartInfo.Arguments = selectionName + ".obj" + " " + selectionName + ".dxf" + " " + ".dxf";
                xProcess.StartInfo.WorkingDirectory = copyFolder;
            }

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string pathname = Path.GetFullPath(copyFolder);
                string args = selectionName + ".obj" + " " + selectionName + ".dxf" + " " + ".dxf";
                xProcess.StartInfo.UseShellExecute = false;
                xProcess.StartInfo.WorkingDirectory = pathname;
                xProcess.StartInfo.FileName = pathname + Path.DirectorySeparatorChar + "./xChange";
                xProcess.StartInfo.Arguments = args;
            }

            xProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            xProcess.StartInfo.CreateNoWindow = true;
            xProcess.Start();

            x_ProgText = "Handshake With FBX SDK. This May Take Several Minutes";
            displayProgress();

            xProcess.WaitForExit();
            xProcess.Close();

            EditorUtility.ClearProgressBar();
            consoleColor = Color.green;
            m_Console = "Export Complete...";

            moveDXF();
        }

        catch (Exception ex)
        {
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            KillProcess();
        }
    }


    public void uniBatch()
    {
        try
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                string args = batchnames + ".obj" + " " + @"\FBX\" + batchnames + ".fbx .fbx";
                xProcess.StartInfo.WorkingDirectory = batchDir;
                xProcess.StartInfo.Arguments = args;
                xProcess.StartInfo.FileName = "xBatch.exe";
            }

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string pathname = Path.GetFullPath(batchDir);
                string args = batchnames + ".obj" + " " + "/FBX/" + batchnames + ".fbx .fbx";
                xProcess.StartInfo.UseShellExecute = false;
                xProcess.StartInfo.WorkingDirectory = pathname;
                xProcess.StartInfo.FileName = "X-PORTS/BATCH" + "/" + "./xBatch";
                xProcess.StartInfo.Arguments = args;
            }

            xProcess.StartInfo.CreateNoWindow = true;
            xProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            xProcess.Start();

            x_ProgText = "Handshake With FBX. This May Take Several Minutes";
            displayProgress();

            xProcess.WaitForExit();
            xProcess.Close();

            EditorUtility.ClearProgressBar();
            consoleColor = Color.green;
            m_Console = "Batch Export Complete...";
            selectionParent.localScale = originalScale;
        }

        catch (Exception ex)
        {
            selectionParent.localScale = originalScale;
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
            KillProcess();
        }
    }

    public void moveFBX()
    {
        try
        {
            if (!Directory.Exists(fbxMove)) Directory.CreateDirectory(fbxMove);
            FileUtil.MoveFileOrDirectory("tmp" + Path.DirectorySeparatorChar + selectionName + ".fbx", fbxMove + Path.DirectorySeparatorChar + selectionName + ".fbx");
            OpenInFileBrowser(fbxMove);
            dofbx = false;
            EditorUtility.ClearProgressBar();
        }

        catch (Exception ex)
        {
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
        }
        selectionParent.localScale = originalScale;

        if (saveExternal)
        {
            string saveDate = String.Format("{0:MM-dd-yyyy_hh-mm-ss}", DateTime.Now);
            DirectoryCopy(fbxMove, saveDirectory + Path.DirectorySeparatorChar + fbxMove + "_" + saveDate, true);
        }
    }

    public void moveDAE()
    {
        try
        {
            if (!Directory.Exists(daeMove)) Directory.CreateDirectory(daeMove);
            FileUtil.MoveFileOrDirectory("tmp" + Path.DirectorySeparatorChar + selectionName + ".dae", daeMove + Path.DirectorySeparatorChar + selectionName + ".dae");
            OpenInFileBrowser(daeMove);
            dodae = false;
            EditorUtility.ClearProgressBar();
        }

        catch (Exception ex)
        {
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
        }
        selectionParent.localScale = originalScale;

        if (saveExternal)
        {
            string saveDate = String.Format("{0:MM-dd-yyyy_hh-mm-ss}", DateTime.Now);
            DirectoryCopy(daeMove, saveDirectory + Path.DirectorySeparatorChar + daeMove + "_" + saveDate, true);
        }
    }

    public void moveDXF()
    {
        try
        {
            if (!Directory.Exists(dxfMove)) Directory.CreateDirectory(dxfMove);
            FileUtil.MoveFileOrDirectory("tmp" + Path.DirectorySeparatorChar + selectionName + ".dxf", dxfMove + Path.DirectorySeparatorChar + selectionName + ".dxf");
            OpenInFileBrowser(dxfMove);
            dodxf = false;
            EditorUtility.ClearProgressBar();
        }

        catch (Exception ex)
        {
            resetScene();
            UnityEngine.Debug.LogError("X-Change Error at " + ex);
            consoleColor = Color.red;
            m_Console = "Check Console for Errors";
        }
        selectionParent.localScale = originalScale;

        if (saveExternal)
        {
			string saveDate = String.Format("{0:MM-dd-yyyy_hh-mm-ss}", DateTime.Now);
			//DirectoryCopy(dxfMove, saveDirectory + Path.DirectorySeparatorChar + dxfMove + "_" + selectionName + "__"+ saveDate, true);
			DirectoryCopy(dxfMove, saveDirectory + Path.DirectorySeparatorChar + dxfMove + "_" + selectionParent.name, true);
        }
    }

    public void resetScene()
    {
        vertexOffset = 0;
        normalOffset = 0;
        uvOffset = 0;
        isobj = false;
        isfbx = false;
        isdae = false;
        isdxf = false;
        doobj = false;
        dofbx = false;
        dodxf = false;
        hasTexture = false;
        showLegacy = true;
        showBatch = false;
        showOBJ = false;
        showFBX = false;
        showDAE = false;
        showDXF = false;
        showOBJBatch = false;
        showFBXBatch = false;
        meshtypeLabel = "Thinking...";
        legacyColor = Color.green;
        batchColor = Color.white;
        destfbx = null;
        destdae = null;
        destdxf = null;
        destbatch = null;
        isOptions = false;
        isLegacy = true;
        isBatch = false;
        EditorUtility.ClearProgressBar();
        x_ProgressValue = 0;
        if (Directory.Exists(targetFolder) || Directory.Exists(copyFolder))
        {
            FileUtil.DeleteFileOrDirectory(targetFolder);
            FileUtil.DeleteFileOrDirectory(copyFolder);
            createStruct();
        }
    }

    void OpenInFileBrowser(string path)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            string winPath = path.Replace("/", @"\");
            System.Diagnostics.Process.Start("explorer.exe", "/select," + winPath);
        }

        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            string macPath = path.Replace(@"\", "/");
            System.Diagnostics.Process.Start("open", macPath);
        }
    }

    public void displayProgress()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        EditorUtility.DisplayProgressBar("Working...", x_ProgText, Mathf.InverseLerp(0, x_TotalValue, ++x_ProgressValue));
    }

    void KillProcess()
    {
        Process[] running = Process.GetProcesses();
        foreach (Process process in running)
        {
            try
            {
                if (!process.HasExited && process.ProcessName == "xChange")
                {
                    process.Kill();
                    process.WaitForExit(1000);
                }
            }

            catch (InvalidOperationException ex)
            {
                UnityEngine.Debug.LogError("X-Change Process() Exception at " + ex);
            }
        }
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
        }
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }
        FileInfo[] files = dir.GetFiles();

        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

}