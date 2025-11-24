using UnityEngine;

namespace _Project.Core.Framework.ServiceLocator
{
    public static class ServiceLocatorExtensions {
        public static T OrNull<T> (this T obj) where T : Object => obj ? obj : null;
    }
}