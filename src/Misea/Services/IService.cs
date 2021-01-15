using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Misea.Services
{
    public interface IService
    {
        event Action<string> MessageReceived;
        Task Start();
        Task Stop();
        Task SendMessage(string text);
        void SetAvailableActions(List<string> actions);
    }
}
