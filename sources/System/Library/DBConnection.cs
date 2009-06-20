//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Xml;

namespace InventIt.SiteSystem.Library
{
    public class DBConnection
    {
        private readonly string _connectionString;

        public DBConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void GetXml(string query, XmlNode xmlNode)
        {
            GetXml(query, xmlNode, "result");
        }

        private void GetXml(string query, XmlNode xmlNode, string tableName)
        {
            GetXml(query, xmlNode, tableName, "row");
        }

        private void GetXml(string query, XmlNode xmlNode, string tableName, string rowName)
        {
            var connection = new OleDbConnection(_connectionString);
            connection.Open();

            var dataSet = new DataSet();
            var dataAdapter = new OleDbDataAdapter(query, connection);
            dataAdapter.Fill(dataSet);

            connection.Close();

            // Better naming
            dataSet.DataSetName = tableName;
            dataSet.Tables[0].TableName = rowName;

            xmlNode.InnerXml = dataSet.GetXml();
        }

        private static string QuoteValue(string value)
        {
            return value.Replace("'", "''");
        }

        public DataTable GetDataTable(string query, params string[] values)
        {
            var quotedValues = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
                quotedValues[i] = QuoteValue(values[i]);

            string newQuery = string.Format(query, quotedValues);

            DataTable dataTable;
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();

                dataTable = new DataTable();
                var dataAdapter = new OleDbDataAdapter(newQuery, connection);
                dataAdapter.Fill(dataTable);

                connection.Close();
            }
            return dataTable;
        }

        private object ExecuteScalar(string query, params string[] values)
        {
            var quotedValues = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
                quotedValues[i] = QuoteValue(values[i]);

            string newQuery = string.Format(query, quotedValues);

            var connection = new OleDbConnection(_connectionString);
            connection.Open();

            var command = new OleDbCommand(newQuery, connection);
            object value = command.ExecuteScalar();

            connection.Close();
            return value;
        }

        private void ExecuteNonQuery(string query)
        {
            var connection = new OleDbConnection(_connectionString);
            connection.Open();

            var command = new OleDbCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public void SaveXml(XmlNode dataNode, string tableName, string primaryKey)
        {
            List<string> columns = GetColumnsFromNode(dataNode);

            foreach (XmlNode childNode in dataNode.ChildNodes)
            {
                // Get values
                Dictionary<string, string> values = GetValuesFromNode(columns, childNode);

                // First, check to see if a record exists
                var recordCount = (int) ExecuteScalar("SELECT COUNT(*) AS RecordCount FROM {0} WHERE {1} = '{2}'",
                                                      tableName, primaryKey, values[primaryKey]);

                if (recordCount > 0)
                    SaveXmlByUpdate(tableName, primaryKey, columns, values);
                else
                    SaveXmlByInsert(tableName, columns, values);
            }
        }

        public string ConvertToSafeString(string txt)
        {
            return txt.Replace("'", "''");
        }

        #region Helper methods for SaveXml

        private List<string> GetColumnsFromNode(XmlNode dataNode)
        {
            var columns = new List<string>();

            // Get list of columns
            if (dataNode.ChildNodes.Count > 0)
            {
                XmlNode firstChild = dataNode.FirstChild;
                foreach (XmlNode rowNode in firstChild.ChildNodes)
                {
                    columns.Add(rowNode.Name);
                }
            }
            return columns;
        }

        private static Dictionary<string, string> GetValuesFromNode(List<string> columns, XmlNode childNode)
        {
            var values = new Dictionary<string, string>();
            foreach (string column in columns)
            {
                string value;

                try
                {
                    value = childNode.SelectSingleNode(column).InnerText;
                }
                catch
                {
                    value = string.Empty;
                }

                values[column] = value;
            }
            return values;
        }

        private void SaveXmlByInsert(string tableName, List<string> columns, Dictionary<string, string> values)
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat("INSERT INTO {0} (", tableName);

            queryBuilder.Append(string.Join(", ", columns.ToArray()));

            queryBuilder.Append(") VALUES (");

            var valueList = new List<string>();
            foreach (string column in columns)
            {
                valueList.Add(string.Format("'{0}'", QuoteValue(values[column])));
            }

            queryBuilder.Append(string.Join(", ", valueList.ToArray()));

            queryBuilder.Append(")");

            ExecuteNonQuery(queryBuilder.ToString());
        }

        private void SaveXmlByUpdate(string tableName, string primaryKey, List<string> columns,
                                     Dictionary<string, string> values)
        {
            // Run an UPDATE command
            var queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat("UPDATE {0} SET ", tableName);

            var setCommands = new List<string>();
            foreach (string column in columns)
            {
                setCommands.Add(string.Format("{0} = '{1}'", column, QuoteValue(values[column])));
            }
            queryBuilder.Append(string.Join(", ", setCommands.ToArray()));

            queryBuilder.AppendFormat(" WHERE {0} = '{1}'", primaryKey, QuoteValue(values[primaryKey]));

            ExecuteNonQuery(queryBuilder.ToString());
        }

        #endregion
    }
}