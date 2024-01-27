using MySql.Data.MySqlClient;
using System.Text;

namespace Food.Common
{
    public static class CommonFunctions
    {

        public static T? ExecuteQuery<T>(string connectionString, string query) where T : new()
        {
            T item = new();
            try
            {
                MySqlConnection connection = new(connectionString);
                connection.Open();
                MySqlCommand command = new(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        T listItem = new();
                        listItem.GetType().GetProperties().ToList().ForEach(prop =>
                        {
                            if (reader[prop.Name] != null)
                            {
                                object? value = prop.PropertyType == typeof(string) ? Convert.ToString(reader[prop.Name])
                                : prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?) ? (long)Convert.ToDecimal(reader[prop.Name])
                                : prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?) ? Convert.ToDecimal(reader[prop.Name])
                                : prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?) ? Convert.ToDateTime(reader[prop.Name])
                                : prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?) ? Convert.ToInt32(reader[prop.Name])
                                : prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?) ? Convert.ToString(reader[prop.Name]) == "1"
                                : reader[prop.Name];
                                prop.SetValue(listItem, value);
                            }
                        });
                        item = listItem;
                        break;
                    }
                    reader.Close();
                    command.Dispose();
                    connection.Close();
                    return item;
                }
                else
                {
                    reader.Close();
                    command.Dispose();
                    connection.Close();
                    return default;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<T> ExecuteQueryResult<T>(string connectionString, string query) where T : new()
        {
            List<T> list = new();
            try
            {
                MySqlConnection connection = new(connectionString);
                connection.Open();
                MySqlCommand command = new(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    T listItem = new();
                    listItem.GetType().GetProperties().ToList().ForEach(prop =>
                    {
                        if (reader[prop.Name] != null)
                        {
                            object? value = prop.PropertyType == typeof(string) ? Convert.ToString(reader[prop.Name])
                            : prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?) ? (long)Convert.ToDecimal(reader[prop.Name])
                            : prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?) ? Convert.ToDecimal(reader[prop.Name])
                            : prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?) ? Convert.ToDateTime(reader[prop.Name])
                            : prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?) ? Convert.ToInt32(reader[prop.Name])
                            : prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?) ? Convert.ToString(reader[prop.Name]) == "true"
                            : reader[prop.Name];
                            prop.SetValue(listItem, value);
                        }
                    });
                    list.Add(listItem);
                }

                reader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        public static string Encrypt(this string text)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(text));
        }

        public static string Decrypt(this string text)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(text));
        }
    }
}
