using UnityEditor;
using UnityEngine;

public class ClearPlayerPrefsEditor
{
    [MenuItem("Tools/Player Preferences/Clear All Preferences")]
    private static void ClearAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog(
            title: "Clear All PlayerPrefs",
            message: "Are you sure you want to delete all PlayerPrefs data?",
            ok: "Yes, delete everything",
            cancel: "Cancel"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All PlayerPrefs have been cleared.");
        }
    }
}
