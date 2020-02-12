using System;
using System.Collections.Generic;
using System.Data.Common;
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
    }
}
