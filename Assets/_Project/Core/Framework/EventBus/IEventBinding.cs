using System;

namespace TMC._Project.Core.Framework.EventBus
{
    public interface IEventBinding<T> where T : IEvent
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
}