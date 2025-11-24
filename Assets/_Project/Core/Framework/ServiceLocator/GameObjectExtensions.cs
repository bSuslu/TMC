using UnityEngine;

namespace _Project.Core.Framework.ServiceLocator
{
    public static class GameObjectExtensions {
        public static T OrNull<T> (this T obj) where T : Object => obj ? obj : null;
    }
}