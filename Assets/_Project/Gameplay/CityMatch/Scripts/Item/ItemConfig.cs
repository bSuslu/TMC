using TMC._Project.Core.Common.Utilities;
using TMC._Project.Gameplay.Common.Scripts.ClickSystem.ClickBehaviours.Base;
using TMC._Project.Gameplay.Common.Scripts.Enums;
using UnityEditor;
using UnityEngine;

namespace TMC._Project.Gameplay.CityMatch.Scripts.Item
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "SO/Item/Config")]
    public class ItemConfig : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public ClickBehaviour ClickBehaviour { get; private set; }
        [field: SerializeField]
        public SerializableDictionary<IsometricFaceDirection, ItemEntity> ItemEntities { get; private set; }

        private void OnValidate()
        {
            foreach (var itemEntity in ItemEntities)
            {
                if (itemEntity.Value == null) continue;
                itemEntity.Value.Id = Id;
                EditorUtility.SetDirty(itemEntity.Value);
            }
            EditorUtility.SetDirty(this);
        }
        
        public ItemEntity GetItemEntity(IsometricFaceDirection faceDirection) => ItemEntities[faceDirection];
    }
}