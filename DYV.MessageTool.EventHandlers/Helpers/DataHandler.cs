using System.Collections.Generic;
using System.Data.SqlClient;
using Relativity.API;

namespace DYV.MessageTool.EventHandlers.Helpers
{
    public class DataHandler
    {
        IDBContext EddsDbContext { get; set; }
        private IAPILog Logger { get; set; }

        public DataHandler(IDBContext eddsDbContext, IAPILog logger)
        {
            EddsDbContext = eddsDbContext;
            Logger = logger;
        }

        public int InsertTestMessageToQueue(int userArtifactId, int workspaceId, int msgArtifactId, string firstName, string emailAddress, string msgSubject, string msgBody)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("UserArtifactID", userArtifactId));
            parameters.Add(new SqlParameter("WorkspaceID",workspaceId));
            parameters.Add(new SqlParameter("MsgArtifactID", msgArtifactId));
            parameters.Add(new SqlParameter("FirstName", firstName));
            parameters.Add(new SqlParameter("EmailAddress", emailAddress));
            parameters.Add(new SqlParameter("Subject", msgSubject));
            parameters.Add(new SqlParameter("Body", msgBody));

            return EddsDbContext.ExecuteNonQuerySQLStatement(DataAccess.Queries.INSERT.TstUsr_toQueue,parameters);
        }

        public int InsertAllUsersToQueue(int workspaceId, int msgArtifactId, string msgSubject, string msgBody)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("WorkspaceID",workspaceId));
            parameters.Add(new SqlParameter("MsgArtifactID", msgArtifactId));
            parameters.Add(new SqlParameter("Subject", msgSubject));
            parameters.Add(new SqlParameter("Body", msgBody));

            return EddsDbContext.ExecuteNonQuerySQLStatement(DataAccess.Queries.INSERT.Usrs_All_toQueue, parameters);
        }

        public int InsertActiveUsersToQueue(int workspaceId, int msgArtifactId, string msgSubject, string msgBody)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("WorkspaceID",workspaceId));
            parameters.Add(new SqlParameter("MsgArtifactID", msgArtifactId));
            parameters.Add(new SqlParameter("Subject", msgSubject));
            parameters.Add(new SqlParameter("Body", msgBody));

            return EddsDbContext.ExecuteNonQuerySQLStatement(DataAccess.Queries.INSERT.Usrs_Active_toQueue, parameters);
        }

        public int InsertEnabledUsersToQueue(int msgArtifactId, int workspaceId, string msgSubject, string msgBody)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("WorkspaceID", workspaceId));
            parameters.Add(new SqlParameter("MsgArtifactID", msgArtifactId));
            parameters.Add(new SqlParameter("Subject", msgSubject));
            parameters.Add(new SqlParameter("Body", msgBody));

            return EddsDbContext.ExecuteNonQuerySQLStatement(DataAccess.Queries.INSERT.Usrs_Enabled_toQueue, parameters);
        }
    }
}
