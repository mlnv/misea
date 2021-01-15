using System.Threading.Tasks;

namespace Misea.Actions
{
    interface IAction
    {
        string Name { get; }
        Task PerformAction();
    }
}
