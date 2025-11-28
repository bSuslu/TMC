using System;
using _Project.Core.Framework.EventBus;
using TMC._Project.Gameplay.CityMatch.Scripts.Events;
using TMC._Project.Gameplay.Common.ClickSystem;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    public class ItemEntity : MonoBehaviour, IClickable
    {
        [field : SerializeField] public ItemConfig Config { get; private set; }
        [field : SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        public Vector3 Position => transform.position;

        public static event Action<ItemEntity> OnItemClicked;
        
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