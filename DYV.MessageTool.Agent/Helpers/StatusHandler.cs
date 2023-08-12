using Relativity.API;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using DYV.MessageTool.EventHandlers.References;
using System.Threading.Tasks;
using System.Linq;

namespace DYV.MessageTool.Agent.Helpers
{
    public class StatusHandler
    {
        internal IServicesMgr ServicesMgr { get; private set; }
        internal MT_References References { get; set; }
        IAPILog Logger { get; set; }

        public StatusHandler(IServicesMgr servicesMgr, IAPILog logger)
        { 
            ServicesMgr = servicesMgr;
            Logger= logger;
            References = new MT_References();
        }


        public DataTable MsgStatus()
        {
            DataTable dt = new DataTable();
            DataColumn dtColumn;

            dtColumn = new DataColumn("MsgArtifactID")
            {
                DataType = typeof(Int32)
            };
            dt.Columns.Add(dtColumn);
            
            dtColumn = new DataColumn("SentDateTime")
            { 
                DataType = Type.GetType("System.DateTime")
            };
            dt.Columns.Add(dtColumn);

            return dt;
        }

        public List<string> MsgReceipients() 
        {
           return  new List<string>();
        }

        public async Task UpdateJobStatus(Guid statusChoice, int workspaceId, int recordId, List<string> MsgRecipients, DataTable MsgTimeSpan)
        {
            double minDuration = 0;
            double secondDuration = 0;

            var StatusDetails = from x in MsgTimeSpan.AsEnumerable()
                         group x by new { x =  x.Field<Int32>("MsgArtifactID") }
                         into y
                         select new
                         {
                             MsgArtifactID = y.Key,
                             MinSentDateTime = (from x2 in y select x2.Field<DateTime>("SentDateTime")).Min(),
                             MaxSentDateTime = (from x2 in y select x2.Field<DateTime>("SentDateTime")).Max()
                         };

            string receipientList = string.Join(", ", MsgRecipients);
             
            foreach (var detail in StatusDetails)
            {
                minDuration = ((detail.MaxSentDateTime - detail.MinSentDateTime).TotalSeconds / 60);
                secondDuration = ((detail.MaxSentDateTime - detail.MinSentDateTime).TotalSeconds % 60);
            }

            string msg = $"<p>Duration {minDuration} minutes, {secondDuration} seconds.</p><hr/><p>Receipient List:<br/>{receipientList}</p>";

            using (IObjectManager objectManager = ServicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
            {
                List<FieldRefValuePair> fieldValues = new List<FieldRefValuePair>
                {
                    new FieldRefValuePair()
                    {
                        Field = new FieldRef()
                        {
                            Guid = References.Status
                        },
                        Value = new ChoiceRef()
                        {
                            Guid = statusChoice
                        }
                    },

                    new FieldRefValuePair()
                    {
                        Field = new FieldRef()
                        {
                            Guid = References.LastSentDate
                        },
                        Value = DateTime.Now
                    },

                    new FieldRefValuePair()
                    {
                        Field = new FieldRef()
                        {
                            Guid = References.StatusDetail
                        },
                        Value = msg
                    }
                };

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
                    Logger.LogError($"Message Tool - (error updating status) - detail: {ex.Message}");
                    return;
                }
            }
        }

        public async Task UpdateJobStatus(Guid statusChoice, int workspaceId, int recordId)
        {
            using (IObjectManager objectManager = ServicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
            {
                List<FieldRefValuePair> fieldValues = new List<FieldRefValuePair>
                {
                    new FieldRefValuePair()
                    {
                        Field = new FieldRef()
                        {
                            Guid = References.Status
                        },
                        Value = new ChoiceRef()
                        {
                            Guid = statusChoice
                        }
                    },

                    new FieldRefValuePair()
                    {
                        Field = new FieldRef()
                        {
                            Guid = References.LastSentDate
                        },
                        Value = DateTime.Now
                    },

                    new FieldRefValuePair()
                    {
                        Field = new FieldRef()
                        {
                            Guid = References.StatusDetail
                        },
                        Value = string.Empty
                    }
                };

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
                    Logger.LogError($"Message Tool - (error updating status) - detail: {ex.Message}");
                    return;
                }
            }
        }
    }
}
