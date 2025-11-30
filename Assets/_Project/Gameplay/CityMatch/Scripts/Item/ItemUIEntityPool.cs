using UnityEngine;
using UnityEngine.Pool;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemUIEntityPool : MonoBehaviour
    {
        [SerializeField] private ItemUIEntity _prefab;
        private IObjectPool<ItemUIEntity> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<ItemUIEntity>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 20, 40);
        }

        private ItemUIEntity CreatePooledItem()
        {
            if (_prefab == null)
            {
                Debug.LogError("ItemUIEntity prefab is not assigned on " + name);
                return null;
            }

            var instance = Instantiate(_prefab, transform);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnTakeFromPool(ItemUIEntity obj)
        {
            if (obj == null) return;
            obj.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(ItemUIEntity obj)
        {
            if (obj == null) return;
            obj.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(ItemUIEntity obj)
        {
            if (obj == null) return;
            Destroy(obj.gameObject);
        }
        
        public ItemUIEntity GetPooledObject() => _pool.Get();
        public void ReturnObject(ItemUIEntity obj) => _pool.Release(obj);
    }
}