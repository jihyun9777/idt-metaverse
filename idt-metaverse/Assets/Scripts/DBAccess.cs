using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class DBAccess : MonoBehaviour
{
    private IDbConnection dbConnection;
    private string connectionString;

    public void Start()
    {
        string dbname = "DataBase.db";
        string filepath = Path.Combine(Application.streamingAssetsPath, dbname);

        if (!File.Exists(filepath))
        {
            Debug.LogError("Database file not found at path: " + filepath);
            return;
        }

        connectionString = "URI=file:" + filepath;
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

    public int AddSpaceData(string name, int x, int y, byte[] preview)
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

    public void SetSpaceData(string name, int x, int y)
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

    #endregion

    #region Asset DB

    public List<AssetData> SearchAllAssets(int spaceID)
    {
        List<AssetData> assets = new List<AssetData>();

        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT * FROM Asset WHERE SpaceID = @spaceID";
            dbCommand.Parameters.Add(new SqliteParameter("@spaceID", spaceID));

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    AssetData asset = new AssetData
                    {
                        SpaceID = dataReader.GetInt32(0),
                        Name = dataReader.GetString(1),
                        X = dataReader.IsDBNull(2) ? (float?)null : dataReader.GetFloat(2),
                        Z = dataReader.IsDBNull(3) ? (float?)null : dataReader.GetFloat(3),
                        Scale = dataReader.IsDBNull(4) ? (float?)null : dataReader.GetFloat(4),
                        Model = dataReader.GetString(5),
                        Preview = dataReader.IsDBNull(6) ? null : (byte[])dataReader[6]
                    };

                    assets.Add(asset);
                }
            }
        }

        CloseDB();

        return assets;
    }

    public AssetData SearchAsset(int spaceId, string name)
    {
        AssetData assetData = null;

        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT * FROM Asset WHERE SpaceID = @spaceID AND Name = @name";

            dbCommand.Parameters.Add(new SqliteParameter("@spaceID", spaceId));
            dbCommand.Parameters.Add(new SqliteParameter("@name", name));

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                if (dataReader.Read())
                {
                    assetData = new AssetData
                    {
                        SpaceID = dataReader.GetInt32(0),
                        Name = dataReader.GetString(1),
                        X = dataReader.IsDBNull(2) ? (float?)null : dataReader.GetFloat(2),
                        Z = dataReader.IsDBNull(3) ? (float?)null : dataReader.GetFloat(3),
                        Scale = dataReader.IsDBNull(4) ? (float?)null : dataReader.GetFloat(4),
                        Model = dataReader.GetString(5),
                        Preview = dataReader.IsDBNull(6) ? null : (byte[])dataReader[6]
                    };
                }
                else
                {
                    Debug.LogWarning("No asset found with the specified ID and Name.");
                }
            }
        }

        CloseDB();

        return assetData;
    }

    public void AddAssetData(int spaceId, string name, float? x, float? z, float? scale, string model, byte[] preview)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "INSERT INTO Asset (spaceId, name, x, z, scale, model, preview) VALUES (@spaceId, @name, @x, @z, @scale, @model, @preview)";

            dbCommand.Parameters.Add(new SqliteParameter("@spaceId", spaceId));
            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@x", x.HasValue ? (object)x.Value : DBNull.Value));
            dbCommand.Parameters.Add(new SqliteParameter("@z", z.HasValue ? (object)z.Value : DBNull.Value));
            dbCommand.Parameters.Add(new SqliteParameter("@scale", scale.HasValue ? (object)scale.Value : DBNull.Value));
            dbCommand.Parameters.Add(new SqliteParameter("@model", model));
            dbCommand.Parameters.Add(new SqliteParameter("@preview", preview ?? new byte[0]));

            dbCommand.ExecuteNonQuery();
        }

        CloseDB();
    }


    public void SetAssetLocation(int spaceId, string name, float x, float z)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Asset SET x = @x, z = @z WHERE name = @name AND spaceId = @spaceId";

            dbCommand.Parameters.Add(new SqliteParameter("@spaceId", spaceId));
            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@x", x));
            dbCommand.Parameters.Add(new SqliteParameter("@z", z));

            Debug.Log($"Executing SQL: {dbCommand.CommandText} with Parameters: SpaceID={spaceId}, Name={name}, X={x}, Z={z}");

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated. Check if the Asset with the given Name and SpaceID exists.");
            }
        }

        CloseDB();
    }

    public void SetAssetScale(int spaceId, string name, float scale)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Asset SET Scale = @scale WHERE SpaceID = @spaceID AND Name = @name";

            dbCommand.Parameters.Add(new SqliteParameter("@spaceID", spaceId));
            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@scale", scale));
            
            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated. Check if the Asset with the given ID and Name exists.");
            }
        }

        CloseDB();
    }


    public void SetAssetModel(int spaceId, string name, string modelUrl)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Asset SET model = @model WHERE name = @name AND spaceId = @spaceId";

            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@spaceId", spaceId));
            dbCommand.Parameters.Add(new SqliteParameter("@model", modelUrl));

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated. Check if the Asset with the given Name and SpaceID exists.");
            }
        }

        CloseDB();
    }

    public void SetAssetPreview(int spaceId, string name, byte[] preview)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Asset SET preview = @preview WHERE name = @name AND spaceId = @spaceId";

            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@spaceId", spaceId));
            dbCommand.Parameters.Add(new SqliteParameter("@preview", preview));

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated. Check if the Asset with the given Name and SpaceID exists.");
            }
        }

        CloseDB();
    }

    public string GetAssetModel(int spaceId, string name)
    {
        OpenDB();
        string modelUrl = null;

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT model FROM Asset WHERE name = @name AND spaceId = @spaceId";

            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@spaceId", spaceId));

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                if (dataReader.Read())
                {
                    modelUrl = dataReader.GetString(0);
                }
                else
                {
                    Debug.LogWarning("No data found with the specified name and spaceId.");
                }
            }
        }

        CloseDB();
        return modelUrl;
    }

    #endregion
}


