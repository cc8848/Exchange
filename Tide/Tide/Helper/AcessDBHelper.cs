using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace CommonClass.DBOperator
{
    public class AcessDBHelper
    {
        private string connString;

        public AcessDBHelper(string path = "")
        {
            path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\aibaopen.accdb");
            connString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=True", path);
        }

        public void ExcuteNoQueryDB(string sql)
        {
            try
            {
                using (OleDbConnection con = new OleDbConnection(connString))
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand(sql, con);

                    cmd.ExecuteNonQuery();
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable QueryData(string sql)
        {
            var dt = new DataTable();
            try
            {
                using (var con = new OleDbConnection(connString))
                {
                    con.Open();

                    var cmd = new OleDbCommand(sql, con);

                    var adapter = new OleDbDataAdapter(cmd);

                    adapter.Fill(dt);

                    return dt;
                }
            }

            catch (Exception ex)
            {
                throw ex;
                return dt;
            }
        }

        public string SelectJYSINfo(string name)
        {
            return string.Format("SELECT * FROM {0}", name);
        }

        public string QuerySubUnit(string tablename, string parentid)
        {
            return string.Format("SELECT * FROM {0} WHERE (ParentID = '{1}') ", tablename, parentid);
        }

    }
}
