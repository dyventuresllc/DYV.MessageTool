using DYV.MessageTool.EventHandlers.References;
using kCura.EventHandler;

namespace DYV.MessageTool.EventHandlers
{
    [kCura.EventHandler.CustomAttributes.Description("Message Tool Pre-Save EH")]
    [System.Runtime.InteropServices.Guid("493A71D1-6263-4D3F-BC9F-EB8F4DB7FA0A")]
    public class PrSEH_MessageTool : PreSaveEventHandler
    {
        public override FieldCollection RequiredFields
        {
            get
            {
                FieldCollection requiredFields = new FieldCollection();
                return requiredFields;
            }
        }

        public override Response Execute()
        {
            kCura.EventHandler.Response retVal = new kCura.EventHandler.Response()
            {
                Success = true,
                Message = string.Empty
            };

            MT_References references = new MT_References();

            int msgToAllUsers = 0;
            int msgToEnabledUsers = 0;
            int msgToActiveUsers = 0;

            if (!ActiveArtifact.Fields[references.MessageTo_AllUsers.ToString()].Value.IsNull)
            {
                msgToAllUsers = (bool)ActiveArtifact.Fields[references.MessageTo_AllUsers.ToString()].Value.Value == true ? 1 : 0;

            }

            if (!ActiveArtifact.Fields[references.MessageTo_AllUsersEnabled.ToString()].Value.IsNull)
            {
                msgToEnabledUsers = (bool)ActiveArtifact.Fields[references.MessageTo_AllUsersEnabled.ToString()].Value.Value == true ? 1 : 0;
            }

            if (!ActiveArtifact.Fields[references.MessageTo_AllUsersActive.ToString()].Value.IsNull)
            {
                msgToActiveUsers = (bool)ActiveArtifact.Fields[references.MessageTo_AllUsersActive.ToString()].Value.Value == true ? 1 : 0;
            }

            if ((msgToAllUsers + msgToActiveUsers + msgToEnabledUsers) != 1)
            {
                retVal.Message = $"You can only select one Audiance selection";
                retVal.Success = false;
            }


            if (ActiveArtifact.Fields[references.MessageSubject.ToString()].Value.IsNull)
            {
                retVal.Message = "You must have a subject";
                retVal.Success = false;
            }

            if (ActiveArtifact.Fields[references.MessageBody.ToString()].Value.IsNull)
            {
                retVal.Message = "You must have a message body";
                retVal.Success = false;
            }

            return retVal;
        }
    }
}
