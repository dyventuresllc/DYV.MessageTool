using System;
using System.Diagnostics;
using DYV.MessageTool.Agent.Helpers;
using DYV.MessageTool.Agent.Models;
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
        private Stopwatch _sw;
        public override void Execute()
        {
            RaiseMessageNoLogging(string.Empty, 5);
            _logger = Helper.GetLoggerFactory().GetLogger();
            IServicesMgr srvcsMgr;
            srvcsMgr = Helper.GetServicesManager();           
            int agentArtifactId = AgentID;

            try
            {
                int agentTypeID = 0;
                int agentServerID = 0;
                int agentCurrentInterval = 0;
                int agentCurrentLoggingLevel = 0;

                AgentHandler agentHandler = new AgentHandler(srvcsMgr, _logger);
                agentHandler.GetAgentTypeIdAndServerId((this), agentArtifactId, out agentTypeID, out agentServerID, out agentCurrentInterval, out agentCurrentLoggingLevel);

                if (agentCurrentInterval != 60 && agentCurrentLoggingLevel != 5) //defaults for logging and interval
                {
                    agentHandler.updateAgentConfiguration((this),agentArtifactId, agentServerID, agentTypeID);
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
                QueueHandler queueHandler = new QueueHandler(eddsDbContext, _logger);
                int queueTotal = queueHandler.QueueTotal();

                if (queueTotal == 0)
                {
                    return;
                }
                else
                {
                    StatusHandler statusHandler = new StatusHandler();
                    var statusTable = statusHandler.MsgStatus();
                    var statusReceipients = statusHandler.MsgReceipients();                    
                    int i = 0;

                    while (i < queueHandler.QueueTotal())
                    {
                        queueItem = queueHandler.NextJobInQueue(agentArtifactId);

                        try
                        {
                            EmailHandler.SendEmail(smtpPassword, emailFromAddress, queueItem.EmailAddress, queueItem.Subject, queueItem.Body);
                            queueHandler.CompletedJob(queueItem.ArtifactID);
                            statusTable.Rows.Add(queueItem.MsgArtifactID, DateTime.UtcNow);
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
                    //update status msgArtifactID
                    //objectmanager to update record job status
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
