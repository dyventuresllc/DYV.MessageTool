using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DYV.MessageTool.Agent.Models;
using Relativity.API;

namespace DYV.MessageTool.Agent.Helpers
{
    public class QueueHandler
    {
        IDBContext EddsDbContext { get; set; } 
      
        public QueueHandler(IDBContext eddsDbContext)
        {
            EddsDbContext = eddsDbContext;
        }

        public MessageToolQueueModel NextJobInQueue(int agentID)
        {
            MessageToolQueueModel record = null;

            List<SqlParameter> parameters = new List<SqlParameter>() 
            { 
                new SqlParameter("AgentID", agentID)
            };

            DataTable dt = EddsDbContext.ExecuteSqlStatementAsDataTable(DataAccess.Queries.SELECT.NextItemInQueue, parameters);

            if (dt.Rows.Count ==1)
            {                
                record = Transformers.QueueItem(dt.Rows[0]);
            }

            return record;
        }

        public void CompletedJob(int queueID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            { 
                new SqlParameter("QueueID", queueID)
            };

            EddsDbContext.ExecuteNonQuerySQLStatement(DataAccess.Queries.DELETE.CompletedItemInQueue, parameters);
        }

        public int QueueTotal()
        {
            int total = (int)EddsDbContext.ExecuteSqlStatementAsScalar(DataAccess.Queries.SELECT.QueueTotal);
            return total;
        }
    }
}
