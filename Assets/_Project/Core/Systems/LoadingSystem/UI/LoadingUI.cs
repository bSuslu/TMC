using _Project.Core.Framework.EventBus;
using _Project.Core.Framework.EventBus.Implementations;
using _Project.Core.Systems.LoadingSystem.Events;
using _Project.Core.Systems.SceneSystem.Events;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Core.Systems.LoadingSystem.UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private float _fadeOutDuration = 1f;
        [SerializeField] private Ease _fadeOutEase = Ease.Linear;
        [SerializeField] private float _fadeInDuration = 1f;
        [SerializeField] private Ease _fadeInEase = Ease.Linear;
        [SerializeField] private float _betweenScenesDelay = 1f;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        
        private EventBinding<SceneTransitionStartedEvent> _startBinding;
        private EventBinding<ServicesReadyEvent> _servicesReadyBinding;

        private int _betweenScenesDelayMS;
        private void Awake()
        {
            _startBinding = new EventBinding<SceneTransitionStartedEvent>(OnTransitionStarted);
            _servicesReadyBinding = new EventBinding<ServicesReadyEvent>(OnServicesReady);
            
            EventBus<SceneTransitionStartedEvent>.Subscribe(_startBinding);
            EventBus<ServicesReadyEvent>.Subscribe(_servicesReadyBinding);
            
            // _betweenScenesDelayMS = (int)(_betweenScenesDelay * 1000); // TODO: should wait service unload and load
        }

        private void OnDestroy()
        {
            EventBus<SceneTransitionStartedEvent>.Unsubscribe(_startBinding);
            EventBus<ServicesReadyEvent>.Unsubscribe(_servicesReadyBinding);
        }

        private void OnTransitionStarted()
        {
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, _fadeOutDuration).SetEase(_fadeOutEase).ToUniTask();
        }

        private async void OnServicesReady()
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            await UniTask.Delay(_betweenScenesDelayMS);
            await _canvasGroup.DOFade(0f, _fadeInDuration).SetEase(_fadeInEase).ToUniTask();
            _canvasGroup.gameObject.SetActive(false);
        }
    }
}