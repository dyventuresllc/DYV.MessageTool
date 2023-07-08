
namespace DYV.MessageTool.Agent.Models
{
    public class MessageToolQueueModel
    {
        public int ArtifactID { get; set; }
        public int WorkspaceID { get; set; }
        public int MsgArtifactID { get; set; }
        public string FirstName { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }    
        public int AgentID { get; set; }
    }
}
