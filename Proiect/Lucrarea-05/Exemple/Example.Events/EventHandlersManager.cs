using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Events
{
    public class EventHandlersManager
    {
        private Dictionary<string, Func<CloudNative.CloudEvents.CloudEvent, Task>> eventHandlers =
        new Dictionary<string, Func<CloudNative.CloudEvents.CloudEvent, Task>>();

        public void RegisterHandler(string eventType, Func<CloudNative.CloudEvents.CloudEvent, Task> handler)
        {
            eventHandlers[eventType] = handler;
        }

        public bool TryGetHandler(string eventType, out Func<CloudNative.CloudEvents.CloudEvent, Task> handler)
        {
            return eventHandlers.TryGetValue(eventType, out handler);
        }
    }
}
