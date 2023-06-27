using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DYV.MessageTool.Agent.Helpers
{
    public class StatusHandler
    {
        public DataTable MsgStatus()
        {
            DataTable dt = new DataTable();
            DataColumn dtColumn;

            dtColumn = new DataColumn("MsgArtifactID");
            dtColumn.DataType = typeof(Int32);
            //dtColumn.ColumnName = "MsgArtifactID";
            
            dtColumn = new DataColumn("SentDateTime");
            dtColumn.DataType = System.Type.GetType("System.DateTime");
            //dtColumn.ColumnName = "EmailAddress";

            return dt;
        }

        public List<string> MsgReceipients() 
        {
           return  new List<string>();
        }
    }
}
