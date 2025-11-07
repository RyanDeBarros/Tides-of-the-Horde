using UnityEditor;
using UnityEngine;

public class ClearPlayerPrefsEditor
{
    [MenuItem("Tools/Player Preferences/Clear All")]
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

    [MenuItem("Tools/Persistent Data/Open File Location")]
    public static void OpenPersistentDataFileLocation()
    {
        EditorUtility.RevealInFinder(PlayerDataManager.PATH);
    }


    [MenuItem("Tools/Persistent Data/Reset Level Select Data")]
    private static void ClearPersistentLevelSelectData()
    {
        if (EditorUtility.DisplayDialog(
            title: "Clear All Level Select Data",
            message: "Are you sure you want to delete all Level Select Data?",
            ok: "Yes, delete everything",
            cancel: "Cancel"))
        {
            LevelSelectUI.ResetPersistentData();
            Debug.Log("All Level Select Data have been cleared.");
        }
    }

    [MenuItem("Tools/Persistent Data/Reset Challenge Data")]
    private static void ClearPersistentChallengeData()
    {
        if (EditorUtility.DisplayDialog(
            title: "Clear All PersistentChallengeData",
            message: "Are you sure you want to delete all persistent challenge data?",
            ok: "Yes, delete everything",
            cancel: "Cancel"))
        {
            PersistentChallengeData.Reset();
            PersistentChallengeData.Save();
            Debug.Log("All PersistentChallengeData have been cleared.");
        }
    }
}
