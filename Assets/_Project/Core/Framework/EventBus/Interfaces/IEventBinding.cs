using System;

namespace _Project.Core.Framework.EventBus.Interfaces
{
    public interface IEventBinding<T> where T : IEvent
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
}