using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevStudios.Ormy
{
    public class OrmyContext
    {
        private readonly DbConnection _connection;
        public string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        public OrmyContext(DbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Insert new record
        /// </summary>
        /// <typeparam name="T">Type of the return output</typeparam>
        /// <param name="tableName">Table name</param>
        /// <param name="inserted">Object with field/value of the new record</param>
        /// <param name="output"> name of output element</param>
        /// <returns>INSERTED.output</returns>
        public T Insert<T>(string tableName,object inserted,string output)
        {
            try
            {
                var t = inserted.GetType();
                var valuesName = new List<string>();
                var values = new List<string>();

                foreach (var property in t.GetProperties())
                {
                    valuesName.Add(property.Name);
                    if (property.GetValue(inserted) is string)
                    {
                        values.Add("N'" + property.GetValue(inserted) + "'");
                    }
                    else if (property.GetValue(inserted) is DateTime)
                    {
                        values.Add("'" + ((DateTime)property.GetValue(inserted)).ToString(DateTimeFormat) + "'");
                    }
                    else if (property.GetValue(inserted) is bool)
                    {
                        var tmpValue = Convert.ToInt32(Convert.ToBoolean(property.GetValue(inserted)));
                        values.Add(tmpValue.ToString());
                    }
                    else
                    {
                        values.Add(property.GetValue(inserted).ToString());
                    }

                }
                var sql = $"INSERT INTO {tableName} ({string.Join(",", valuesName)}) OUTPUT INSERTED.{output} VALUES ({string.Join(",", values)})";

                _connection.Open();
                var cmd = new SqlCommand(sql, _connection as SqlConnection);
                var outputValue = cmd.ExecuteScalar();
                _connection.Close();
                return (T)outputValue;
            }
            catch (Exception)
            {
                _connection.Close();
                throw;
            }
            
        }
        /// <summary>
        /// Delete record based on predicate object
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="predicate">predicate object</param>
        /// <returns>Number of record deleted</returns>
        public int Delete(string tableName, object predicate)
        {
            try
            {
                var t = predicate.GetType();
                var predicateList = new List<string>();

                foreach (var property in t.GetProperties())
                {
                    if (property.GetValue(predicate) is string)
                    {
                        predicateList.Add(property.Name + " = N'" + property.GetValue(predicate) + "'");
                    }
                    else if (property.GetValue(predicate) is DateTime)
                    {
                        predicateList.Add(property.Name + " = '" + ((DateTime)property.GetValue(predicate)).ToString(DateTimeFormat) + "'");
                    }
                    else if (property.GetValue(predicate) is bool)
                    {
                        var tmpValue = Convert.ToInt32(Convert.ToBoolean(property.GetValue(predicate)));
                        predicateList.Add(property.Name +"="+  tmpValue.ToString());
                    }
                    else
                    {
                        predicateList.Add(property.Name + " = " +property.GetValue(predicate).ToString());
                    }

                }
                var sql = $"DELETE FROM {tableName} WHERE   {string.Join(" AND ", predicateList)}";

                _connection.Open();
                var cmd = new SqlCommand(sql, _connection as SqlConnection);
                var result = cmd.ExecuteNonQuery();
                _connection.Close();
                return result;
            }
            catch (Exception)
            {
                _connection.Close();
                throw;
            }
        }
    }
}
