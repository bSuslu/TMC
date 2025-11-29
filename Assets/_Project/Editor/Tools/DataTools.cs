#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Project.Editor.Tools
{
    public static class DataTools
    {
        [MenuItem("Data/Delete All Save Data")]
        public static void DeleteAllPersistentData()
        {
            string path = Application.persistentDataPath;

            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true); 
                    Debug.Log($"All data under {path} has been deleted.");
                }
                catch (IOException e)
                {
                    Debug.LogError($"Failed to delete save data: {e.Message}");
                }
            }
            else
            {
                Debug.Log("No save data found.");
            }
            
            // Clear all Player Prefs
            PlayerPrefs.DeleteAll();
            Debug.Log("All Player Prefs have been deleted.");
        }
        
        [MenuItem("Data/Open Save Folder")]
        public static void OpenPersistentDataPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}
#endif