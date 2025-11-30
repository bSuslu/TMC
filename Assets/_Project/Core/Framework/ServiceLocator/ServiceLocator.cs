using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Core.Framework.ServiceLocator.Bootstrappers;
using _Project.Core.Systems.LogSystems;
using UnityEditor;
using UnityEngine;

namespace _Project.Core.Framework.ServiceLocator
{
    /// <summary>
    /// Provides a centralized container for registering and resolving services within the application.
    /// Supports global and scene-specific service locators to manage service lifetimes and scopes.
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _global;
        private static Dictionary<UnityEngine.SceneManagement.Scene, ServiceLocator> _sceneContainers;
        private static List<GameObject> _tmpSceneGameObjects;

        private readonly ServiceManager _services = new ();

        private const string k_globalServiceLocatorName = "ServiceLocator [Global]";
        private const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";

        /// <summary>
        /// Configures this ServiceLocator instance as the global service locator.
        /// Optionally marks the GameObject to not be destroyed on scene load.
        /// </summary>
        /// <param name="dontDestroyOnLoad">If true, the GameObject will persist across scene loads.</param>
        internal void ConfigureAsGlobal(bool dontDestroyOnLoad = true)
        {
            if (_global == this)
            {
                Log.Warning("ServiceLocator.ConfigureAsGlobal: Already configured as global");
            }
            else if (_global != null)
            {
                Log.Error(
                    "ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global");
            }
            else
            {
                _global = this;
                if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Configures this ServiceLocator instance as the service locator for its current scene.
        /// Throws an error if another ServiceLocator is already configured for the scene.
        /// </summary>
        internal void ConfigureForScene()
        {
            UnityEngine.SceneManagement.Scene scene = gameObject.scene;

            if (_sceneContainers.ContainsKey(scene))
            {
                Log.Error(
                    "ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene");
                return;
            }

            _sceneContainers.Add(scene, this);
        }

        /// <summary>
        /// Gets the global ServiceLocator instance. Creates and bootstraps a new one if none exists.
        /// </summary>        
        public static ServiceLocator Global
        {
            get
            {
                if (_global != null) return _global;

                if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return _global;
                }

                var container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

                return _global;
            }
        }

        /// <summary>
        /// Returns the <see cref="ServiceLocator"/> configured for the scene of a given MonoBehaviour.
        /// Falls back to the global ServiceLocator if no scene-specific locator is found.
        /// </summary>
        /// <param name="mb">The MonoBehaviour whose scene is used to find the ServiceLocator.</param>
        /// <returns>The ServiceLocator instance for the scene or the global instance.</returns>
        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            UnityEngine.SceneManagement.Scene scene = mb.gameObject.scene;

            if (_sceneContainers.TryGetValue(scene, out ServiceLocator container) && container != mb)
            {
                return container;
            }

            _tmpSceneGameObjects.Clear();
            scene.GetRootGameObjects(_tmpSceneGameObjects);

            foreach (GameObject go in _tmpSceneGameObjects.Where(go => go.GetComponent<ServiceLocatorSceneBootstrapper>() != null))
            {
                if (go.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) && bootstrapper.Container != mb)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;
        }

        /// <summary>
        /// Gets the closest ServiceLocator instance to the provided MonoBehaviour in its hierarchy,
        /// or the ServiceLocator for its scene, or the global ServiceLocator as a fallback.
        /// </summary>
        /// <param name="mb">The MonoBehaviour to find the ServiceLocator for.</param>
        /// <returns>The closest ServiceLocator instance.</returns>
        public static ServiceLocator For(MonoBehaviour mb)
        {
            return mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;
        }

        /// <summary>
        /// Registers a service instance with the ServiceLocator using the service's type.
        /// </summary>
        /// <typeparam name="T">The class type of the service to be registered.</typeparam>
        /// <param name="service">The service instance to register.</param>
        /// <returns>The registered service instance.</returns>
        public T Register<T>(T service)
        {
            _services.Register(service);
            return service;
        }

        /// <summary>
        /// Registers a service instance with the ServiceLocator using a specific type.
        /// </summary>
        /// <param name="type">The type to register the service as.</param>
        /// <param name="service">The service instance to register.</param>
        /// <returns>The ServiceLocator instance after registering the service.</returns>
        public ServiceLocator Register(Type type, object service)
        {
            _services.Register(type, service);
            return this;
        }

        /// <summary>
        /// Retrieves a service of a specific type. Throws an exception if the service is not found.
        /// </summary>
        /// <typeparam name="T">The class type of the service to retrieve.</typeparam>
        /// <param name="service">The output parameter to receive the service instance.</param>
        /// <returns>The ServiceLocator instance after retrieving the service.</returns>
        /// <exception cref="ArgumentException">Thrown if the service of the specified type is not registered.</exception>
        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if (TryGetNextInHierarchy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
        }

        /// <summary>
        /// Retrieves a service of a specific type. Throws an exception if the service is not found.
        /// </summary>
        /// <typeparam name="T">The class type of the service to retrieve.</typeparam>
        /// <returns>The instance of the requested service.</returns>
        /// <exception cref="ArgumentException">Thrown if the service of the specified type is not registered.</exception>
        public T Get<T>() where T : class
        {
            Type type = typeof(T);
            T service = null;

            if (TryGetService(type, out service)) return service;

            if (TryGetNextInHierarchy(out ServiceLocator container))
                return container.Get<T>();

            throw new ArgumentException($"Could not resolve type '{typeof(T).FullName}'.");
        }

        /// <summary>
        /// Attempts to retrieve a service of a specific type.
        /// </summary>
        /// <typeparam name="T">The class type of the service to retrieve.</typeparam>
        /// <param name="service">The output parameter to receive the service instance.</param>
        /// <returns>True if the service was found; otherwise, false.</returns>
        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);
            service = null;

            if (TryGetService(type, out service))
                return true;

            return TryGetNextInHierarchy(out ServiceLocator container) && container.TryGet(out service);
        }

        bool TryGetService<T>(out T service) where T : class
        {
            return _services.TryGet(out service);
        }

        bool TryGetService<T>(Type type, out T service) where T : class
        {
            return _services.TryGet(out service);
        }

        bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == _global)
            {
                container = null;
                return false;
            }

            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
            return container != null;
        }

        /// <summary>
        /// Called when this ServiceLocator instance is destroyed.
        /// Removes references to this instance from global or scene containers as appropriate.
        /// </summary>
        void OnDestroy()
        {
            if (this == _global)
            {
                _global = null;
            }
            else if (_sceneContainers.ContainsValue(this))
            {
                _sceneContainers.Remove(gameObject.scene);
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _global = null;
            _sceneContainers = new Dictionary<UnityEngine.SceneManagement.Scene, ServiceLocator>();
            _tmpSceneGameObjects = new List<GameObject>();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal()
        {
            var go = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene()
        {
            var go = new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));
        }
#endif
    }
}