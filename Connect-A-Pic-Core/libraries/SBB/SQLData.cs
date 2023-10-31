using System.Text;
using System.Data;
using System.Data.SQLite;

namespace AtetDataFormats
{
    /// <summary>
    /// Retrieves data using SQL statements and abstracts away the
    /// implementation. Currently, SQLite.
    /// </summary>
    public static class SQLData
    {
        /// <value>The connection to the local database files.</value>
        private static SQLiteConnection _connection = null;
        private static bool _cantConnect = false;

        /// <value>The current local DB file.</value>
        public static string LocalDBFile 
        // TODO: Temp value
        { get; private set; } = "URI=file:../dev.db";

        /// <value>Checks if a DB is connected.</value>
        public static bool IsConnected
        {
            get 
            {
                return (SQLData._connection != null && !SQLData._cantConnect);
            }
        }

        /// <summary>
        /// Sets the path of the local database.
        /// </summary>
        /// <side-effects>
        /// Modifies LocalDBFile.
        /// </side-effects>   
        public static void setLocalDB(String dbName)
        {
            SQLData.LocalDBFile = dbName;
        }

        /// <summary>
        /// Starts a connection with the local database file.
        /// </summary>
        /// <side-effects>
        /// Modifies _connection.
        /// </side-effects>   
        public static void connectLocalDB(bool reconnect = false)
        {
            if ((SQLData._connection == null || reconnect))
            {
                try 
                {
                    SQLData._connection = new SQLiteConnection(SQLData.LocalDBFile);
                    SQLData._connection.Open();
                    SQLData._cantConnect = false;
                }
                catch
                {
                    SQLData._cantConnect = true;
                }
            }
        }

        /// <summary>
        /// Ends the connection with a local DB.
        /// </summary>
        /// <side-effects>
        /// Modifies _connection.
        /// </side-effects>   
        public static void disconnectLocalDB()
        {
            if (SQLData._connection != null)
            {
                SQLData._connection.Close();
                SQLData._cantConnect = false;
                SQLData._connection = null;
            }
        }

        /// <summary>
        /// Runs a Select statement on the local database and returns the table.
        /// </summary>
        /// <side-effects>
        /// Calls connectLocalDB.
        /// Queries a local DB.
        /// </side-effects>   
        /// <returns>
        /// The resulting DataTable.
        /// </returns>   
        public static DataTable executeSelect(String sqlStatement)
        {
            SQLData.connectLocalDB();

            DataTable dataTable = new DataTable();

            if (SQLData._cantConnect)
            {
                return dataTable;
            }

            try 
            {
                SQLiteCommand command = new SQLiteCommand(sqlStatement, SQLData._connection);
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);

                dataAdapter.Fill(dataTable);
            }
            catch
            {
                SQLData._cantConnect = true;
            }

            return dataTable;
        }

        /// <summary>
        /// Import a tables data from a DataTable.
        /// </summary>
        /// <side-effects>
        /// Inserts into a Database
        /// </side-effects>  
        /// <returns>
        /// If a connection could be established.
        /// </returns>   
        public static bool importDataTable(DataTable table, string tableName, string timeColumn = null, long minTime = 0)
        {
            SQLData.connectLocalDB();

            if (SQLData._cantConnect)
            {
                return false;
            }

            using (SQLiteTransaction transaction = SQLData._connection.BeginTransaction())
            {
                StringBuilder insertBuilder = new StringBuilder("INSERT INTO ");
                
                insertBuilder.Append(tableName);
                insertBuilder.Append("(");

                foreach(DataColumn column in table.Columns)
                {
                    insertBuilder.Append(column.ColumnName);
                    insertBuilder.Append(",");
                }
                
                // Remove the last comma
                insertBuilder.Length--;
                insertBuilder.Append(") VALUES (");
                
                foreach (DataRow row in table.Rows)
                {
                    if (timeColumn != null && (long)row[timeColumn] < minTime)
                    {
                        continue;
                    }
                    
                    StringBuilder fullInsert = new StringBuilder(insertBuilder.ToString());
                    
                    foreach(DataColumn column in table.Columns)
                    {
                        fullInsert.Append("'");
                        fullInsert.Append(row[column]);
                        fullInsert.Append("',");
                    }

                    // Remove the last comma
                    fullInsert.Length--;
                    fullInsert.Append(")");

                    using (SQLiteCommand sqlitecommand = new SQLiteCommand(fullInsert.ToString(), SQLData._connection))
                    {
                        sqlitecommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            
            return true;
        }

        /// <summary>
        /// Converts a BASE64 string into a byte array.
        /// </summary>
        /// <returns>
        /// A decoded byte array.
        /// </returns>  
        public static byte[] fromBase64(string encoded)
        {
            byte[] result;
            try
            {
                result = Convert.FromBase64String(encoded);
            }
            catch
            {
                result = new byte[]{};
            }
            return result;
        }

        /// <summary>
        /// Converts a byte array to a BASE64 string.
        /// </summary>
        /// <returns>
        /// A encoded byte array.
        /// </returns>  
        public static string toBase64(byte[] bytes)
        {
            string result;
            try
            {
                result = Convert.ToBase64String(bytes);
            }
            catch
            {
                result = "";
            }
            return result;
        }
    }
}
