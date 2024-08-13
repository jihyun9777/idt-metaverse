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
                Debug.LogWarning("No rows were updated.");
            }
        }

        CloseDB();
    }

    public void SetSpacePreview(string name, byte[] preview)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Space SET preview = @preview WHERE name = @name";

            dbCommand.Parameters.Add(new SqliteParameter("@name", name));
            dbCommand.Parameters.Add(new SqliteParameter("@preview", preview));

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No preview were updated.");
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
                        ID = dataReader.GetInt32(0), 
                        SpaceID = dataReader.GetInt32(1),
                        Name = dataReader.GetString(2),
                        X = dataReader.IsDBNull(3) ? (float?)null : dataReader.GetFloat(3),
                        Z = dataReader.IsDBNull(4) ? (float?)null : dataReader.GetFloat(4),
                        Scale = dataReader.IsDBNull(5) ? (float?)null : dataReader.GetFloat(5),
                        Model = dataReader.GetString(6),
                        Preview = dataReader.IsDBNull(7) ? null : (byte[])dataReader[7]
                    };

                    assets.Add(asset);
                }
            }
        }

        CloseDB();

        return assets;
    }

    public AssetData SearchAsset(int id)
    {
        AssetData assetData = null;

        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT * FROM Asset WHERE ID = @id";

            dbCommand.Parameters.Add(new SqliteParameter("@id", id));

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                if (dataReader.Read())
                {
                    assetData = new AssetData
                    {
                        ID = dataReader.GetInt32(0),
                        SpaceID = dataReader.GetInt32(1),
                        Name = dataReader.GetString(2),
                        X = dataReader.IsDBNull(3) ? (float?)null : dataReader.GetFloat(3),
                        Z = dataReader.IsDBNull(4) ? (float?)null : dataReader.GetFloat(4),
                        Scale = dataReader.IsDBNull(5) ? (float?)null : dataReader.GetFloat(5),
                        Model = dataReader.GetString(6),
                        Preview = dataReader.IsDBNull(7) ? null : (byte[])dataReader[7]
                    };
                }
                else
                {
                    Debug.LogWarning("No asset found with the specified ID.");
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


    public void SetAssetLocation(int id, float x, float z)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Asset SET X = @x, Z = @z WHERE ID = @id";

            dbCommand.Parameters.Add(new SqliteParameter("@id", id));
            dbCommand.Parameters.Add(new SqliteParameter("@x", x));
            dbCommand.Parameters.Add(new SqliteParameter("@z", z));

            Debug.Log($"Executing SQL: {dbCommand.CommandText} with Parameters: ID={id}, X={x}, Z={z}");

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated.");
            }
        }

        CloseDB();
    }

    public void SetAssetScale(int id, float scale)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Asset SET Scale = @scale WHERE ID = @id";

            dbCommand.Parameters.Add(new SqliteParameter("@id", id));
            dbCommand.Parameters.Add(new SqliteParameter("@scale", scale));

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated.");
            }
        }

        CloseDB();
    }

    public void SetAssetModel(int id, string modelUrl)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Asset SET Model = @model WHERE ID = @id";

            dbCommand.Parameters.Add(new SqliteParameter("@id", id));
            dbCommand.Parameters.Add(new SqliteParameter("@model", modelUrl));

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated.");
            }
        }

        CloseDB();
    }

    public void SetAssetPreview(int id, byte[] preview)
    {
        OpenDB();

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "UPDATE Asset SET Preview = @preview WHERE ID = @id";

            dbCommand.Parameters.Add(new SqliteParameter("@id", id));
            dbCommand.Parameters.Add(new SqliteParameter("@preview", preview));

            int rowsAffected = dbCommand.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                Debug.LogWarning("No rows were updated.");
            }
        }

        CloseDB();
    }

    public string GetAssetModel(int id)
    {
        OpenDB();
        string modelUrl = null;

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT Model FROM Asset WHERE ID = @id";

            dbCommand.Parameters.Add(new SqliteParameter("@id", id));

            using (IDataReader dataReader = dbCommand.ExecuteReader())
            {
                if (dataReader.Read())
                {
                    modelUrl = dataReader.GetString(0);
                }
                else
                {
                    Debug.LogWarning("No data found with the specified ID.");
                }
            }
        }

        CloseDB();
        return modelUrl;
    }

    #endregion
}


