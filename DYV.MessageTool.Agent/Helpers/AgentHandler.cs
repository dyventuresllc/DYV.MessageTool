using System;
using kCura.Agent;
using Relativity.API;
using Relativity.Services.Interfaces.Agent;
using Relativity.Services.Interfaces.Agent.Models;
using Relativity.Services.Interfaces.Shared;
using Relativity.Services.Interfaces.Shared.Models;

namespace DYV.MessageTool.Agent.Helpers
{
    public class AgentHandler
    {             
        IServicesMgr SrvcsMgr { get; set; }
        IAPILog Logger { get; set; }

        const int workspaceArtifactId = -1;
        public AgentHandler(IServicesMgr srvcsMgr, IAPILog logger)
        {
            SrvcsMgr = srvcsMgr;
            Logger = logger;
        }

        public void GetAgentTypeIdAndServerId(AgentBase agnt, int agntArtifactId, out int agntTypeId, out int agntServerId, out int agntCurrentInterval, out int agntCurrentLoggingLevel)
        {      
            agntTypeId = 0;
            agntServerId = 0;
            agntCurrentInterval = 0;
            agntCurrentLoggingLevel = 0;    

            using (IAgentManager agntMngr = SrvcsMgr.CreateProxy<IAgentManager>(ExecutionIdentity.System))
            {
                try
                {
                    AgentResponse response = agntMngr.ReadAsync(workspaceArtifactId, agntArtifactId).Result;
                    agntTypeId = response.AgentType.Value.ArtifactID;
                    agntServerId = response.AgentServer.Value.ArtifactID;
                    agntCurrentInterval = decimal.ToInt32(response.Interval);
                    agntCurrentLoggingLevel = response.LoggingLevel;
                }
                catch (Exception ex)
                {
                    string errorMessage = "Error obtaining agent information";
                    Logger.LogError(ex, errorMessage);
                    agnt.RaiseError(errorMessage, ex.ToString());
                }                
            }
        }

        public void UpdateAgentConfiguration(AgentBase agnt, int agentArtifactId, int agentServerId, int agentTypeId)
        {
            AgentRequest agentRequest = new AgentRequest
            {
                Enabled= true,
                Interval = 60,      //setting to 60 seconds
                LoggingLevel = 5,   //setting to 5 log warnings and errors
                AgentType = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = agentTypeId }),
                AgentServer = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = agentServerId })
            };

            using (IAgentManager agntMngr = SrvcsMgr.CreateProxy<IAgentManager>(ExecutionIdentity.System))
            {
                try
                {
                    agntMngr.UpdateAsync(workspaceArtifactId, agentArtifactId, agentRequest).Wait();
                }
                catch (Exception ex) 
                {
                    string errorMessage = "Error updating agent properties";
                    Logger.LogError(ex, errorMessage);
                    agnt.RaiseError(errorMessage, ex.ToString());
                }
            }
        }
    }
}
