using System;
using DYV.MessageTool.Agent.Helpers;
using DYV.MessageTool.Agent.Models;
using DYV.MessageTool.EventHandlers.References;
using kCura.Agent;
using Relativity.API;

namespace DYV.MessageTool.Agent
{
    [kCura.Agent.CustomAttributes.Name("Message Tool Worker")]
    [System.Runtime.InteropServices.Guid("E7F911B3-6B2A-45F7-B828-5906FC8AC90C")]
    public class MessageToolWorker : AgentBase
    {
        public override string Name => "Message Tool Agent";

        private IAPILog _logger;
        public override void Execute()
        {
            RaiseMessageNoLogging(string.Empty, 5);
            _logger = Helper.GetLoggerFactory().GetLogger();
            IServicesMgr srvcsMgr;
            srvcsMgr = Helper.GetServicesManager();           
            int agentArtifactId = AgentID;
            MT_References references= new MT_References();

            try
            {
                //int agentTypeID = 0;
                //int agentServerID = 0;
                //int agentCurrentInterval = 0;
                //int agentCurrentLoggingLevel = 0;

                AgentHandler agentHandler = new AgentHandler(srvcsMgr, _logger);
                agentHandler.GetAgentTypeIdAndServerId((this), agentArtifactId, out int agentTypeID, out int agentServerID, out int agentCurrentInterval, out int agentCurrentLoggingLevel);

                if (agentCurrentInterval != 60 && agentCurrentLoggingLevel != 5) //defaults for logging and interval
                {
                    agentHandler.UpdateAgentConfiguration((this),agentArtifactId, agentServerID, agentTypeID);
                }
            }
            catch (Exception ex)
            {
                string errorMessage1 = "Agent configuration error";
                _logger.LogError(ex, errorMessage1);
                RaiseError(ex.ToString(), ex.ToString());
                return;
            }

            IDBContext eddsDbContext = Helper.GetDBContext(-1);
            MessageToolQueueModel queueItem;
            var instanceSettingManager = Helper.GetInstanceSettingBundle();

            string smtpPassword = instanceSettingManager.GetStringAsync("QuinnEmanuel.Notification", "SMTPPassword").Result;
            string emailFromAddress = instanceSettingManager.GetStringAsync("QuinnEmanuel.Notification", "EmailFromAddress").Result;
            string errorMessage;

            try
            {
                QueueHandler queueHandler = new QueueHandler(eddsDbContext);
                int queueTotal = queueHandler.QueueTotal();

                if (queueTotal == 0)
                {
                    return;
                }
                else
                {                    
                    StatusHandler statusHandler = new StatusHandler(srvcsMgr, _logger);
                    var statusTable = statusHandler.MsgStatus();
                    var statusReceipients = statusHandler.MsgReceipients();                    
                    int i = 0;
                    int workspaceID = 0;
                    int MsgID = 0;

                    while (i < queueHandler.QueueTotal())
                    {
                        queueItem = queueHandler.NextJobInQueue(agentArtifactId);
                                                
                        if (i== 0)
                            statusHandler.UpdateJobStatus(references.SendingInProgess, queueItem.WorkspaceID, queueItem.MsgArtifactID).Wait();  //update once
                        
                        workspaceID = queueItem.WorkspaceID;
                        MsgID = queueItem.MsgArtifactID;

                        try
                        {
                            EmailHandler.SendEmail(smtpPassword, emailFromAddress, queueItem.EmailAddress, queueItem.Subject, queueItem.Body, queueItem.FirstName);
                            queueHandler.CompletedJob(queueItem.ArtifactID);
                            statusTable.NewRow();
                            statusTable.Rows.Add(queueItem.MsgArtifactID, DateTime.Now);
                            statusReceipients.Add(queueItem.EmailAddress);
                            i++;
                        }
                        catch (Exception ex)
                        {
                            errorMessage = "Error sending email";
                            _logger.LogError(ex, errorMessage);
                            RaiseError(errorMessage, ex.ToString());
                        }
                    }
                    statusHandler.UpdateJobStatus(references.SendingComplete, workspaceID, MsgID, statusReceipients, statusTable).Wait();
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error during setup and job retrieval";
                _logger.LogError(ex, errorMessage);
                RaiseError(errorMessage, ex.ToString());
                return;
            }
        }
    }
}
