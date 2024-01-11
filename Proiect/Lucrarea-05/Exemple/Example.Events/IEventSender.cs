using LanguageExt;
using LanguageExt.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Events
{
    public interface IEventSender
    {
        //TryAsync<Unit> SendAsync<T>(string topicName, T @event);
        Task<Result<Unit>> SendAsync<T>(string topicName, T @event);
    }
}
