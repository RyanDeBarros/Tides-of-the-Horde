using UnityEngine;
using UnityEditor;

public static class AddBoxColliderToPrefabs
{
    private const string targetFolder = "Assets/Arena/Prefabs";

    [MenuItem("Tools/Add BoxColliders to Arena Prefabs")]
    public static void AddBoxColliders()
    {
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            EditorUtility.DisplayDialog("Folder not found",
                $"The folder \"{targetFolder}\" does not exist. Make sure the path is correct.",
                "OK");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolder });

        if (guids == null || guids.Length == 0)
        {
            EditorUtility.DisplayDialog("No prefabs found",
                $"No prefabs were found inside \"{targetFolder}\".", "OK");
            return;
        }

        int modified = 0;
        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                EditorUtility.DisplayProgressBar("Adding BoxColliders",
                    $"Processing {assetPath} ({i + 1}/{guids.Length})", (float)i / guids.Length);

                // Load prefab contents for safe editing of the asset
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
                if (prefabRoot == null)
                {
                    Debug.LogWarning($"Could not load prefab at path: {assetPath}");
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                    continue;
                }

                // Check root for existing BoxCollider (do not add duplicates)
                BoxCollider existing = prefabRoot.GetComponent<BoxCollider>();
                if (existing == null)
                {
                    // Add BoxCollider to root of prefab
                    prefabRoot.AddComponent<BoxCollider>();

                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                    modified++;
                    Debug.Log($"Added BoxCollider to prefab: {assetPath}");
                }
                else
                {
                    // already has BoxCollider on root
                    Debug.Log($"Skipped (already has BoxCollider): {assetPath}");
                }

                // Unload the prefab contents to avoid leaked objects
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("Done",
            $"Processed {guids.Length} prefabs.\nAdded BoxCollider to {modified} prefabs.", "OK");
    }
}
