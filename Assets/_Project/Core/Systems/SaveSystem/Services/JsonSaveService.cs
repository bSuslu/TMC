using System;
using System.IO;
using Newtonsoft.Json;
using TMC._Project.Core.Systems.SaveSystem.Interfaces;
using UnityEngine;

namespace TMC._Project.Core.Systems.SaveSystem.Services
{
    public class JsonSaveService : ISaveService
    {
        private const string FolderName = "SaveData";
        private string FullFolderPath => Path.Combine(Application.persistentDataPath, FolderName);
        public bool Save<T>(string relativePath, T data, bool overwrite, bool encrypted)
        {
            string path = GetFullPath(relativePath);
            
            try
            {
                if (File.Exists(path))
                {
                    Debug.Log($"Data already exists at path: {path}, creating new file");
                    File.Delete(path);
                }
                else
                {
                    Debug.LogWarning($"Data does not exist at path: {path}, creating new file");
                }
            
                using FileStream fs = File.Create(path);
                fs.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(data));
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }

        public bool TryLoad<T>(string relativePath, out T data, bool encrypted = false)
        {
            string path = GetFullPath(relativePath);

            if (!File.Exists(path))
            {
                data = default;
                return false;
            }

            try
            {
                string json = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TryLoad failed: {e.Message}");
                data = default;
                return false;
            }
        }

        public T Load<T>(string relativePath, bool encrypted)
        {
            string path = GetFullPath(relativePath);

            if (!File.Exists(path))
            {
                Debug.LogError($"File does not exist at path: {path}");
                throw new FileNotFoundException($"File does not exist at path: {path}");
            }

            try
            {
                T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to load data from file: {path} due to: {e.Message} {e.StackTrace}");
                throw;
            }
        }

        public void Delete(string relativePath)
        {
            string path = GetFullPath(relativePath);
            if (File.Exists(path))
            {
                Debug.Log($"Deleting file: {path}");
                File.Delete(path);
            }
            else
            {
                Debug.LogWarning($"File does not exist at path: {path}");
            }
        }

        public void DeleteAll()
        {
            string path = Path.Combine(Application.persistentDataPath,FolderName);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                Debug.Log("No save data found.");
            }
        }
        
        private string GetFullPath(string relativePath)
        {
            if (!Directory.Exists(FullFolderPath))
            {
                Directory.CreateDirectory(FullFolderPath);
            }

            return Path.Combine(Application.persistentDataPath, FolderName, relativePath + ".json");
        }
    }
}