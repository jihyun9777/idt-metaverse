// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Data;
// using Mono.Data.Sqlite;
// using System.IO;

// public class DBAccess : MonoBehaviour
// {
//     private string dbname;
//     private string connectionString;

//     public void Start()
//     {
//         dbname = "DataBase.db";
//         string filepath = Path.Combine(Application.streamingAssetsPath, dbname);

//         if (!File.Exists(filepath))
//         {
//             Debug.LogError("Database file not found at path: " + filepath);
//             return;
//         }

//         connectionString = "URI=file:" + filepath;
//         //ReadSpaceData();
//     }

//     public void ReadSpaceData()
//     {
//         try
//         {
//             IDbConnection dbConnection = new SqliteConnection(connectionString);
//             dbConnection.Open();
//             IDbCommand dbCommand = dbConnection.CreateCommand();

//             dbCommand.CommandText = "SELECT * FROM Space";
//             IDataReader dataReader = dbCommand.ExecuteReader();

//             if (!dataReader.Read())
//             {
//                 Debug.LogWarning("No data found in Space table.");
//             }
//             else
//             {
//                 do
//                 {
//                     string name = dataReader.GetString(0);
//                     int x = dataReader.GetInt32(1);
//                     int y = dataReader.GetInt32(2);
//                     Debug.Log("Name: " + name + ", X: " + x + ", Y: " + y);
//                 } while (dataReader.Read());
//             }

//             dataReader.Dispose();
//             dataReader = null;
//             dbCommand.Dispose();
//             dbCommand = null;
//             dbConnection.Close();
//             dbConnection = null;
//         }
//         catch (SqliteException ex)
//         {
//             Debug.LogError("SQLite Exception: " + ex.Message);
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("Exception: " + ex.Message);
//         }
//     }

//     public void InsertSpaceData(string name, int x, int y)
//     {
//         try
//         {
//             IDbConnection dbConnection = new SqliteConnection(connectionString);
//             dbConnection.Open();
//             IDbCommand dbCommand = dbConnection.CreateCommand();

//             dbCommand.CommandText = $"INSERT INTO Space (name, x, y) VALUES ('{name}', {x}, {y})";
//             dbCommand.ExecuteNonQuery();
//             Debug.Log("??");

//             dbCommand.Dispose();
//             dbCommand = null;
//             dbConnection.Close();
//             dbConnection = null;
//         }
//         catch (SqliteException ex)
//         {
//             Debug.LogError("SQLite Exception: " + ex.Message);
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("Exception: " + ex.Message);
//         }
//     }

//     public void DeleteSpaceData(string name)
//     {
//         try
//         {
//             IDbConnection dbConnection = new SqliteConnection(connectionString);
//             dbConnection.Open();
//             IDbCommand dbCommand = dbConnection.CreateCommand();

//             dbCommand.CommandText = $"DELETE FROM Space WHERE name = '{name}'";
//             dbCommand.ExecuteNonQuery();

//             dbCommand.Dispose();
//             dbCommand = null;
//             dbConnection.Close();
//             dbConnection = null;
//         }
//         catch (SqliteException ex)
//         {
//             Debug.LogError("SQLite Exception: " + ex.Message);
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("Exception: " + ex.Message);
//         }
//     }

//     public void SearchData(string name)
//     {
//         try
//         {
//             IDbConnection dbConnection = new SqliteConnection(connectionString);
//             dbConnection.Open();
//             IDbCommand dbCommand = dbConnection.CreateCommand();

//             dbCommand.CommandText = $"SELECT * FROM Space WHERE name = '{name}'";
//             IDataReader dataReader = dbCommand.ExecuteReader();

//             if (!dataReader.Read())
//             {
//                 Debug.LogWarning("No data found with the specified name.");
//             }
//             else
//             {
//                 do
//                 {
//                     string foundName = dataReader.GetString(0);
//                     int x = dataReader.GetInt32(1);
//                     int y = dataReader.GetInt32(2);
//                     Debug.Log("Found - Name: " + foundName + ", X: " + x + ", Y: " + y);
//                 } while (dataReader.Read());
//             }

//             dataReader.Dispose();
//             dataReader = null;
//             dbCommand.Dispose();
//             dbCommand = null;
//             dbConnection.Close();
//             dbConnection = null;
//         }
//         catch (SqliteException ex)
//         {
//             Debug.LogError("SQLite Exception: " + ex.Message);
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("Exception: " + ex.Message);
//         }
//     }

// }


