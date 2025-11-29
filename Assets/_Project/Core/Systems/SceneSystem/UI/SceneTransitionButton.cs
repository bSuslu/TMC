using _Project.Core.Framework.EventBus;
using _Project.Core.Systems.SceneSystem.Events;
using _Project.Core.Systems.SceneSystem.SO;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Core.Systems.SceneSystem.UI
{
    [RequireComponent(typeof(Button))]
    public class SceneTransitionButton : MonoBehaviour
    {
        [SerializeField] private SceneType _sceneType;
        
        private Button _button;
        
        protected void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        protected void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            EventBus<LoadSceneRequestEvent>.Publish(new LoadSceneRequestEvent(_sceneType));
        }
    }
}