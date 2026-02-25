using UnityEngine;
using UnityEditor;

//目前有点问题，先不要用
public class AssetDependencyFinder : EditorWindow
{
    private string assetPath = "Assets/YourFolder/Texture.png"; // 目标资源路径

    [MenuItem("Tools/Find Asset Dependencies(Don't use it)")]
    public static void ShowWindow()
    {
        GetWindow<AssetDependencyFinder>("Asset Dependency Finder");
    }

    private void OnGUI()
    {
        assetPath = EditorGUILayout.TextField("Asset Path:", assetPath);

        if (GUILayout.Button("Find Dependencies"))
        {
            FindDependencies(assetPath);
        }
    }

    private void FindDependencies(string assetPath)
    {
        string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);

        GUILayout.Label("Dependencies for " + assetPath + ":");
        foreach (var dep in dependencies)
        {
            GUILayout.Label(dep);
        }
    }
}