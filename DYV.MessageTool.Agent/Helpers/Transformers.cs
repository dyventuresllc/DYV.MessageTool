using DYV.MessageTool.Agent.Models;
using System.Data;

namespace DYV.MessageTool.Agent.Helpers
{
    internal static class Transformers
    {
        internal static MessageToolQueueModel QueueItem(DataRow row)
        {   
            MessageToolQueueModel record = new MessageToolQueueModel();
            
            record.ArtifactID = int.Parse(row[0].ToString());
            record.WorkspaceID= int.Parse(row[1].ToString());
            record.MsgArtifactID = int.Parse(row[2].ToString());
            record.FirstName = row[3].ToString();
            record.EmailAddress = row[4].ToString();
            record.Subject = row[5].ToString();
            record.Body = row[6].ToString();
            record.AgentID = int.Parse(row[7].ToString());

            return record;
        }
    }
}
