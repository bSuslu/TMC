#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Editor.Tools
{
    [InitializeOnLoad]
    public static class SceneSwitcherToolbar
    {
        private static string[] _sceneNames = Array.Empty<string>();
        private static int _selectedIndex = 0;
        private static string _lastActiveScene = "";
        private static VisualElement _toolbarUI;

        private static readonly float _dropdownBoxHeight = 20f; // Dropdown button height

        private static bool FetchAllScenes
        {
            get => EditorPrefs.GetBool("SceneSwitcher_FetchAllScenes", false);
            set => EditorPrefs.SetBool("SceneSwitcher_FetchAllScenes", value);
        }

        static SceneSwitcherToolbar()
        {
            RefreshSceneList();
            SelectCurrentScene();
            
            EditorSceneManager.activeSceneChangedInEditMode += (prev, current) => UpdateSceneSelection();
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            EditorApplication.delayCall += AddToolbarUI;
        }

        static void AddToolbarUI()
        {
            var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            if (toolbarType == null) return;

            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            if (toolbars.Length == 0) return;

            var toolbar = toolbars[0];
            var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rootField == null) return;

            var root = rootField.GetValue(toolbar) as VisualElement;
            if (root == null) return;

            var leftContainer = root.Q("ToolbarZonePlayMode");
            if (leftContainer == null) return;
            
            // Remove old UI if it exists to prevent duplication
            if (_toolbarUI != null)
            {
                leftContainer.Remove(_toolbarUI);
            }

            _toolbarUI = new IMGUIContainer(OnGUI);
            leftContainer.Insert(0, _toolbarUI);
        }

        static void OnGUI()
        {
            CheckAndRefreshScenes();

            if (_selectedIndex >= _sceneNames.Length)
                _selectedIndex = 0;

            bool isPlaying = EditorApplication.isPlaying; // Check if in Play Mode

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(isPlaying);

            // Draw dropdown for the rest
            if (_sceneNames.Length > 1)
            {
                // Dropdown options are all scenes except the first one
                string[] dropdownOptions = _sceneNames.Skip(1).ToArray();
                // The selected index for the dropdown is _selectedIndex-1 (since dropdown starts at index 0 for _sceneNames[1])
                int dropdownIndex = (_selectedIndex <= 0) ? -1 : _selectedIndex - 1;
                int newDropdownIndex = EditorGUILayout.Popup(dropdownIndex, dropdownOptions, GUILayout.Height(_dropdownBoxHeight), GUILayout.Width(120));
                // If selection changed, update _selectedIndex and load scene
                if (newDropdownIndex != dropdownIndex && newDropdownIndex >= 0)
                {
                    _selectedIndex = newDropdownIndex + 1; // Offset by 1 to match _sceneNames index
                    LoadScene(_sceneNames[_selectedIndex]);
                }
            }
            else
            {
                // Only one scene, ensure _selectedIndex is 0
                _selectedIndex = 0;
            }

            // Draw first scene (PersistentScene) as a direct button
            if (_sceneNames.Length > 0)
            {
                if (GUILayout.Button(_sceneNames[0], GUILayout.Height(_dropdownBoxHeight)))
                {
                    _selectedIndex = 0;
                    LoadScene(_sceneNames[0]);
                    // Reset dropdown so same scene can be selected again later
                    _lastActiveScene = _sceneNames[0];
                }
            }

            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        static void RefreshSceneList()
        {
            if (FetchAllScenes)
            {
                _sceneNames = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
                    .Select(path => Path.GetFileNameWithoutExtension(path))
                    .ToArray();
            }
            else
            {
                _sceneNames = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                    .ToArray();
            }
        }

        static void CheckAndRefreshScenes()
        {
            string[] currentScenes;
            if (FetchAllScenes)
            {
                currentScenes = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
                    .Select(path => Path.GetFileNameWithoutExtension(path))
                    .ToArray();
            }
            else
            {
                currentScenes = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                    .ToArray();
            }

            if (!currentScenes.SequenceEqual(_sceneNames))
            {
                _sceneNames = currentScenes;
                SelectCurrentScene();
            }
        }

        static void SelectCurrentScene()
        {
            string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
            int index = System.Array.IndexOf(_sceneNames, currentScene);

            if (index != -1)
            {
                _selectedIndex = index;
                _lastActiveScene = currentScene;
            }
            else
            {
                // Append "(not in build index)" if the scene isn't listed
                string notInBuildName = currentScene + " (not in build index)";

                // Insert it at the beginning or replace first element
                _sceneNames = new[] { notInBuildName }.Concat(_sceneNames).ToArray();
                _selectedIndex = 0;
                _lastActiveScene = currentScene;
            }
        }

        static void UpdateSceneSelection()
        {
            string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
            if (currentScene != _lastActiveScene)
            {
                _lastActiveScene = currentScene;

                // Remove any previous "(not in build index)" label to avoid duplicates
                _sceneNames = _sceneNames.Where(name => !name.EndsWith(" (not in build index)")).ToArray();

                SelectCurrentScene();
            }
        }


        static void LoadScene(string sceneName)
        {
            string scenePath;

            if (FetchAllScenes)
            {
                scenePath = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
                    .FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == sceneName);
            }
            else
            {
                scenePath = EditorBuildSettings.scenes
                    .FirstOrDefault(scene => scene.enabled && scene.path.Contains(sceneName))?.path;
            }

            if (!string.IsNullOrEmpty(scenePath))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }
            else
            {
                Debug.LogError("Scene not found: " + sceneName);
            }
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.delayCall += AddToolbarUI;
            }
        }
    }
}
#endif
