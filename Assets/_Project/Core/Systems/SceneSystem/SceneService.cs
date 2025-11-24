using System;
using System.Collections.Generic;
using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.EventBus.Implementations;
using _Project.Core.Systems.SceneSystem.Events;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Core.Systems.SceneSystem
{
    public class SceneService : IDisposable
    {
        private readonly GameScene _firstSceneToLoad = GameScene.MainMenuScene;

        private GameScene _currentScene;
        private bool _isLoading;
        private readonly HashSet<string> _cachedSceneNames = new();

        private readonly EventBinding<LoadSceneRequestEvent> _loadSceneRequestEventBinding;
        
        public SceneService()
        {
            CacheBuildScenes();

            _loadSceneRequestEventBinding = new EventBinding<LoadSceneRequestEvent>(OnLoadSceneRequest);
            EventBus<LoadSceneRequestEvent>.Subscribe(_loadSceneRequestEventBinding);

            Initialize().Forget();
        }

        private async UniTask Initialize()
        {
            await LoadFirstScene();
        }

        private async UniTask LoadFirstScene()
        {
            await LoadSceneAsync(_firstSceneToLoad);
        }
        
        private void OnLoadSceneRequest(LoadSceneRequestEvent loadSceneRequestEvent)
        {
            if (loadSceneRequestEvent.Reload)
            {
                ReloadSceneAsync(loadSceneRequestEvent.Scene).Forget();
            }
            else
            {
                LoadScene(loadSceneRequestEvent.Scene);
            }
        }

        private async UniTask ReloadSceneAsync(GameScene sceneKey)
        {
            if (_isLoading)
            {
                Debug.LogWarning($"Another Scene is loading.");
                return;
            }

            EventBus<OnBeforeSceneUnloadEvent>.Publish(new OnBeforeSceneUnloadEvent(sceneKey));
            _isLoading = true;

            string sceneName = GetSceneName(sceneKey);


            EventBus<SceneTransitionStartedEvent>.Publish(new SceneTransitionStartedEvent(sceneKey));
            // unload current scene
            await SceneManager.UnloadSceneAsync(sceneName);

            // load again
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncLoad == null)
            {
                Debug.LogError($"[Scene Error] Failed to reload scene '{sceneName}'.");
                _isLoading = false;

                EventBus<SceneTransitionCompletedEvent>.Publish(new SceneTransitionCompletedEvent(sceneKey));
                return;
            }

            await asyncLoad.ToUniTask();
            _currentScene = sceneKey;
            _isLoading = false;

            EventBus<SceneTransitionCompletedEvent>.Publish(new SceneTransitionCompletedEvent(sceneKey));
        }

        private void LoadScene(GameScene sceneKey)
        {
            if (_isLoading || _currentScene == sceneKey)
            {
                Debug.LogWarning($"Scene '{GetSceneName(sceneKey)}' is already loading or loaded.");
                return;
            }

            EventBus<OnBeforeSceneUnloadEvent>.Publish(new OnBeforeSceneUnloadEvent(sceneKey));

            LoadSceneAsync(sceneKey).Forget();
        }
        
        private async UniTask LoadSceneAsync(GameScene sceneKey)
        {
            _isLoading = true;

            EventBus<SceneTransitionStartedEvent>.Publish(new SceneTransitionStartedEvent(sceneKey));

            string sceneName = GetSceneName(sceneKey);

            await UniTask.Delay(400);
            await CleanupScenes(sceneKey);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncLoad == null)
            {
                Debug.LogError($"[Scene Error] Failed to load scene '{sceneName}'.");
                _isLoading = false;

                EventBus<SceneTransitionCompletedEvent>.Publish(new SceneTransitionCompletedEvent(sceneKey));
                return;
            }

            await asyncLoad.ToUniTask();
            _currentScene = sceneKey;
            _isLoading = false;

            EventBus<SceneTransitionCompletedEvent>.Publish(new SceneTransitionCompletedEvent(sceneKey));
        }

        private async UniTask CleanupScenes(GameScene loadedSceneKey)
        {
            List<string> scenesToUnload = new List<string>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != GetSceneName(GameScene.PersistentScene) && scene.name != GetSceneName(loadedSceneKey))
                {
                    scenesToUnload.Add(scene.name);
                }
            }

            foreach (var sceneName in scenesToUnload)
            {
                await SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        private string GetSceneName(GameScene scene)
        {
            // enum has exact scene name
            string sceneName = scene.ToString();

            if (!IsSceneInBuildSettings(sceneName))
            {
                Debug.LogError(
                    $"[Scene Error] The scene '{sceneName}' mapped from enum '{scene}' is not added to Build Settings or the name is incorrect.");
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