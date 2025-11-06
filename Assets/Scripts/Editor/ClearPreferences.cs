using UnityEditor;
using UnityEngine;

public class ClearPlayerPrefsEditor
{
    [MenuItem("Tools/Player Preferences/Clear All Preferences")] // Ctrl/Cmd + Shift + D
    private static void ClearAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog(
            "Clear All PlayerPrefs",
            "Are you sure you want to delete all PlayerPrefs data?",
            "Yes, delete everything",
            "Cancel"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All PlayerPrefs have been cleared.");
        }
    }
}
