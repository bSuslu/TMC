using System;
using System.Collections.Generic;
using _Project.Core.Systems.LogSystems;
using UnityEngine;

namespace _Project.Core.Framework.ServiceLocator
{
    /// <summary>
    /// Manages the registration and retrieval of service instances by their types.
    /// Provides a simple service locator pattern implementation.
    /// </summary>
    public class ServiceManager
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Gets an enumerable collection of all registered service instances.
        /// </summary>
        public IEnumerable<object> RegisteredServices => _services.Values;

        /// <summary>
        /// Attempts to retrieve a registered service of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <param name="service">When this method returns, contains the service instance if found; otherwise, null.</param>
        /// <returns>True if the service is found; otherwise, false.</returns>
        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }

        /// <summary>
        /// Retrieves a registered service of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <returns>The service instance of the specified type.</returns>
        /// <exception cref="ArgumentException">Thrown if the service of the specified type is not registered.</exception>
        public T Get<T>() where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object obj))
            {
                return obj as T;
            }

            throw new ArgumentException($"ServiceManager.Get: Service of type {type.FullName} not registered");
        }

        /// <summary>
        /// Registers a service instance with the manager.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="service">The service instance to register.</param>
        /// <returns>The registered service instance.</returns>
        /// <remarks>
        /// If a service of the same type is already registered, an error is logged and the existing registration is not replaced.
        /// </remarks>
        public T Register<T>(T service)
        {
            Type type = typeof(T);

            if (!_services.TryAdd(type, service))
            {
                Log.Error($"ServiceManager.Register: Service of type {type.FullName} already registered");
            }

            return service;
        }

        /// <summary>
        /// Registers a service instance with the manager using a specified type.
        /// </summary>
        /// <param name="type">The type under which to register the service.</param>
        /// <param name="service">The service instance to register.</param>
        /// <returns>The current <see cref="ServiceManager"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown if the service instance does not match the specified type.</exception>
        /// <remarks>
        /// If a service of the same type is already registered, an error is logged and the existing registration is not replaced.
        /// </remarks>
        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Type of service does not match type of service interface",
                    nameof(service));
            }

            if (!_services.TryAdd(type, service))
            {
                Log.Error($"ServiceManager.Register: Service of type {type.FullName} already registered");
            }

            return this;
        }
    }
}