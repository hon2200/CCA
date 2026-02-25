using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ResourceReferenceChecker : EditorWindow
{
    private string folderPath = "Assets/YourFolder";  // Folder path to check
    private List<string> foundReferences = new List<string>();

    [MenuItem("Tools/Check Resource References")]
    public static void ShowWindow()
    {
        GetWindow<ResourceReferenceChecker>("Resource Reference Checker");
    }

    private void OnGUI()
    {
        folderPath = EditorGUILayout.TextField("Folder Path:", folderPath);

        if (GUILayout.Button("Check References"))
        {
            foundReferences.Clear();  // Clear previous references
            CheckReferences(folderPath);  // Check references for the folder
            Repaint();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Referenced Files:");
        foreach (var reference in foundReferences)
        {
            GUILayout.Label(reference);
        }
    }

    private void CheckReferences(string folder)
    {
        // Find all assets in the given folder
        string[] allAssets = AssetDatabase.FindAssets("", new[] { folder });

        foreach (var guid in allAssets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Check if the asset is referenced anywhere in the project
            if (IsReferenced(assetPath))
            {
                foundReferences.Add(assetPath);  // If referenced, add it to the list
            }
        }
    }

    private bool IsReferenced(string assetPath)
    {
        // Get all scenes in the project
        string[] allScenes = AssetDatabase.FindAssets("t:Scene");

        foreach (string sceneGuid in allScenes)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);

            // Get all dependencies of the scene
            string[] dependencies = AssetDatabase.GetDependencies(scenePath);

            // Check if the current assetPath is referenced by any dependency
            foreach (var dep in dependencies)
            {
                if (dep == assetPath)
                {
                    return true;  // Asset is referenced in the scene
                }
            }
        }

        // Add additional checks for Prefabs, Materials, and other dependencies as needed
        return false;
    }
}