using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BIMSWebAPI
{
    public class BIMSDb
    {
        private int scalarResult;

        public static string MySQLServerHost;
        public static string MySQLPort;
        public static string MySQLUsername;
        public static string MySQLPassword;
        public static string MySQLDatabase;

        private MySqlConnection connect;
        private MySqlCommand command;
        private MySqlDataReader reader;
        private List<string> parameters;
        private DataTable Table;
        private string squery;
        private int affected_rows;


        public bool IsConnected = false;

        bool isConnected = false;

        public BIMSDb()
        {
            Connect();
            Table = Table = new DataTable();
            parameters = new List<string>();
        }

        void Connect()
        {

            MySQLServerHost = "localhost";
            MySQLPort = "3306";
            MySQLDatabase = "bimsdb";
            MySQLUsername = "root";
            MySQLPassword = "";

            //MySQLServerHost = "148.72.232.174";

            //MySQLPort = "3306";
            //MySQLDatabase = "webcais";
            //MySQLUsername = "webcais";
            //MySQLPassword = "webcais123";
            try
            {
                string connectionString = "Server=" + MySQLServerHost + "; Port=" + MySQLPort + "; Database=" +
                    MySQLDatabase + "; user id=" + MySQLUsername + "; password=" + MySQLPassword + ";";
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                connect = connection;
                isConnected = true;
                IsConnected = true;
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.ToString());
                IsConnected = false;
            }
        }

        public void CloseConnection()
        {
            if (isConnected == true)
            {
                isConnected = false;
                connect.Close();
                connect.Dispose();
                IsConnected = false;
            }
        }

        private void Init(string Query, string[] bindings = null)
        {
            //create tayo ng connection kung wala pang existing
            if (isConnected == false)
            {
                Connect();
            }

            using (command = new MySqlCommand(Query, connect))
            {
                if (parameters.Count > 0)
                {
                    parameters.ForEach(delegate (string parameter)
                    {
                        string[] sparameters = parameter.ToString().Split('\x7F');
                        command.Parameters.AddWithValue(sparameters[0], sparameters[1]);
                    });
                }
                this.squery = Query.ToLower();
                string firstWord = squery.Split(' ')[0];
                if (firstWord == "select")
                {
                    this.Table = executeDatatable();
                }
                if (firstWord == "delete" || firstWord == "update" || firstWord == "insert")
                {
                    this.affected_rows = execNonquery();
                }
                /*
                if (squery.Contains("select"))
                {
                    this.Table = executeDatatable();
                }
                if (squery.Contains("delete") || squery.Contains("update") || squery.Contains("insert"))
                {
                    this.affected_rows = execNonquery();
                }
                 * */
                // clear the parameters string array
                this.parameters.Clear();
            }

        }

        /// <summary>
        /// Mabilisang Bind, Overloaded Command to ng Bind. Binding ng isang value
        /// </summary>
        /// <param name="fields"></param>
        public void bind(string field, string value)
        {
            parameters.Add("@" + field + "\x7F" + value);
        }

        /// <summary>
        /// Binding with names na.
        /// </summary>
        /// <param name="fields"></param>
        public void bind(string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    bind(fields[i], fields[i + 1]);
                    i += 1;
                }
            }
        }

        /// <summary>
        /// Mabilisang Bind Pag Maraming Parameters starting sa @1
        /// </summary>
        /// <param name="fields"></param>
        public void QuickBind(string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    int s = i + 1;
                    bind(s.ToString(), fields[i]);
                }
            }
        }


        //for select query
        private DataTable executeDatatable()
        {
            DataTable dT = new DataTable();
            try
            {
                reader = command.ExecuteReader();
                dT.Load(reader);
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            return dT;
        }

        //for Selecting a whole table
        /// <summary>
        /// Pang select ng values sa buong table, recommended lang kung kelangan talagang kunin values sa table
        /// </summary>
        public DataTable SelectTable(string table, string[] bindings = null)
        {
            Init("SELECT * FROM " + table, bindings);
            return this.Table;
        }

        /// <summary>
        /// Regular Select Query, Di Applicable si execute scalar dito
        /// </summary>
        public DataTable Select(string QUERY, string[] bindings = null)
        {
            Init(QUERY, bindings);
            return this.Table;
        }

        /// <summary>
        /// Select query nagrereturn ng LIST ng column, different sa datatable
        /// </summary>
        public List<string> SelectColumn(string QUERY, string[] bindings = null)
        {
            Init(QUERY, bindings);
            List<string> column = new List<string>();
            for (int index = 0; index < Table.Rows.Count; index++)
            {
                column.Add(Table.Rows[index][0].ToString());
            }
            return column;
        }
        /// <summary>
        /// Select Query na specific lang ang binabato, single value at string ang binabalik
        /// </summary>
        public string SelectSingleValue(string QUERY, string[] bindings = null)
        {
            Init(QUERY, bindings);
            if (Table.Rows.Count > 0)
            {
                string value = Table.Rows[0][0].ToString();
                return value;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Select Query pang select ng isang row, same with execute scalar. NagRereturn ng String Array
        /// </summary>
        public string[] SelectSingleRow(string QUERY, string[] bindings = null)
        {
            Init(QUERY, bindings);
            string[] row = new string[Table.Columns.Count];
            if (Table.Rows.Count > 0)
                for (int i = 0; i++ < Table.Columns.Count; row[i - 1] = Table.Rows[0][i - 1].ToString()) ;

            return row;
        }

        //for insert,update saka delete
        private int execNonquery()
        {

            int affected = 0;

            try
            {
                affected = command.ExecuteNonQuery();
            }
            catch (MySqlException my)
            {
                //MessageBox.Show(my.ToString());
            }

            return affected;
        }

        /// <summary>
        /// Query for Insert,Delete saka Update
        /// </summary>
        public int NonQuery(string QUERY, string[] bindings = null)
        {
            Init(QUERY, bindings);
            return this.affected_rows;
        }

        //2nd initialization ko, for execute scalar to
        private void Init2(string Query, string[] bindings = null)
        {
            //if there is no connection on the server, create a connection
            if (isConnected == false)
            {
                Connect();
            }

            using (command = new MySqlCommand(Query, connect))
            {
                if (parameters.Count > 0)
                {
                    parameters.ForEach(delegate (string parameter)
                    {
                        string[] sparameters = parameter.ToString().Split('\x7F');
                        command.Parameters.AddWithValue(sparameters[0], sparameters[1]);
                    });
                }
                this.scalarResult = Convert.ToInt32(command.ExecuteScalar());
                this.parameters.Clear();
            }

        }

        /// <summary>
        /// Query ko for execute scalar. Selfbased to usually ginagamit  sa double/triple or quadruple entry na query.
        /// </summary>
        public int ExecuteScalar(string QUERY, string[] bindings = null)
        {
            Init2(QUERY, bindings);
            return this.scalarResult;
        }
    }
    
}