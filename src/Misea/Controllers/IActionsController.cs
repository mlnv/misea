using System.Threading.Tasks;

namespace Misea.Controllers
{
    public interface IActionsController
    {
        void Initialize();
        Task ParseMessageAndInvokeAction(string message);
        Task PerformUpdate();
    }
}
