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
            _logger = Helper.GetLoggerFactory().GetLogger();
            IServicesMgr srvcsMgr;
            srvcsMgr = Helper.GetServicesManager();           
            int agentArtifactId = AgentID;
            MT_References references= new MT_References();
            IDBContext eddsDbContext = Helper.GetDBContext(-1);
            MessageToolQueueModel queueItem;
            var instanceSettingManager = Helper.GetInstanceSettingBundle();

            string smtpPassword = instanceSettingManager.GetStringAsync("QuinnEmanuel.Notification", "SMTPPassword").Result;
            string emailFromAddress = instanceSettingManager.GetStringAsync("QuinnEmanuel.Notification", "EmailFromAddress").Result;
            _logger.LogInformation("instance setting values obtained");
            string errorMessage;
            int agentLoggingLevel = 10;
            try
            {
                RaiseMessage("Starting.", agentLoggingLevel);
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

                    while (i <= queueHandler.QueueTotal())
                    {
                        queueItem = queueHandler.NextJobInQueue(agentArtifactId);
                                                
                        if (i== 0)
                            statusHandler.UpdateJobStatus(references.SendingInProgess, queueItem.WorkspaceID, queueItem.MsgArtifactID).Wait();  //update once
                        
                        workspaceID = queueItem.WorkspaceID;
                        MsgID = queueItem.MsgArtifactID;

                        try
                        {
                            RaiseMessage("Sending emails", agentLoggingLevel);
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
                    RaiseMessage("Done", agentLoggingLevel);
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
