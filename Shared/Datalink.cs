using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BKR.Test.ToscaAPI.Shared
{
    public class Datalink
    {
        public string ConnectionString{get;set;}
        private const int COMMANDTIMEOUT = 900; //kwartier

        public Datalink() { }

        public Datalink(string _connectionString)
        {
            ConnectionString = _connectionString;
        }
        public DataTable GetResultSet(string query)
        {
            DataTable dt = null;
            dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlDataAdapter adpt = new SqlDataAdapter(query, conn))
                {
                    adpt.SelectCommand.CommandTimeout = conn.ConnectionTimeout; ;
                    adpt.Fill(dt);
                }
            }
            return dt;
        }

        public DataRow GetRowFromDb(string query)
        {
            DataTable dt = null;
            DataRow row = null;
            dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlDataAdapter adpt = new SqlDataAdapter(query, conn))
                {
                    adpt.SelectCommand.CommandTimeout = conn.ConnectionTimeout;
                    adpt.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        row = dt.Rows[0];
                    }
                }
            }
            return row;
        }

        public int ExecuteStatement(string query)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.CommandTimeout = COMMANDTIMEOUT;
                    return cmd.ExecuteNonQuery();
                }
            }
            
        }

        public string Execute(string query)
        {
            if (query.Contains("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                DataRow row = GetRowFromDb(query);
                if (row == null)
                    return "-1";
                return row[0].ToString();
            }
            else
            {
                return ExecuteStatement(query).ToString();
            }

        }

        public Boolean IsValidSqlConnectionString()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch { return false; }
        }
    }
}
