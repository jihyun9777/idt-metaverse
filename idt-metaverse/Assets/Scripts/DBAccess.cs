using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class DBAccess : MonoBehaviour
{
    private IDbConnection dbConnection;

    public void Start()
    {
        string dbname = "DataBase.db";
        string filepath = Path.Combine(Application.streamingAssetsPath, dbname);

        if (!File.Exists(filepath))
        {
            Debug.LogError("Database file not found at path: " + filepath);
            return;
        }

        string connectionString = "URI=file:" + filepath;
        dbConnection = new SqliteConnection(connectionString);

        ReadSpaceData();
    }

    public void OpenDB()
    {
        if (dbConnection != null && dbConnection.State == ConnectionState.Closed)
        {
            dbConnection.Open();
        }
    }

    public void CloseDB()
    {
        if (dbConnection != null && dbConnection.State == ConnectionState.Open)
        {
            dbConnection.Close();
        }
    }

    public void ReadSpaceData()
    {
        OpenDB();
        
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT * FROM Space";

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                if (!dataReader.Read())
                {
                    Debug.LogWarning("No data found in Space table.");
                }
                else
                {
                    do
                    {
                        string name = dataReader.GetString(0);
                        int x = dataReader.GetInt32(1);
                        int y = dataReader.GetInt32(2);
                        Debug.Log("Name: " + name + ", X: " + x + ", Y: " + y);
                    } while (dataReader.Read());
                }
            }
        }

        CloseDB();
    }

    public void InsertSpaceData(string name, int x, int y)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = $"INSERT INTO Space (name, x, y) VALUES ('{name}', {x}, {y})";
            dbCommand.ExecuteNonQuery();
        }

        CloseDB();
    }

    public void DeleteSpaceData(string name)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = $"DELETE FROM Space WHERE name = '{name}'";
            dbCommand.ExecuteNonQuery();
        }

        CloseDB();
    }

    public void SearchData(string name)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = $"SELECT * FROM Space WHERE name = '{name}'";

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                if (!dataReader.Read())
                {
                    Debug.LogWarning("No data found with the specified name.");
                }
                else
                {
                    do
                    {
                        string foundName = dataReader.GetString(0);
                        int x = dataReader.GetInt32(1);
                        int y = dataReader.GetInt32(2);
                        Debug.Log("Found - Name: " + foundName + ", X: " + x + ", Y: " + y);
                    } while (dataReader.Read());
                }
            }
        }

        CloseDB();
    }

    public List<SpaceData> ReadAllSpaces()
    {
        List<SpaceData> spaces = new List<SpaceData>();

        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT * FROM Space";

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    SpaceData space = new SpaceData
                    {
                        Name = dataReader.GetString(0),
                        X = dataReader.GetInt32(1),
                        Y = dataReader.GetInt32(2),
                        Preview = dataReader.IsDBNull(3) ? null : (byte[])dataReader[3]
                    };

                    spaces.Add(space);
                }
            }
        }
        
        CloseDB();

        return spaces;
    }
}
