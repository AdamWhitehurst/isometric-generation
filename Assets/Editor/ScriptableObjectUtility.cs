using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{

    private static System.Type ProjectWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ObjectBrowser");

    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    //  Source:
    //  http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
    /// </summary>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        var e = new Event();
        e.keyCode = KeyCode.F2;
        e.type = EventType.KeyDown;
        EditorWindow.GetWindow(ProjectWindowType).SendEvent(e);
    }
}