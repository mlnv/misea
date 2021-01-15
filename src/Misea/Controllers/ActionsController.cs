using Misea.Actions;
using Misea.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Misea.Controllers
{
    class ActionsController : IActionsController
    {
        private readonly IService service;

        private IEnumerable<IAction> actions;

        public ActionsController(IEnumerable<IAction> actions, IService service)
        {
            this.actions = actions;
            this.service = service;
        }

        public void Initialize()
        {
            service.SetAvailableActions(actions.Select(a => a.Name).ToList());
        }

        public async Task ParseMessageAndInvokeAction(string message)
        {
            IAction action = actions
                .FirstOrDefault(a => a.Name == message);

            if (action == null)
            {
                return;
            }

            await action.PerformAction();
        }

        public async Task PerformUpdate()
        {
            foreach (var action in actions)
            {
                if (action is IUpdatable actionToUpdate)
                {
                    await actionToUpdate.Update();
                }
            }
        }
    }
}
