using System.Collections.Generic;
using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.EventBus.Implementations;
using _Project.Core.Systems.LoadingSystem.Interfaces;
using _Project.Core.Systems.LogSystems;
using _Project.Core.Systems.SceneSystem.Events;
using _Project.Core.Systems.SceneSystem.SO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Core.Systems.SceneSystem.Services
{
    public class SceneService : IAsyncService
    {
        private readonly SceneType _firstSceneTypeToLoad = SceneType.MainMenu;

        private SceneType _currentSceneType;
        private bool _isLoading;
        private readonly HashSet<string> _cachedSceneNames = new();

        private readonly EventBinding<LoadSceneRequestEvent> _loadSceneRequestEventBinding;
        
        public SceneService()
        {
            CacheBuildScenes();

            _loadSceneRequestEventBinding = new EventBinding<LoadSceneRequestEvent>(OnLoadSceneRequest);
            EventBus<LoadSceneRequestEvent>.Subscribe(_loadSceneRequestEventBinding);
        }
        
        public async UniTask InitializeAsync()
        {
            Log.Info("Initializing SceneService");
            await LoadSceneAsync(_firstSceneTypeToLoad);
            Log.Info("SceneService Initialized");
        }
        
        private void OnLoadSceneRequest(LoadSceneRequestEvent loadSceneRequestEvent)
        {
            if (loadSceneRequestEvent.Reload)
            {
                ReloadSceneAsync(loadSceneRequestEvent.SceneType).Forget();
            }
            else
            {
                LoadScene(loadSceneRequestEvent.SceneType);
            }
        }

        private async UniTask ReloadSceneAsync(SceneType sceneTypeKey)
        {
            if (_isLoading)
            {
                Log.Warning($"Another Scene is loading.");
                return;
            }

            EventBus<OnBeforeSceneUnloadEvent>.Publish(new OnBeforeSceneUnloadEvent(sceneTypeKey));
            _isLoading = true;

            string sceneName = GetSceneName(sceneTypeKey);


            EventBus<SceneTransitionStartedEvent>.Publish(new SceneTransitionStartedEvent(sceneTypeKey));
            // unload current scene
            await SceneManager.UnloadSceneAsync(sceneName);

            // load again
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncLoad == null)
            {
                Log.Error($"[Scene Error] Failed to reload scene '{sceneName}'.");
                _isLoading = false;

                EventBus<SceneTransitionCompletedEvent>.Publish(new SceneTransitionCompletedEvent(sceneTypeKey));
                return;
            }

            await asyncLoad.ToUniTask();
            _currentSceneType = sceneTypeKey;
            _isLoading = false;

            EventBus<SceneTransitionCompletedEvent>.Publish(new SceneTransitionCompletedEvent(sceneTypeKey));
        }

        private void LoadScene(SceneType sceneTypeKey)
        {
            if (_isLoading || _currentSceneType == sceneTypeKey)
            {
                Log.Warning($"Scene '{GetSceneName(sceneTypeKey)}' is already loading or loaded.");
                return;
            }

            EventBus<OnBeforeSceneUnloadEvent>.Publish(new OnBeforeSceneUnloadEvent(sceneTypeKey));

            LoadSceneAsync(sceneTypeKey).Forget();
        }
        
        private async UniTask LoadSceneAsync(SceneType sceneTypeKey)
        {
            _isLoading = true;

            EventBus<SceneTransitionStartedEvent>.Publish(new SceneTransitionStartedEvent(sceneTypeKey));

            string sceneName = GetSceneName(sceneTypeKey);

            await UniTask.Delay(400);
            await CleanupScenes(sceneTypeKey);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncLoad == null)
            {
                Log.Error($"[Scene Error] Failed to load scene '{sceneName}'.");
                _isLoading = false;

                EventBus<SceneTransitionCompletedEvent>.Publish(new SceneTransitionCompletedEvent(sceneTypeKey));
                return;
            }

            await asyncLoad.ToUniTask();
            _currentSceneType = sceneTypeKey;
            _isLoading = false;

            EventBus<SceneTransitionCompletedEvent>.Publish(new SceneTransitionCompletedEvent(sceneTypeKey));
        }

        private async UniTask CleanupScenes(SceneType loadedSceneTypeKey)
        {
            List<string> scenesToUnload = new List<string>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != GetSceneName(SceneType.Persistent) && scene.name != GetSceneName(loadedSceneTypeKey))
                {
                    scenesToUnload.Add(scene.name);
                }
            }

            foreach (var sceneName in scenesToUnload)
            {
                await SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        private string GetSceneName(SceneType sceneType)
        {
            // enum has exact scene name
            string sceneName = sceneType.ToString();

            if (!IsSceneInBuildSettings(sceneName))
            {
                Log.Error(
                    $"[Scene Error] The scene '{sceneName}' mapped from enum '{sceneType}' is not added to Build Settings or the name is incorrect.");
            }

            return sceneName;
        }

        private void CacheBuildScenes()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                _cachedSceneNames.Add(sceneName);
            }
        }

        private bool IsSceneInBuildSettings(string sceneName) => _cachedSceneNames.Contains(sceneName);

        public void Dispose()
        {
            EventBus<LoadSceneRequestEvent>.Unsubscribe(_loadSceneRequestEventBinding);
        }

        
    }
}