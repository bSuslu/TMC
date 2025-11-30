using System;
using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.ServiceLocator;
using TMC._Project.Gameplay.CityMatch.Scripts.Events;
using TMC._Project.Gameplay.Common.Scripts.ClickSystem;
using TMC._Project.Gameplay.Common.Scripts.Enums;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemEntity : MonoBehaviour, IClickable
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        public Vector3 Position => transform.position;

        public IsometricFaceDirection FaceDirection;
        

        public static event Action<ItemEntity> OnItemClicked;

        public ItemConfig Config { get; private set; }

        private void Awake()
        {
            Config = ServiceLocator.Global.Get<ItemSettings>().ItemConfigs[Id];
        }

        public void HandleClick()
        {
            Config.ClickBehaviour.Execute(this);
            EventBus<CollectableItemClickedEvent>.Publish(new CollectableItemClickedEvent(Config.Id));
            OnItemClicked?.Invoke(this);
        }

        public void ClickSuccess()
        {
            Destroy(gameObject);
        }
    }
}