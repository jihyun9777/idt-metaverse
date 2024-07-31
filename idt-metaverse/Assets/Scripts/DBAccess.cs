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

    #region Space DB

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
                        int id = dataReader.GetInt32(0);
                        string name = dataReader.GetString(1);
                        int x = dataReader.GetInt32(2);
                        int y = dataReader.GetInt32(3);
                        Debug.Log("ID: " + id + ", Name: " + name + ", X: " + x + ", Y: " + y);
                    } while (dataReader.Read());
                }
            }
        }

        CloseDB();
    }

    public int InsertSpaceData(string name, int x, int y, byte[] preview)
    {
        OpenDB();

        int lastInsertedId = -1;
        
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = $"INSERT INTO Space (name, x, y, preview) VALUES (@name, @x, @y, @preview)";
            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@x", x));
            dbCommand.Parameters.Add(new SqliteParameter("@y", y));
            dbCommand.Parameters.Add(new SqliteParameter("@preview", preview));
            dbCommand.ExecuteNonQuery();
        }

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT last_insert_rowid()";
            lastInsertedId = (int)(long)dbCommand.ExecuteScalar();
        }

        CloseDB();

        return lastInsertedId;
    }

    public void UpdateSpaceData(string name, int x, int y)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            // Use parameterized queries to prevent SQL injection
            dbCommand.CommandText = "UPDATE Space SET x = @x, y = @y WHERE name = @name";

            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@x", x));
            dbCommand.Parameters.Add(new SqliteParameter("@y", y));

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated. Check if the Space with the given Name exists.");
            }
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

    public SpaceData SearchSpaceData(string name)
    {
        SpaceData spaceData = null;

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
                    spaceData = new SpaceData
                    {
                        ID = dataReader.GetInt32(0),
                        Name = dataReader.GetString(1),
                        X = dataReader.GetInt32(2),
                        Y = dataReader.GetInt32(3),
                        //Preview = dataReader.IsDBNull(4) ? null : (byte[])dataReader[4]
                    };
                }
            }
        }

        CloseDB();

        return spaceData;
    }

    public List<SpaceData> SearchAllSpaces()
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
                        ID = dataReader.GetInt32(0),
                        Name = dataReader.GetString(1),
                        X = dataReader.GetInt32(2),
                        Y = dataReader.GetInt32(3),
                        Preview = dataReader.IsDBNull(4) ? null : (byte[])dataReader[4]
                    };

                    spaces.Add(space);
                }
            }
        }
        
        CloseDB();

        return spaces;
    }

    #endregion

    #region Asset DB

    public List<AssetData> SearchAllAssets(int spaceID)
    {
        List<AssetData> assets = new List<AssetData>();

        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = $"SELECT * FROM Assets WHERE SpaceID = {spaceID}";

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    AssetData asset = new AssetData
                    {
                        SpaceID = dataReader.GetInt32(0),
                        Name = dataReader.GetString(1),
                        X = dataReader.GetInt32(2),
                        Z = dataReader.GetInt32(3),
                        Model = dataReader.GetString(4),
                        Preview = dataReader.IsDBNull(5) ? null : (byte[])dataReader[5]
                    };

                    assets.Add(asset);
                }
            }
        }
        
        CloseDB();

        return assets;
    }

    public void InsertAssetData(int spaceId, string name, int x, int z, byte[] model, byte[] preview)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "INSERT INTO Asset (spaceId, name, x, z, model, preview) VALUES (@spaceId, @name, @x, @z, @model, @preview)";

            dbCommand.Parameters.Add(new SqliteParameter("@spaceId", spaceId));
            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@x", x));
            dbCommand.Parameters.Add(new SqliteParameter("@z", z));
            dbCommand.Parameters.Add(new SqliteParameter("@model", model ?? new byte[0])); 
            dbCommand.Parameters.Add(new SqliteParameter("@preview", preview ?? new byte[0])); 

            dbCommand.ExecuteNonQuery();
        }

        CloseDB();
    }

    public void UpdateAssetData(int spaceId, string name, int x, int z)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            //Use parameterized queries to prevent SQL injection
            dbCommand.CommandText = "UPDATE Asset SET x = @x, z = @z WHERE name = @name AND spaceId = @spaceId";

            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@spaceId", spaceId));
            dbCommand.Parameters.Add(new SqliteParameter("@x", x));
            dbCommand.Parameters.Add(new SqliteParameter("@z", z));

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated. Check if the Asset with the given Name and SpaceID exists.");
            }
        }

        CloseDB();
    }

    #endregion
}


