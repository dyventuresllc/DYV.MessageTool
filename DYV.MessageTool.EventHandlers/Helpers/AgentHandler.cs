using Relativity.API;
using System;
using Relativity.HostingBridge.V1.AgentStatusManager;

namespace DYV.MessageTool.EventHandlers.Helpers
{
    public static class AgentHandler
    {
        public static async void StartAgent(IServicesMgr srvcMgr, IAPILog logger)
        {
            using (var agentStatusManager = srvcMgr.CreateProxy<IAgentStatusManagerService>(ExecutionIdentity.System))
            {
                try
                {
                    await agentStatusManager.StartAgentAsync(new Guid("e7f911b3-6b2a-45f7-b828-5906fc8ac90c"));
                    logger.LogInformation("agent kicked off"); 
                }
                catch (Exception ex) 
                {
                    logger.LogError($"Message Tool Agent Issue - detail:{ex.Message}");
                    return;
                }
            }
        }
    }
}
