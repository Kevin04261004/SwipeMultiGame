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
                string sql = $"SELECT COUNT(*) FROM user WHERE id = \'{id}\' AND password = \'{password}\'";
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
        /* mySQL중 user 테이블에 아이디가 존재한다면 true를 리턴하는 함수 */
        public static bool TryCheckUserID(string id)
        {
            Debug.Assert(connection != null);
            try
            {
                connection.Open();
                string sql = $"SELECT COUNT(*) FROM user WHERE id = \'{id}\'";
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
        /* mySQL중 playerData테이블에 아이디가 존재한다면 true를 리턴하는 함수 */
        public static bool TryCheckPlayerDataID(string id)
        {
            Debug.Assert(connection != null);
            try
            {
                connection.Open();
                string sql = $"SELECT COUNT(*) FROM playerData WHERE id = \'{id}\'";
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
        public static void SaveGameDataToDataBase(string id, float length, out SwipeGame_GamePlayData[] gamePlayDatas)
        {
            SwipeGame_GamePlayData data = new SwipeGame_GamePlayData(id, length, DateTime.Now);
            InsertData(data);
            gamePlayDatas = SortGamePlayData();
            Console.WriteLine("===== 10등 순위 =====");
            for(int i = 0; i < gamePlayDatas.Length;++i)
            {
                Console.WriteLine($"id: {gamePlayDatas[i].Id}, length: {gamePlayDatas[i].Length}");
            }
            Console.WriteLine("===== 끝 !!!!! =====");
             
        }
        public static SwipeGame_PlayerData GetPlayerDataOrNull(string id)
        {
            SwipeGame_PlayerData playerData = null;

            Debug.Assert(connection != null);
            try
            {
                connection.Open();

                string sql = $"SELECT * FROM playerData WHERE id = '{id}'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        playerData = new SwipeGame_PlayerData()
                        {
                            id = reader.GetString("id"),
                            nickName = reader.GetString("nickName"),
                            level = reader.GetInt32("level"),
                            highScore = reader.GetFloat("highScore"),
                            lastConnectTime = reader.GetFloat("lastConnectTime")
                        };
                    }
                }

                return playerData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return playerData;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
        public static bool GetUserNickName(string id, out string nickName)
        {
            Debug.Assert(connection != null);
            try
            {
                connection.Open();
                string sql = $"SELECT nickName FROM playerData WHERE id = \'{id}\'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    nickName = result.ToString();
                }
                else
                {
                    goto return_fail;
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                goto return_fail;
            }
            finally
            {
                connection.Close();
            }
return_fail:
            connection.Close();
            nickName = null;
            return false;
        }
        public static bool CreateUser(string id, string password, string nickName, out SwipeGame_PlayerData playerDataOrNull)
        {
            try
            {
                if (!TryCheckUserID(id))
                {
                    SwipeGame_User newUser = new SwipeGame_User(id, password);
                    InsertData(newUser);
                    SwipeGame_PlayerData newPlayerData = new SwipeGame_PlayerData()
                    {
                        id = id,
                        nickName = nickName,
                        level = 1,
                        highScore = -1,
                        lastConnectTime = -1,
                    };
                    CreatePlayerData(newPlayerData);
                    playerDataOrNull = newPlayerData;
                    return true;
                }
                else
                {
                    playerDataOrNull = null;
                    goto return_fail;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                goto return_fail;
            }
return_fail:
            playerDataOrNull = null;
            return false;
        }
        private static bool CreatePlayerData(SwipeGame_PlayerData playerData)
        {
            Debug.Assert(connection != null);
            Debug.Assert(playerData != null);
            try
            {
                if (!TryCheckPlayerDataID(playerData.id))
                {
                    InsertData(playerData);
                    return true;
                }
                else
                {
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
                        switch(property.PropertyType.ToString())
                        {
                            case "System.DateTime":
                                valuesBuilder.Append(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
                                break;
                            default:
                                valuesBuilder.Append(value.ToString());
                                break;
                        }
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
        private static SwipeGame_GamePlayData[] SortGamePlayData()
        {
            Debug.Assert(connection != null);
            try
            {
                string tableName = "GamePlayData";

                connection.Open();
                string sql = $"SELECT * FROM {tableName} ORDER BY Length ASC LIMIT 10";
                Console.WriteLine($"[INPUT] {sql}");
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                List<SwipeGame_GamePlayData> resultList = new List<SwipeGame_GamePlayData>(); // 결과를 저장할 리스트 생성

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SwipeGame_GamePlayData pd = readDataFromMySQLReader(reader);
                        resultList.Add(pd);
                    }
                }

                connection.Close();

                return resultList.ToArray(); // 리스트를 배열로 변환하여 반환
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
        private static SwipeGame_GamePlayData readDataFromMySQLReader(MySqlDataReader reader)
        {
            string id = reader.GetString("id");
            float length = reader.GetFloat("Length");
            DateTime dateTime = reader.GetDateTime("DateTime");

            return new SwipeGame_GamePlayData(id, length, dateTime);
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
