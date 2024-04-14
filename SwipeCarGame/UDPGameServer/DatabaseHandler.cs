using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UDPGameServer
{
    public static class DatabaseHandler
    {
        static readonly string USER_ID = "root";
        static readonly string DB_NAME = "swipeGame";
        static readonly string PASSWORD = "kdy04261004";
        static MySqlConnection connection = null;

        public static void SetMySqlConnection()
        {
            string strConn = $"Server=localhost;Database={DB_NAME};Uid={USER_ID};Pwd={PASSWORD};";
            connection = new MySqlConnection(strConn);
        }

        /* mySQL중 user 테이블에 비번과 아이디가 존재한다면 true를 리턴하는 함수 */
        public static bool TryCheckUser(string id, string password)
        {
            Debug.Assert(connection != null);
            try
            {
                connection.Open();
                string sql = $"SELECT COUNT(*) FROM user WHERE id = {id} AND password = {password}";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
        public static bool CreateUser(string id, string password, string nickName)
        {
            Debug.Assert(connection != null);
            try
            {
                // Check if the user already exists
                if (!TryCheckUser(id, password))
                {
                    // User does not exist, so insert a new user
                    SwipeGame_User newUser = new SwipeGame_User(id, password, nickName); // Assuming you have a User class with properties Id and Password
                    InsertData(newUser); // Insert the new user into the database
                    return true; // Return true indicating that the user was created successfully
                }
                else
                {
                    // User already exists, return false
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static void InsertData<T>(T table)
        {
            Debug.Assert(connection != null);
            try
            {
                connection.Open();
                string tableName = GetTableName<T>();
                StringBuilder columnsBuilder = new StringBuilder();
                StringBuilder valuesBuilder = new StringBuilder();
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                foreach (var property in table.GetType().GetProperties(bindingFlags))
                {
                    columnsBuilder.Append(property.Name);
                    columnsBuilder.Append(", ");

                    var value = property.GetValue(table);
                    if (value != null)
                    {
                        valuesBuilder.Append("\'");
                        valuesBuilder.Append(value.ToString());
                        valuesBuilder.Append("\'");
                    }
                    else
                    {
                        valuesBuilder.Append("NULL");
                    }
                    valuesBuilder.Append(", ");
                }
                // ", " 들 제거
                columnsBuilder.Length -= 2;
                valuesBuilder.Length -= 2;
                string sql = $"INSERT INTO {tableName}({columnsBuilder.ToString()}) VALUES({valuesBuilder.ToString()})";
                Console.WriteLine($"[INPUT] {sql}");
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        private static void SelectData<T>(string condition)
        {
            Debug.Assert(connection != null);
            try
            {
                connection.Open();
                string tableName = GetTableName<T>();
                string sql = $"SELECT * FROM {tableName} WHERE {condition}";
                Console.WriteLine($"[INPUT] {sql}");
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("==========");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"{reader.GetName(i)}: {reader.GetValue(i)}");
                        }
                        Console.WriteLine("==========");
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        private static string GetTableName<T>()
        {
            return typeof(T).ToString().GetTableName();
        }
    }
}
