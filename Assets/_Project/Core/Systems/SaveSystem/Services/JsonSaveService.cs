using System;
using System.IO;
using System.Threading;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using _Project.Core.Systems.LogSystems;
using _Project.Core.Systems.SaveSystem.Interfaces;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.Core.Systems.SaveSystem.Services
{
    public class JsonSaveService : ISaveService
    {
        private const string k_folderName = "SaveData";
        private readonly string _fullFolderPath;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        public JsonSaveService()
        {
            _fullFolderPath = Path.Combine(Application.persistentDataPath, k_folderName);
        }
        
        public async UniTask InitializeAsync()
        {
            Log.Info("Initializing JsonSaveService");
            
            // SaveData klasörünü oluştur
            await EnsureSaveDirectoryAsync();
            
            Log.Info("JsonSaveService Initialized");
        }

        public async UniTask<bool> SaveAsync<T>(string relativePath, T data, bool overwrite = true, bool encrypted = false)
        {
            string path = GetFullPath(relativePath);
            
            try
            {
                // Overwrite kontrolü
                if (File.Exists(path) && !overwrite)
                {
                    Log.Warning($"File already exists at {path} and overwrite is false");
                    return false;
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                // Async write
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                await File.WriteAllTextAsync(path, json, _cancellationTokenSource.Token);
                
                Log.Info($"Data saved successfully to: {path}");
                return true;
            }
            catch (OperationCanceledException)
            {
                Log.Info("Save operation cancelled");
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Unable to save data due to: {e.Message}");
                return false;
            }
        }

        public async UniTask<(bool success, T data)> TryLoadAsync<T>(string relativePath, bool encrypted = false)
        {
            string path = GetFullPath(relativePath);

            if (!File.Exists(path))
            {
                Log.Warning($"File does not exist at path: {path}");
                return (false, default);
            }

            try
            {
                // Async read
                string json = await File.ReadAllTextAsync(path, _cancellationTokenSource.Token);
                T data = JsonConvert.DeserializeObject<T>(json);
                return (true, data);
            }
            catch (OperationCanceledException)
            {
                Log.Info("Load operation cancelled");
                return (false, default);
            }
            catch (Exception e)
            {
                Log.Error($"TryLoad failed: {e.Message}");
                return (false, default);
            }
        }

        public async UniTask<T> LoadAsync<T>(string relativePath, bool encrypted = false)
        {
            string path = GetFullPath(relativePath);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File does not exist at path: {path}");
            }

            try
            {
                // Async read
                string json = await File.ReadAllTextAsync(path, _cancellationTokenSource.Token);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Load operation was cancelled");
            }
            catch (Exception e)
            {
                Log.Error($"Unable to load data from file: {path} due to: {e.Message}");
                throw;
            }
        }

        public async UniTask DeleteAsync(string relativePath)
        {
            string path = GetFullPath(relativePath);
            
            try
            {
                if (File.Exists(path))
                {
                    await UniTask.RunOnThreadPool(() => File.Delete(path));
                    Log.Info($"File deleted: {path}");
                }
                else
                {
                    Log.Warning($"File does not exist at path: {path}");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Delete failed: {e.Message}");
                throw;
            }
        }

        public async UniTask DeleteAllAsync()
        {
            string path = Path.Combine(Application.persistentDataPath, k_folderName);
            
            try
            {
                if (Directory.Exists(path))
                {
                    await UniTask.RunOnThreadPool(() => Directory.Delete(path, true));
                    Log.Info("All save data deleted");
                }
                else
                {
                    Log.Info("No save data found.");
                }
            }
            catch (Exception e)
            {
                Log.Error($"DeleteAll failed: {e.Message}");
                throw;
            }
        }

        public async UniTask<bool> ExistsAsync(string relativePath)
        {
            string path = GetFullPath(relativePath);
            return await UniTask.RunOnThreadPool(() => File.Exists(path));
        }


        private async UniTask EnsureSaveDirectoryAsync()
        {
            if (!Directory.Exists(_fullFolderPath))
            {
                await UniTask.RunOnThreadPool(() => Directory.CreateDirectory(_fullFolderPath));
            }
        }

        private string GetFullPath(string relativePath)
        {
            return Path.Combine(Application.persistentDataPath, k_folderName, relativePath + ".json");
        }
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}