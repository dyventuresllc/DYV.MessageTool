using DYV.MessageTool.EventHandlers.Helpers;
using DYV.MessageTool.EventHandlers.References;
using kCura.EventHandler;
using Relativity.API;
using System;
using System.Collections.Generic;

namespace DYV.MessageTool.EventHandlers
{
    [kCura.EventHandler.CustomAttributes.Description("Creates and manages the Message Board page")]
    [System.Runtime.InteropServices.Guid("928eaf23-69d6-43a5-8992-6293ca001185")]
    public class CEH_MessageTool : ConsoleEventHandler
    {
        private IAPILog logger;
        public override FieldCollection RequiredFields
        {
            get
            {
                FieldCollection retval = new FieldCollection();
                return retval;
            }
        }

        public override kCura.EventHandler.Console GetConsole(PageEvent pageEvent)
        {
            kCura.EventHandler.Console returnConsole = new kCura.EventHandler.Console()
            {
                Items = new List<IConsoleItem>()
            };

            returnConsole.Items.Add(new ConsoleButton() { Name = "SendEmailTest", DisplayText = "Email Yourself (Test)", Enabled = true, RaisesPostBack = true });
            returnConsole.Items.Add(new ConsoleSeparator());
            returnConsole.Items.Add(new ConsoleButton() { Name = "SendEmail", DisplayText = "Send Email", Enabled = true, RaisesPostBack = true });

            return returnConsole;
        }

        public override void OnButtonClick(ConsoleButton consoleButton)
        {
            MT_References references = new MT_References();
            IDBContext eddsDbContext;
            DataHandler dataHandler;
            logger = Helper.GetLoggerFactory().GetLogger();

            eddsDbContext = Helper.GetDBContext(-1);
            dataHandler = new DataHandler(eddsDbContext, logger);
            int rows = 0;
            string msgSubject = ActiveArtifact.Fields[references.MessageSubject.ToString()].Value.Value.ToString();
            string msgBody = ActiveArtifact.Fields[references.MessageBody.ToString()].Value.Value.ToString();      

            switch (consoleButton.Name)
            {
                case "SendEmail":
                    try
                    {
                        if ((bool)ActiveArtifact.Fields[references.MessageTo_AllUsersEnabled.ToString()].Value.Value)
                        {              
                            logger.LogInformation("Message Tool - Sending Email to All Enabled Users");
                            rows = dataHandler.InsertEnabledUsersToQueue(ActiveArtifact.ArtifactID, msgSubject, msgBody);
                        }

                        if ((bool)ActiveArtifact.Fields[references.MessageTo_AllUsers.ToString()].Value.Value)
                        {
                            logger.LogInformation("Message Tool - Sending Email to All Users");
                            rows = dataHandler.InsertAllUsersToQueue(ActiveArtifact.ArtifactID, msgSubject, msgBody);
                        }

                        if ((bool)ActiveArtifact.Fields[references.MessageTo_AllUsersActive.ToString()].Value.Value)
                        {
                            logger.LogInformation("Message Tool - Sending Email to Active Users");
                            rows = dataHandler.InsertActiveUsersToQueue(ActiveArtifact.ArtifactID, msgSubject, msgBody);
                        }
                        logger.LogInformation($"Message Tools - Inserted {rows} records into queue");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Message Tool - (error sending email) - detail: {ex.Message}");
                    }
                break;

                case "SendEmailTest":
                    int userArtifactId = Helper.GetAuthenticationManager().UserInfo.ArtifactID;
                    string firstName = Helper.GetAuthenticationManager().UserInfo.FirstName;
                    string userEmail = Helper.GetAuthenticationManager().UserInfo.EmailAddress;
                   
                    try 
                    {
                        rows = dataHandler.InsertTestMessageToQueue(userArtifactId, ActiveArtifact.ArtifactID, firstName, userEmail, msgSubject, msgBody);
                        logger.LogInformation($"Message Tools - Inserted {rows} records into queue");
                    }
                    catch (Exception ex) 
                    {
                        logger.LogError($"Message Tool - (error sending test email) - detail: {ex.Message}");                        
                    }
                break;
            }
        }
    }
}
