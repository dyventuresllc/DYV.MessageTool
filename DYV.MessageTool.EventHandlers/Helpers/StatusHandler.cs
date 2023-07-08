using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DYV.MessageTool.EventHandlers.References;
using Relativity.API;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.User;

namespace DYV.MessageTool.EventHandlers.Helpers
{
    public class StatusHandler
    {
        IDBContext EddsDbContext { get; set; }
        private IAPILog Logger { get; set; }
        internal IServicesMgr ServicesMgr { get; private set; }
        internal MT_References References { get; set; }
        public StatusHandler(IServicesMgr servicesMgr, IDBContext eddsDbContext, IAPILog logger)
        {
            EddsDbContext = eddsDbContext;
            Logger = logger;
            ServicesMgr = servicesMgr;
            References = new MT_References();
        }

        public async Task UpdateJobStatus(Guid statusChoice, int workspaceId, int recordId, int userId)
        {
            using (IObjectManager objectManager = ServicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
            {
                List<FieldRefValuePair> fieldValues = new List<FieldRefValuePair>();

                fieldValues.Add(new FieldRefValuePair()
                {
                    Field = new FieldRef()
                    {
                        Guid = References.LastSentBy
                    },
                    Value = new UserRef()
                    {
                        ArtifactID = userId
                    }
                });

                fieldValues.Add(new FieldRefValuePair() 
                { 
                    Field = new FieldRef() 
                    { 
                        Guid = References.LastSentDate 
                    }, Value = DateTime.Now 
                });

                fieldValues.Add(new FieldRefValuePair()
                {
                    Field = new FieldRef()
                    {
                        Guid = References.Status
                    },
                    Value = new ChoiceRef()
                    {
                        Guid = statusChoice
                    }
                });

                UpdateRequest updateRequest = new UpdateRequest()
                {
                    Object = new RelativityObjectRef() { ArtifactID = recordId },
                    FieldValues = fieldValues
                };

                UpdateOptions updateOptions = new UpdateOptions()
                {
                    UpdateBehavior = FieldUpdateBehavior.Replace
                };

                try 
                { 
                    await objectManager.UpdateAsync(workspaceId, updateRequest, updateOptions); 
                }
                catch (Exception ex) 
                {
                    Logger.LogError($"Message Tool - (error updating status fields) - detail: {ex.Message}");
                    return;
                }                   
            }
        }
    }
}
