using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;

public class DatabaseConnect : MonoBehaviour
{
    [SerializeField] private string IN_ip = "127.0.0.1";
    [SerializeField] private string IN_dbId = "root";
    [SerializeField] private string IN_dbPassword = "1234";
    [SerializeField] private string IN_Db = "network_project_ky";

    private static MySqlConnection _connection;

    private void Start()
    {
        ConnectDb();
    }

    public void ConnectDb()
    {
        ConnectDb(IN_ip, IN_dbId, IN_dbPassword, IN_Db);
    }

    private void ConnectDb(string ip, string uid, string pwd, string db)
    {
        string _connectString =
        $"Server={ip};Uid={uid};Pwd={pwd};Database={db};";
        try
        {
            _connection = new MySqlConnection(_connectString);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString() + e.Message);
        }
    }

    public static DataSet RequestSelect(string quary, string tableName)
    {
        try
        {
            _connection.Open();

            MySqlCommand command = new MySqlCommand(quary, _connection);

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset, tableName);

            _connection.Close();

            return dataset;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString() + e.Message);
            return null;
        }
    }

    public static bool RequestUpdate(string quary, string tableName)
    {
        return RequestNonquary(quary, tableName);
    }
    public static bool RequestInsert(string quary, string tableName)
    {
        return RequestNonquary(quary, tableName);
    }
    private static bool RequestNonquary(string quary, string tableName)
    {
        try
        {
            _connection.Open();
            MySqlCommand command = new MySqlCommand(quary, _connection);

            command.ExecuteNonQuery();

            _connection.Close();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString() + e.Message);
            return false;
        }
    }

    public string DataSetToString(DataSet dataSet)
    {
        string resultStr = string.Empty;

        foreach(DataTable dataTable in dataSet.Tables)
        {
            foreach(DataRow row in dataTable.Rows)
            {
                foreach(DataColumn col in dataTable.Columns)
                {
                    resultStr += $"{col.ColumnName}: {row[col]}\n";
                }
            }
        }
        return resultStr;
    }

    public void Disconnect()
    {
        _connection.Close();
    }
}
